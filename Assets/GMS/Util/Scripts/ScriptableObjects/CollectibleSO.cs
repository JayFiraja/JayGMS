using UnityEngine;

namespace GMS
{
    /// <summary>
    /// Centralized data holder for collectible properties
    /// Look and Feel
    /// Value or any other parameter.
    /// </summary>
    [CreateAssetMenu(fileName = "New Collectible", menuName = "GMS/CollectibleSO")]
    public class CollectibleSO : ScriptableObject
    {
        [Header("Parameters")]
        public string CollectibleDisplayName;
        public string CollectibleKey;
        public Sprite Icon;
        /// <summary>
        /// value per unit
        /// </summary>
        public int Value;
        public bool randomXRotationWhenSpawned;
        public bool randomYRotationWhenSpawned;
        public bool randomZRotationWhenSpawned;
        public RotationRange RotationRange;

        [Header("View Prefabs")]
        public CollectibleItem collectibleItem;
        public CollectibleItem collectibleItemBuiltInRendering;

        [Header("SFX")]
        public AudioClip[] PickedUpClips;

        [Header("Effects Prefabs")]
        public ParticleSystem SpawnParticlePrefab;
        public ParticleSystem DeSpawnParticlePrefab;
    }
}