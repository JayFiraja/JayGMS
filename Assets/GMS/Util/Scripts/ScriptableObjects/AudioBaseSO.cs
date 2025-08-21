using UnityEngine;
using UnityEngine.Audio;

namespace GMS
{
    /// <summary>
    /// Scriptable object for holding base data for Audio SubManagers
    /// </summary>
    [CreateAssetMenu(fileName = "newAudioBase", menuName = "GMS/Data Collections/Audio Base")]
    public class AudioBaseSO : ScriptableObject
    {
        [Header("Settings")]
        public AudioMixer AudioMixer;
        public AudioMixerGroup VoiceGroup;
        public AudioMixerGroup UIGroup;
        public AudioMixerGroup SfxGroup;
        public AudioMixerGroup TrackGroup;

        [Header("Prefabs")]
        public AudioSource Voice_SFX_Prefab;
        public AudioSource UI_SFX_Prefab;
        public AudioSource PositionSFX_Prefab;

        // view content keys
        public string AudioSourceContentParentName = "Content_AudioSources";

        public string VoiceSourceCategory = "VoiceSource";
        public string VoiceSourceParent = "Voice SFX";

        public string UISourceName = "UISources";
        public string UISourceParent = "UI SFX";

        public string PositionSource = "PositionSources";
        public string PositionParent = "Position SFX";
    }
}
