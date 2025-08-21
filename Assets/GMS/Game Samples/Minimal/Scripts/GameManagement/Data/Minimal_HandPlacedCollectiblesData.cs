using UnityEngine;
namespace GMS.Samples
{
    /// <summary>
    /// This submanager is an example of a different functionality for spawning hand placed collectibles.
    /// It's used to show case the versatility of swaping SubManagers.
    /// </summary>
    [System.Serializable]
    [LinkDataLogic(typeof(Minimal_HandPlacedCollectiblesData), dataDisplayName: "Minimal_HandPlacedCollectiblesData", typeof(Minimal_HandPlacedCollectibles), displayName: "Minimal_HandPlacedCollectibles")]
    public struct Minimal_HandPlacedCollectiblesData : ISubManagerData
    {
        public CollectiblesToSpawn[] CollectiblesToSpawns;
        public string PoolTransformName;
        public bool UseBuiltInRenderingPrefab;
    }

    [System.Serializable]
    public struct CollectiblesToSpawn
    {
        public CollectibleSO CollectibleSO;
        public Vector3[] WorldPositions;
    }

    /// <summary>
    /// Wrapper class for spawning elements from a queue
    /// </summary>
    public class CollectibleToSpawn
    {
        public CollectibleSO CollectibleSO;
        public TransformData StartingTransformData;
        public string PoolCategory;
    }
}
