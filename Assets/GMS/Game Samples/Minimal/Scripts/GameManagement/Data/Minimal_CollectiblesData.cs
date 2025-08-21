using UnityEngine;

namespace GMS.Samples
{
    [System.Serializable]
    [LinkDataLogic(typeof(Minimal_CollectiblesData), dataDisplayName: "Minimal_CollectiblesData", typeof(Minimal_CollectiblesManager), displayName: "Minimal_CollectiblesManager")]
    public struct Minimal_CollectiblesData : ISubManagerData
    {
        public string CollectibleItemsAdressablesLabel;
        [Tooltip("Name used for the collectibles pool.")]
        public string CollectiblesParentName;
        /// <summary>
        /// Height in which the spawned collectible will be set.
        /// </summary>
        public float CollectibleHeight;
        [Tooltip("Minimum distance the player needs to travel to trigger collectible spawning.")]
        public float CollectibleSpawnDistanceThreshold;
        public Vector3 GridCenterPosition;
        public int SpawnAreaGridSize;
        public int MaxCollectiblesPerGrid;
        public int SpawnAreaWidth;
        public int SpawnAreaHeight;
        public int SpawnRadius;
        public int SpawnAttemptsRate;
        public bool UseBuiltInRenderingPrefab;
    }
}
