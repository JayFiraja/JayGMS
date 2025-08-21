using System.Text;
using TMPro;
using UnityEngine;

namespace GMS.Samples
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class UIReadPlayerTurn : MonoBehaviour
    {
        [SerializeField]
        private string playerTurnSuffix = "'s turn";
        [SerializeField]
        private string aI_Name = "AI";
        private TextMeshProUGUI _textMesh;

        #region Monobehaviour
        public void Awake()
        {
            TryGetComponent(out _textMesh);
            GameEventService.RegisterListener<TTCEvents.PlayerTurnChanged>(OnPlayerTurnChanged);
        }

        private void OnDestroy()
        {
            GameEventService.UnregisterListener<TTCEvents.PlayerTurnChanged>(OnPlayerTurnChanged);
        }
        #endregion Monobehaviour

        private void OnPlayerTurnChanged(TTCEvents.PlayerTurnChanged playerTurnChanged)
        {
            PlayerTurn newPlayer = playerTurnChanged.PlayerTurn;
            if (newPlayer == PlayerTurn.None)
            {
                _textMesh.SetText(string.Empty);
                return;
            }

            // we use string builder to avoid allocating heap memory AND garbage 
            StringBuilder sb = new StringBuilder();
            if (playerTurnChanged.IsSinglePlayer && newPlayer == PlayerTurn.PlayerB)
            {
                sb.Append(aI_Name);
            }
            else
            {
                sb.Append(newPlayer);
            }
            sb.Append(playerTurnSuffix);
            _textMesh.SetText(sb.ToString());
            sb.Clear();
        }
    }
}
