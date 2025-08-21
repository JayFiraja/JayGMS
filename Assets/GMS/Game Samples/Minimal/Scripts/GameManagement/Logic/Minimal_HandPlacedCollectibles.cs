using System.Collections.Generic;
using UnityEngine;

namespace GMS.Samples
{
    public class Minimal_HandPlacedCollectibles: ISubManager
    {
        private Minimal_HandPlacedCollectiblesData _data;
        private VFXManager _vFXManager;
        private AudioManager _audioManager;
        private Minimal_UIManager _minimalUIManager;

        private Dictionary<string, CollectibleBundle> _collectibleBundles; // bundles keep track of the collected values per CollectibleSO

        /// <summary>
        /// items in the inspector, kept here for inspector organization and cleanup purposes
        /// </summary>
        private ViewContent<CollectibleItem> _collectibleViewItems;

        private Queue<CollectibleToSpawn> _collectiblesToSpawn;

        public Minimal_HandPlacedCollectibles(Minimal_HandPlacedCollectiblesData data)
        {
            _data = data;
        }

        public bool Initialize(GameManager gameManager)
        {
            gameManager.TryGetSubManager(out _vFXManager);
            gameManager.TryGetSubManager(out _audioManager);
            gameManager.TryGetSubManager(out _minimalUIManager);
            
            GameEventService.RegisterListener<MinimalGameEvents.ItemPickedUp>(OnCollectiblePickedup);

            _collectibleViewItems = new ViewContent<CollectibleItem>(_data.PoolTransformName, GameInstance.S.transform);

            _collectibleBundles = new Dictionary<string, CollectibleBundle>();
            _collectiblesToSpawn = new Queue<CollectibleToSpawn>();

            int collectiblesCount = _data.CollectiblesToSpawns.Length;
            for (int i = 0; i < collectiblesCount; i++)
            {
                CollectiblesToSpawn collectiblesToSpawn = _data.CollectiblesToSpawns[i];
                CollectibleSO collectibleSO = collectiblesToSpawn.CollectibleSO;
                if (_collectibleBundles.TryGetValue(collectibleSO.CollectibleKey, out CollectibleBundle collectibleBundle))
                {
                    continue;
                }

                collectibleBundle = new CollectibleBundle();
                collectibleBundle.Initialize(collectibleSO);
                _collectibleBundles.Add(collectibleSO.CollectibleKey, collectibleBundle);

                // now spawn the collectibles at an interval
                int positions = collectiblesToSpawn.WorldPositions.Length;
                string poolCategory = collectibleSO.CollectibleKey;

                for (int p = 0; p < positions; p++)
                {
                    TransformData startingTransformData = new TransformData(collectiblesToSpawn.WorldPositions[p], Quaternion.identity, Vector3.one);
                    CollectibleToSpawn collectibleToSpawn = new CollectibleToSpawn
                    {
                        CollectibleSO = collectibleSO,
                        StartingTransformData = startingTransformData,
                        PoolCategory = poolCategory
                    };

                    _collectiblesToSpawn.Enqueue(collectibleToSpawn);
                }
            }

            return true;
        }

        public void UnInitialize()
        {
            GameEventService.UnregisterListener<MinimalGameEvents.ItemPickedUp>(OnCollectiblePickedup);
            _collectibleViewItems.DestroyViewContent(_data.PoolTransformName);
        }

        public void OnUpdate()
        {
            if (_collectiblesToSpawn.Count > 0)
            {
                InstanceCollectible(_collectiblesToSpawn.Dequeue());
            }
        }

        public bool Equals(ISubManager other)
        {
            // Check if other is null
            if (other == null)
            {
                return false;
            }

            // Compare the runtime types of the current instance and the other instance
            return GetType() == other.GetType();
        }

        public override int GetHashCode()
        {
            // Generate a hash code based on the fields that contribute to equality
            return GetType().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is ISubManager other)
            {
                return Equals(other);
            }
            return false;
        }

        private void OnCollectiblePickedup(MinimalGameEvents.ItemPickedUp itemPickedUp)
        {
            ProcessPickeduUpItem(itemPickedUp.PickupItemUp, itemPickedUp.PickedByPlayableCharacter, _collectibleBundles);
        }

        public void ProcessPickeduUpItem(IPickUp item, bool byPlayableCharacter, Dictionary<string, CollectibleBundle> collectibles)
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

        private void InstanceCollectible(CollectibleToSpawn collectibleToSpawn)
        {
            CollectibleItem prefabToUse = _data.UseBuiltInRenderingPrefab ?
                                                                            collectibleToSpawn.CollectibleSO.collectibleItemBuiltInRendering :
                                                                            collectibleToSpawn.CollectibleSO.collectibleItem;

            CollectibleItem newCollectibleItem = _collectibleViewItems.GetOrCreate(prefab: prefabToUse,
                                                   category: collectibleToSpawn.PoolCategory, categoryParentName: collectibleToSpawn.PoolCategory);

            newCollectibleItem.Initialize(collectibleToSpawn.CollectibleSO, collectibleToSpawn.StartingTransformData);
        }
    }
}
