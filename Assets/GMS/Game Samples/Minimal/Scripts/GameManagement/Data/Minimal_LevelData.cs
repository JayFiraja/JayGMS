using UnityEngine;

namespace GMS.Samples
{
    [System.Serializable]
    [LinkDataLogic(typeof(Minimal_LevelData), dataDisplayName: "Minimal_LevelData", typeof(Minimal_LevelManager), displayName: "Minimal_LevelManager")]
    public struct Minimal_LevelData : ISubManagerData
    {
        [AddressableSelector]
        public string LevelPrefabAddress;

        public Vector3 StartLevelPosition;
        public Vector3 StartLevelRotation;
        public Vector3 StartLevelScale;
    }
}
