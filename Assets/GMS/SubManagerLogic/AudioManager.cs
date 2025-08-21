using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GMS
{
    /// <summary>
    /// Plays Audio clips on demand
    /// Utilizes a simple pooling system and sets the correct AudioMizer group depending on the audio requested.
    /// </summary>
    public class AudioManager: ISubManager
    {
        private AudioManagerData _data;
        private AudioBaseSO _audioSO;

        private const float DORMANT_AUDIO_SOURCES_CHECK_RATE = 3f;
        private float _dormantAudioCheckCooldown;

        [HideInInspector, NonSerialized]
        public ViewContent<AudioSource> _audioSources;

        /// <summary>
        /// cache variable
        /// </summary>
        private AudioSource _aSource;
        // working variable
        private List<AudioSource> _aSources;
        private int lastPosPlayed;

        public bool Initialized { get; private set; }
        
        
        public AudioManager(AudioManagerData data)
        {
            _data = data;
        }
         
        public bool Initialize(GameManager gameManager)
        {
            // we initialize our audio source pooling here.
            Addressables.LoadAssetAsync<AudioBaseSO>(_data.AudioBaseAddress).Completed += OnAudioBaseLoaded;
            return true;
        }
        
        public void UnInitialize()
        {
            Addressables.Release(_data.AudioBaseSO);
            Initialized = false;
        }

        public void OnUpdate()
        {
            if (Cooldown.IsCooled(_dormantAudioCheckCooldown, DORMANT_AUDIO_SOURCES_CHECK_RATE))
            {
                _dormantAudioCheckCooldown = Cooldown.Cool();
                UpdateAudioSourcesToDormant(_audioSO.PositionSource);
                UpdateAudioSourcesToDormant(_audioSO.VoiceSourceCategory);
                UpdateAudioSourcesToDormant(_audioSO.UISourceName);
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

        private void UpdateAudioSourcesToDormant(string category)
        {
            if (_audioSources.GetPool(category, out _aSources))
            {
                for (int i = _aSources.Count-1; i >=0 ; i--)
                {
                    _aSource = _aSources[i];

                    if (!_aSource.isPlaying)
                    {
                        _audioSources.ReturnToPool(_aSource);
                    }
                }
            }
        }

        private void OnAudioBaseLoaded(AsyncOperationHandle<AudioBaseSO> obj)
        {
            if (obj.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to instantiate ScriptableObject with address{_data.AudioBaseAddress}.");
                return;
            }

            _data.AudioBaseSO = obj.Result;
            _audioSO = _data.AudioBaseSO;

            _audioSources = new ViewContent<AudioSource>(_data.AudioBaseSO.AudioSourceContentParentName, GameInstance.S.transform);
            Initialized = true;
        }

        private void GetAudioSourceFromGroup(string category, string parentName, AudioMixerGroup audioMixerGroup, AudioSource prefab, out AudioSource audioSource)
        {
            audioSource = _audioSources.GetOrCreate(prefab, category);
            audioSource.outputAudioMixerGroup = audioMixerGroup;
        }

        private void GetAudioSourceFromGroupAtPosition(Vector3 position, out AudioSource audioSource)
        {
            TransformData transformData = TransformData.Default;
            transformData.Position = position;
            audioSource = _audioSources.GetOrCreate(_audioSO.PositionSFX_Prefab, _audioSO.PositionSource, _audioSO.PositionParent, transformData);
           
            audioSource.outputAudioMixerGroup = _audioSO.SfxGroup;
        }

        public void PlayVoiceSFX(AudioClip[] fx)
        {
            GetAudioSourceFromGroup(_audioSO.VoiceSourceCategory, _audioSO.VoiceSourceParent, _audioSO.VoiceGroup, _audioSO.Voice_SFX_Prefab, out _aSource);

            if (!TryGetRandomSoundEffect(fx, out AudioClip availableClip))
            {
                return;
            }

            _aSource.clip = availableClip;
            _aSource.Play();
        }

        /// <summary>
        /// Plays a UISfx
        /// </summary>
        /// <param name="a"></param>
        public void PlayUISFX(AudioClip audioClip)
        {
            GetAudioSourceFromGroup(_audioSO.UISourceName, _audioSO.UISourceParent, _audioSO.UIGroup, _audioSO.Voice_SFX_Prefab, out _aSource);

            _aSource.clip = audioClip;
            _aSource.Play();
        }

        /// <summary>
        /// Plays a random clip from an array of AudioClips.
        /// </summary>
        /// <param name="audioClip">array of audioclips to play</param>
        /// <param name="position">Position in World Coordinates</param>
        /// </summary>
        public void PlayClipAt(AudioClip[] audioClip, Vector3 position)
        {
            if (!TryGetRandomSoundEffect(audioClip, out AudioClip availableClip))
            {
                return;
            }

            PlayClipAt(availableClip, position);
        }

        /// <summary>
        /// Plays a specific audio clip
        /// </summary>
        /// <param name="audioClip"></param>
        /// <param name="position"></param>
        public void PlayClipAt(AudioClip audioClip, Vector3 position)
        {
            GetAudioSourceFromGroupAtPosition(position, out _aSource);
            _aSource.clip = audioClip;
            _aSource.Play();
        }

        private bool TryGetRandomSoundEffect(AudioClip[] clips, out AudioClip availableClip)
        {
            availableClip = null;
            if (clips == null || clips.Length <= 0)
            {
                return false;
            }

            int randomClip = UnityEngine.Random.Range(0, clips.Length);
            availableClip = clips[randomClip];

            return availableClip != null;
        }

    }
}
