using UnityEngine;

namespace GMS.Samples
{
    public class UI_PlayAudioOnClick : MonoBehaviour
    {
        private GameManager _gameManager;
        [SerializeField, Tooltip("Plays the clip on click")]
        private AudioClip clipToPlay;
        private TicTacToeAudio _audioManager;

        /// <summary>
        /// Plays the given clip on AudioManager
        /// </summary>
        public void PlayUIAudioClip()
        {
            if (_audioManager == null && !GameInstance.TryGetSubManager(out _audioManager))
            {
                Debug.LogError("Audio Manager: TicTacToeAudio is missing");
                return;
            }
            _audioManager.PlayUISFX(clipToPlay);
        }
    
    }
}
