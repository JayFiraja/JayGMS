using UnityEngine;
using UnityEngine.UI;

namespace GMS.Samples
{
    [RequireComponent(typeof(Toggle))]
    public class UI_ToggleAlternativeMesh : MonoBehaviour
    {
        [SerializeField]
        private PlayerTurn player;

        private PlayerData _playerData;
        private TicTacToeStateManager _stateManager;

        private Toggle _toggle;

        private void Start()
        {
            TryGetComponent(out _toggle);
            GameInstance.TryGetSubManager(out  _stateManager);
            _stateManager?.TryGetPlayerTurnData(player, out _playerData);

            _toggle.isOn = _playerData.usingAlternativeMark;
            _toggle.onValueChanged.AddListener(ToggleValue);
        }

        private void OnDestroy()
        {
            _toggle.onValueChanged.RemoveListener(ToggleValue);
        }

        public void ToggleValue(bool bValue)
        {
            TTCEvents.UsingAlternativeMarkToggled usingAlternativeMarkEvent = GameEventService.GetEvent<TTCEvents.UsingAlternativeMarkToggled>();
            usingAlternativeMarkEvent.PlayerData = _playerData;
            usingAlternativeMarkEvent.UsingAlternativeMark = bValue;
            GameEventService.TriggerEvent(usingAlternativeMarkEvent);
        }
    }
}
