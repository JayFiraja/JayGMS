using GMS;
using UnityEngine;

namespace GMS.Samples
{
    /// <summary>
    /// This class uses composition, which is a design principle of using objects of other types, rather than inheriting from them.
    /// This allows for greater flexibility and reusability.
    /// </summary>
    public class TicTacToeAudio : ISubManager
    {
        // instead of inheriting from the base manager, we simply use an instance of it here.
        private AudioManager _audioManager;
        private TicTacToeAudioData _data;

        public TicTacToeAudio(TicTacToeAudioData ticTacToeData)
        {
            _data = ticTacToeData;
            _audioManager = new AudioManager(_data.BaseManagerData);
        }

        public bool Initialize(GMS.GameManager gameManager)
        {
            _audioManager.Initialize(gameManager);
            return true;
        }

        public void UnInitialize()
        {
            _audioManager.UnInitialize();
        }

        public void OnUpdate()
        {
            _audioManager.OnUpdate();
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

        public override bool Equals(object obj)
        {
            if (obj is ISubManager other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            // Generate a hash code based on the fields that contribute to equality
            return GetType().GetHashCode();
        }

        public void PlayVoiceSFX(AudioClip[] sfx)
        {
            _audioManager.PlayVoiceSFX(sfx);
        }

        public void PlayUISFX(AudioClip audioClip)
        {
            _audioManager.PlayUISFX(audioClip);
        }

        public void PlayClipAt(AudioClip[] audioClip, Vector3 position)
        {
            _audioManager.PlayClipAt(audioClip, position);
        }

        public void PlayWinVoice()
        {
            _audioManager.PlayVoiceSFX(_data.WinnerSfx);
        }

        public void PlayDraw()
        {
            _audioManager.PlayVoiceSFX(_data.DrawSfx);
        }

        public void PlayLightOn()
        {
            _audioManager.PlayUISFX(_data.lightTurnedOn);
        }

        public void PlayFallingMarker(Vector3 position)
        {
            _audioManager.PlayClipAt(_data.FallingPiecesSfx, position);
        }

        public void PlayMarkerFallImpact(Vector3 position)
        {
            _audioManager.PlayClipAt(_data.FallenPiecesSfx, position);
        }
    }
}
