using UnityEngine;

namespace GMS.Samples
{
    [System.Serializable]
    [LinkDataLogic(typeof(Minimal_CharacterData), dataDisplayName: "Minimal_CharacterData", typeof(Minimal_CharacterManager), displayName: "Minimal_CharacterManager")]
    public struct Minimal_CharacterData : ISubManagerData
    {
        public string TransformContentName;
        public float SpawnAllCooldown;
        public float SpawnInterval;
        public SpawnCharacter[] SpawnCharacters;
    }

    /// <summary>
    /// Simple wrapper for character spawner.
    /// </summary>
    [System.Serializable]
    public struct SpawnCharacter 
    {
        public Vector3 startPosition;
        public Vector3 startRotation;
        public Vector3 startScale;
        public MinimalCharacterData CharactersToSpawn;
    }
}
