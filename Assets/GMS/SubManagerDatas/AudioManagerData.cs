using UnityEngine;

namespace GMS
{
    [System.Serializable]
    [LinkDataLogic(typeof(AudioManagerData), dataDisplayName: "AudioManagerData", typeof(AudioManager), displayName: "AudioManager")]
    public struct AudioManagerData : ISubManagerData
    {
        [AddressableSelector]
        public string AudioBaseAddress;

        // Assigned in-game when loading the addressable key
        [HideInInspector]
        public AudioBaseSO AudioBaseSO;
    }
}
