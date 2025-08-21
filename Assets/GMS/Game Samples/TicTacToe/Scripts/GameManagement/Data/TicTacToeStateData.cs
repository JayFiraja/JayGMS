using GMS;
using JetBrains.Annotations;
using UnityEngine;
namespace GMS.Samples
{
    [System.Serializable]
    [LinkDataLogic(typeof(TicTacToeStateData), dataDisplayName: "TicTacToe StateData", typeof(TicTacToeStateManager), displayName: "TicTacToeStateManager")]
    public struct TicTacToeStateData : ISubManagerData
    {
        // mini analytics keys example
        public const string TOTAL_TURN_COUNT = "TOTAL_PLAY_COUNTS";
        public const string TOTAL_WIN_PLAYER_COUNT = "TOTAL_WINS_PLAYER_COUNT";
        public const string TOTAL_WINS_AI_COUNT = "TOTAL_WINS_COUNT";

        [Header("Parameters")]
        public float TimeToStartMainMenu;
        public float TimeToCallNextPlayerTurn;
        public float TimeToMoveFromResultsToMainMenu;

        public PlayerData[] PlayerDatas;
    }
}
