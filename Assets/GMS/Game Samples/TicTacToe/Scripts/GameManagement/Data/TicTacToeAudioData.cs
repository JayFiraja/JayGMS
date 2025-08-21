using GMS;
using UnityEngine.Audio;
using UnityEngine;

namespace GMS.Samples
{
    [System.Serializable]
    [LinkDataLogic(typeof(TicTacToeAudioData), dataDisplayName: "TicTacToe AudioData", typeof(TicTacToeAudio), displayName: "TicTacToeAudio")]
    public struct TicTacToeAudioData : ISubManagerData
    {
        [Header("Base Data")]
        public AudioManagerData BaseManagerData;

        [Header("Sfx")]
        public AudioClip[] FallingPiecesSfx;
        public AudioClip[] FallenPiecesSfx;
        public AudioClip[] DrawSfx;
        public AudioClip[] WinnerSfx;
        public AudioClip lightTurnedOn;
    }
}
