using System.Collections.Generic;

namespace GMS.Samples
{
    public class Minimal_CharacterManager: ISubManager
    {
        private GameManager _gameManager;
        private Minimal_CharacterData _data;
        private TimeManager _timeManager;

        private ViewContent<MinimalCharacterView> _charactersViewContent;
        private const string POOL_CATEGORY_NAME = "Characters";

        private List<MinimalCharacter> _characters = new List<MinimalCharacter>(0);
        private Queue<SpawnCharacter> _charactersToSpawn = new Queue<SpawnCharacter>(0);
        private Queue<MinimalCharacter> _charactersToAdd =  new Queue<MinimalCharacter>(0);
        private Queue<MinimalCharacter> _charactersToRemove = new Queue<MinimalCharacter>(0);

        public Minimal_CharacterManager(Minimal_CharacterData data)
        {
            _data = data;
        }

        public bool Initialize(GameManager gameManager)
        {
            gameManager.TryGetSubManager(out _timeManager);
            _charactersViewContent = new ViewContent<MinimalCharacterView>(_data.TransformContentName, GameInstance.S.transform);
            _timeManager?.AddCooldown(_data.SpawnAllCooldown, InstanceALLCharacters);
            
            return true;
        }

        public void UnInitialize()
        {
            // Cleanup, ie. unload instances, send cleanup messages
        }

        public void OnUpdate()
        {
            foreach (MinimalCharacter character in _characters) 
            {
                character.OnUpdate();
            }

            ProcessPendingCharacters();
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

        private void ProcessPendingCharacters()
        {
            if (_charactersToSpawn.Count > 0)
            {
                SpawnCharacter spawnCharacter = _charactersToSpawn.Dequeue();
                InstanceCharacter(spawnCharacter);
            }

            if (_charactersToAdd.Count > 0)
            {
                MinimalCharacter minimalCharacter =  _charactersToAdd.Dequeue();
                _characters.Add(minimalCharacter);
            }

            if (_charactersToRemove.Count > 0)
            {
                MinimalCharacter minimalCharacter = _charactersToRemove.Dequeue();
                _characters.Remove(minimalCharacter);
            }
        }

        public void InstanceALLCharacters()
        {
            SpawnCharacter[] spawnCharacters = _data.SpawnCharacters;

            for (int i = 0; i < spawnCharacters.Length; i++) 
            {
                int index = i;
                SpawnCharacter characterData = spawnCharacters[index];
                _timeManager?.AddCooldown(index * _data.SpawnInterval, EnqueueSpawnCharacter, characterData);
            }
        }

        public void EnqueueSpawnCharacter(SpawnCharacter spawnCharacter)
        {
            _charactersToSpawn.Enqueue(spawnCharacter);
        }

        public void EnqueueCharacter(MinimalCharacter minimalCharacter)
        {
            _charactersToAdd.Enqueue(minimalCharacter);
        }

        public void RemoveCharacter(MinimalCharacter minimalCharacter)
        {
            _charactersToRemove.Enqueue(minimalCharacter);
        }

        private void InstanceCharacter(SpawnCharacter spawnCharacter)
        {
            MinimalCharacterData characterData = spawnCharacter.CharactersToSpawn;
            MinimalCharacterView minimalCharacterView = _charactersViewContent.GetOrCreate(characterData.characterPrefab, POOL_CATEGORY_NAME);
            minimalCharacterView.Initialize(characterData);
            
            TransformData startingTransformData = new TransformData(
                spawnCharacter.startPosition,
                spawnCharacter.startRotation,
                spawnCharacter.startScale
                );

            MinimalCharacter newMinimalCharacter = new MinimalCharacter(characterData, minimalCharacterView, startingTransformData);
            EnqueueCharacter(newMinimalCharacter);
            if (spawnCharacter.CharactersToSpawn.IsPlayerControlled)
            {
                MinimalGameEvents.OnPlayerInstanced playerInstancedEvent = GameEventService.GetEvent<MinimalGameEvents.OnPlayerInstanced>();
                playerInstancedEvent.PlayerGameObject = minimalCharacterView.gameObject;
                GameEventService.TriggerEvent(playerInstancedEvent);
            }
        }
    }
}
