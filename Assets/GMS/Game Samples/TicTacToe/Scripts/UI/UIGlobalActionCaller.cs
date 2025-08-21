using UnityEngine;

namespace GMS.Samples
{
    /// <summary>
    /// Helper UI class for firing Game events with certain data button actions, through Unity Events
    /// </summary>
    public class UIGlobalActionCaller : MonoBehaviour
    {
        /// <summary>
        /// Starts the game as single or multiplayer
        /// </summary>
        /// <param name="singlePlayer">True for player vs AI, false for Player vs Player</param>
        public void StartGameLoop(bool singlePlayer)
        {
            TTCEvents.StartGameLoop startGameLoopEvent = GameEventService.GetEvent<TTCEvents.StartGameLoop>();
            startGameLoopEvent.PlayerTurn = PlayerTurn.PlayerA;
            startGameLoopEvent.IsSinglePlayer = singlePlayer;
            GameEventService.TriggerEvent(startGameLoopEvent);
        }

        /// <summary>
        /// Tries to navigate to the MainMenu
        /// </summary>
        public void TryGoToMainMenu()
        {
            TTCEvents.GoToGameState goToGameStateEvent = GameEventService.GetEvent<TTCEvents.GoToGameState>();
            goToGameStateEvent.NewGameState = GameState.MainMenu;
            GameEventService.TriggerEvent(goToGameStateEvent);
        }
    }
}
