using UnityEngine;

namespace GMS
{
    /// <summary>
    /// Data class for the NPCManager logic class.
    /// </summary>
    [System.Serializable]
    [LinkDataLogic(typeof(NPCManagerData), dataDisplayName:"NPC Manager Data", typeof(NPCManager), displayName: "NPC Manager")]
    public struct NPCManagerData : ISubManagerData
    {
        [Header("Example Fields")]
        public int maxEnemyCount;
        public AnimationCurve animationCurve;
        public GameObject prefab;

        [Header("Scriptable object selector")]
        [AddressableSelector]
        public string LevelsDataKey;
    }
}
