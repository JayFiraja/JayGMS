using UnityEngine;
using UnityEngine.UI;

namespace GMS.Samples
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UI_DisplayUnlockables : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private LayoutElement playerARef;
        [SerializeField]
        private LayoutElement playerBRef;

        private void Start()
        {
            TryGetComponent(out _canvasGroup);
            _canvasGroup.ToggleCanvasGroup(false);
            GameEventService.RegisterListener<TTCEvents.GameStateChanged>(OnGameStateChanged);
        }

        private void OnDestroy()
        {
            GameEventService.UnregisterListener<TTCEvents.GameStateChanged>(OnGameStateChanged);
        }

        private void OnGameStateChanged(TTCEvents.GameStateChanged gameStateChanged)
        {
            if (gameStateChanged.ToGameState != GameState.MainMenu)
            {
                return;
            }
            int playerAWins = PlayerPrefs.GetInt(PlayerTurn.PlayerA.ToString());
            int playerBWins = PlayerPrefs.GetInt(PlayerTurn.PlayerB.ToString());

            bool hasPlayerAWins = playerAWins > 0;
            bool hasPlayerBWins = playerBWins > 0;

            if (hasPlayerAWins || hasPlayerBWins)
            {
                _canvasGroup.ToggleCanvasGroup(true);
            }
            playerARef.ignoreLayout = !hasPlayerAWins;
            playerBRef.ignoreLayout = !hasPlayerBWins;
        }
    }
}
