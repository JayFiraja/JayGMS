using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GMS.Samples
{
    /// <summary>
    /// Instances and keeps track of collectibles
    /// </summary>
    public class Minimal_CollectiblesManager: ISubManager
    {
        private Minimal_CollectiblesData _data;
        private VFXManager _vFXManager;
        private AudioManager _audioManager;
        private Minimal_UIManager _minimalUIManager;

        private Dictionary<string, CollectibleBundle> _collectibles;
        /// <summary>
        /// elements sent to work with the spawnable grid system
        /// </summary>
        private BoundsGridSpawnables<SpawnableEntry<CollectibleItem>> _boundsGridSpawnables;
        /// <summary>
        /// items in the inspector, kept here for inspector organization and cleanup purposes
        /// </summary>
        private ViewContent<CollectibleItem> _collectibleViewItems;

        private Queue<CollectibleSO> _randomCollectibleQueue;
        /// <summary>
        /// Const value used to sort and mix a max count of elements.
        /// Used for organic randomness.
        /// </summary>
        private const int UNIQUE_ELEMENTS_COUNT = 3;

        // working variable for capturing the Addressable result
        private IList<CollectibleSO> _collectibleSO;

        private bool _isInitialized = false;

        // working variable for the last position registered when the spawn occured after the C=Player controlled character moved
        private Vector3 _lastPositionSinceSpawn;

        public Minimal_CollectiblesManager(Minimal_CollectiblesData data)
        {
            _data = data;
            _collectibles = new Dictionary<string, CollectibleBundle>();
            _boundsGridSpawnables = new BoundsGridSpawnables<SpawnableEntry<CollectibleItem>>();
            _randomCollectibleQueue = new Queue<CollectibleSO>();
        }

        public bool Initialize(GameManager gameManager)
        {
            gameManager.TryGetSubManager(out  _vFXManager);
            gameManager.TryGetSubManager(out _audioManager);
            gameManager.TryGetSubManager(out _minimalUIManager);
            
            GameEventService.RegisterListener<MinimalGameEvents.ItemPickedUp>(OnCollectiblePickedup);
            GameEventService.RegisterListener<MinimalGameEvents.OnControllableCharacterMoved>(OnControllableCharacterMoved);

            _collectibleViewItems = new ViewContent<CollectibleItem>(_data.CollectiblesParentName, GameInstance.S.transform);
            Addressables.LoadAssetsAsync<CollectibleSO>(_data.CollectibleItemsAdressablesLabel, null).Completed += OnCollectiblesLoaded;
            _boundsGridSpawnables = new BoundsGridSpawnables<SpawnableEntry<CollectibleItem>>();
            _boundsGridSpawnables.Initialize(_data.SpawnAreaGridSize, _data.MaxCollectiblesPerGrid,
                                             _data.SpawnAreaWidth, _data.SpawnAreaHeight, _data.GridCenterPosition);

            return true;
        }

        public void UnInitialize()
        {
            if (_collectibleSO != null)
            {
                Addressables.Release(_collectibleSO);
                _collectibleSO = null;
            }
            
            GameEventService.UnregisterListener<MinimalGameEvents.ItemPickedUp>(OnCollectiblePickedup);
            GameEventService.UnregisterListener<MinimalGameEvents.OnControllableCharacterMoved>(OnControllableCharacterMoved);
            _collectibleViewItems.DestroyViewContent(_data.CollectiblesParentName);
        }

        public void OnUpdate()
        {
        }

        public bool Equals(ISubManager other)
        {
            if (other == null) return false;

            // Compare the runtime types of the current instance and the other instance
            return GetType() == other.GetType();
        }

        public override bool Equals(object obj)
        {
            if (obj is ISubManager other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            // Generate a hash code based on the fields that contribute to equality
            return GetType().GetHashCode();
        }
        private void OnCollectiblesLoaded(AsyncOperationHandle<IList<CollectibleSO>> handle)
        {
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to load CollectibleSO with address{_data.CollectibleItemsAdressablesLabel}.");
                return;
            }

            _collectibleSO = handle.Result;

            foreach (CollectibleSO collectible in _collectibleSO)
            {
                CollectibleBundle collectibleBundle = new CollectibleBundle();
                collectibleBundle.Initialize(collectible);

                _collectibles.Add(collectible.CollectibleKey, collectibleBundle);
            }
            _isInitialized = true;
        }

        private void OnControllableCharacterMoved(MinimalGameEvents.OnControllableCharacterMoved onControllableCharacterMoved)
        {
            Vector3 newPosition = onControllableCharacterMoved.NewPosition;
            if (!_isInitialized)
            {
                return;
            }

            if (Vector3.Distance(newPosition, _lastPositionSinceSpawn) < _data.CollectibleSpawnDistanceThreshold)
            {
                return;
            }

            if (!_boundsGridSpawnables.TryGetSpawnPoint(newPosition, _data.SpawnRadius, _data.SpawnAttemptsRate, out var spawnPoint))
            {
                return;
            }
            if (!TryGetRandomCollectibleBundle(out CollectibleSO collectibleSO))
            {
                return;
            }

            InstantiateCollectible(collectibleSO, spawnPoint);
            _lastPositionSinceSpawn = newPosition;
        }

        private bool TryGetRandomCollectibleBundle(out CollectibleSO collectibleBundle)
        {
            collectibleBundle = null;

            // refresh the queue
            if (_randomCollectibleQueue.Count == 0)
            {
                // Shuffle the collectibles
                List<CollectibleBundle> shuffledCollectibles = new List<CollectibleBundle>(_collectibles.Values);
                CollectionUtilities.ShuffleList(shuffledCollectibles);

                // Enqueue each collectible multiple times
                foreach (CollectibleBundle bundle in shuffledCollectibles)
                {
                    for (int i = 0; i < UNIQUE_ELEMENTS_COUNT; i++)
                    {
                        _randomCollectibleQueue.Enqueue(bundle.CollectibleSO);
                    }
                }
            }

            // Dequeue a collectible bundle from the queue
            if (_randomCollectibleQueue.Count > 0)
            {
                collectibleBundle = _randomCollectibleQueue.Dequeue();
                return true;
            }

            return false;
        }

        public void InstantiateCollectible(CollectibleSO collectibleSO, Vector3 spawnPosition)
        {
            string poolCategory = collectibleSO.CollectibleKey;

            Quaternion generatedRotation = QuaternionUtilities.CreateRandomQuaternion(collectibleSO.randomXRotationWhenSpawned,  
                                                                                      collectibleSO.randomYRotationWhenSpawned,
                                                                                      collectibleSO.randomZRotationWhenSpawned,
                                                                                      collectibleSO.RotationRange);
            Vector3 position = spawnPosition;
            position.y = _data.CollectibleHeight;
            TransformData startingTransformData = new TransformData 
            {
                Position = position,
                Rotation = generatedRotation,
                Scale = Vector3.one
            };

            CollectibleItem prefabToUse = _data.UseBuiltInRenderingPrefab ?
                                                                           collectibleSO.collectibleItemBuiltInRendering :
                                                                           collectibleSO.collectibleItem;
            CollectibleItem newCollectibleItem = _collectibleViewItems.GetOrCreate(prefab: prefabToUse, 
                                                                                   category: poolCategory, categoryParentName: poolCategory);

            newCollectibleItem.Initialize(collectibleSO, startingTransformData);
            SpawnableEntry<CollectibleItem> newItem = new SpawnableEntry<CollectibleItem>(newCollectibleItem, startingTransformData);

            _boundsGridSpawnables.AddSpawnable(newCollectibleItem.GetInstanceID(), newItem, startingTransformData);
        }

        private void OnCollectiblePickedup(MinimalGameEvents.ItemPickedUp itemPickedUp)
        {
            ProcessPickedUpItem(itemPickedUp.PickupItemUp, itemPickedUp.PickedByPlayableCharacter, _collectibles);
        }

        public void ProcessPickedUpItem(IPickUp item, bool byPlayableCharacter, Dictionary<string, CollectibleBundle> collectibles)
        {
            if ((item.HasBeenPickedUp))
            {
                return;
            }

            if (!(item is CollectibleItem collectibleItem))
            {
                return;
            }

            CollectibleSO collectibleData = collectibleItem.GetCollectibleData();

            // register picked up value
            if (collectibles.TryGetValue(collectibleData.CollectibleKey, out CollectibleBundle bundle))
            {
                bundle.AddCollectedValue(collectibleData.Value);
            }

            item.PickUp();

            Vector3 position = collectibleItem.transform.position;

            _audioManager?.PlayClipAt(collectibleData.PickedUpClips, position);
            _vFXManager?.PlayParticle(collectibleData.DeSpawnParticlePrefab, position, Vector3.zero);
            _minimalUIManager?.DisplayCollectedItemNewValue(collectibleData, bundle.CollectedValue);
        }
    }
}
