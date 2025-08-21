using System.Text;
using TMPro;
using UnityEngine;

namespace GMS.Samples
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class UIReadPlayerWon : MonoBehaviour
    {
        [SerializeField]
        private string DrawDisplay = "Draw!";
        [SerializeField]
        private string WinDisplay = " wins!";
        [SerializeField]
        private string AIDisplay = "AI";
        private TextMeshProUGUI _textMesh;

        #region Monobehaviour
        public void Awake()
        {
            TryGetComponent(out _textMesh);
            
            GameEventService.RegisterListener<TTCEvents.PlayerWon>(OnPlayerWon);
            GameEventService.RegisterListener<TTCEvents.GameEndedInDraw>(OnGameEndedInDraw);
        }

        private void OnDestroy()
        {
            GameEventService.UnregisterListener<TTCEvents.PlayerWon>(OnPlayerWon);
            GameEventService.UnregisterListener<TTCEvents.GameEndedInDraw>(OnGameEndedInDraw);
        }

        #endregion Monobehaviour

        private void OnPlayerWon(TTCEvents.PlayerWon playerWon)
        {
            PlayerTurn winningPlayer = playerWon.PlayerTurn;
            
            if (winningPlayer == PlayerTurn.None)
            {
                _textMesh.SetText(string.Empty);
                return;
            }

            // we use string builder to avoid allocating heap memory AND garbage 
            StringBuilder sb = new StringBuilder();
            if (playerWon.IsSinglePlayer && winningPlayer == PlayerTurn.PlayerB)
            {
                sb.Append(AIDisplay);
            }
            else
            {
                sb.Append(winningPlayer);
            }
            sb.Append(WinDisplay);
            _textMesh.SetText(sb.ToString());
            sb.Clear();
        }

        private void OnGameEndedInDraw(TTCEvents.GameEndedInDraw gameEndedInDraw)
        {
            _textMesh.SetText(DrawDisplay);
        }
    }
}
