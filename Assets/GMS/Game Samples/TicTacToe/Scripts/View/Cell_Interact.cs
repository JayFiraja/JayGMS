using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GMS.Samples
{
    [RequireComponent(typeof(BoxCollider))]
    public class Cell_Interact : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        private CellData _cellData;
        private TicTacToeStateManager _stateManager;

        [SerializeField, Tooltip("Monobehaviours implementing the interface IAction, and that act as Spawn Effects")]
        private MonoBehaviour[] SpawnActionRefs = Array.Empty<MonoBehaviour>();
        private List<IAction> _spawnActions;

        [SerializeField, Tooltip("Monobehaviours implementing the interface IAction, and that act as Highlight Effects")]
        private MonoBehaviour[] HighlightActionRefs = Array.Empty<MonoBehaviour>();
        private List<IAction> _highlightActions;

        [SerializeField, Tooltip("Monobehaviours implementing the interface IAction, and that act as Winning Effects")]
        private MonoBehaviour[] WinningCellActionRefs = Array.Empty<MonoBehaviour>();
        private List<IAction> _winningCellActions;

        private BoxCollider _boxCollider;
        private bool _isHovered = false;

        public void Initialize(CellData cellData)
        {
            _cellData = cellData;
            TryGetComponent(out _boxCollider);
            _boxCollider.center = _cellData.BoxColliderCenter;
            _boxCollider.size = _cellData.BoxColliderSize;

            GameInstance.TryGetSubManager(out _stateManager);
            
            GameEventService.RegisterListener<TTCEvents.PlayAICell>(TryPlayAI);
            GameEventService.RegisterListener<TTCEvents.PlayerTurnChanged>(OnPlayerTurnChanged);

            RefreshActionLists();
        }

        private void OnDestroy()
        {
            GameEventService.UnregisterListener<TTCEvents.PlayAICell>(TryPlayAI);
            GameEventService.UnregisterListener<TTCEvents.PlayerTurnChanged>(OnPlayerTurnChanged);
        }

        private void TryPlayAI(TTCEvents.PlayAICell playAICell)
        {
            if (!_cellData.IsMarked && _cellData.Coords == playAICell.Coords)
            {
                PlayMarker();
            }
        }

        public void SpawnActions()
        {
            IActionUtil.RunActions(_spawnActions, callDoAction: true);
        }

        public void DeSpawnActions()
        {
            _cellData.IsMarked = false;
            IActionUtil.RunActions(_spawnActions, callDoAction: false);
            IActionUtil.RunActions(_highlightActions, callDoAction: false);
            IActionUtil.RunActions(_winningCellActions, callDoAction: false);
        }

        private void PlayOnHoverEnterActions()
        {
            IActionUtil.RunActions(_highlightActions, callDoAction: true);
        }

        private void PlayOnHoveredExitActions()
        {
            IActionUtil.RunActions(_highlightActions, callDoAction: false);
        }

        private void CheckWinningCell()
        {
            IActionUtil.RunActions(_winningCellActions, callDoAction: true);
        }

        private bool CanInteract()
        {
            if (_cellData.IsMarked)
            {
                return false;
            }
            if (!_stateManager.CanPlayCell)
            {
                return false;
            }
            if (_stateManager.IsAIPlaying && _stateManager.PlayerTurn == PlayerTurn.PlayerB)
            {
                return false;
            }

            return true;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovered = true;
            if (!CanInteract())
            {
                return;
            }

            PlayOnHoverEnterActions();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovered = false;

            if (!CanInteract())
            {
                return;
            }

            PlayOnHoveredExitActions();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }
            if (!CanInteract())
            {
                return;
            }

            PlayMarker();
        }

        private void PlayMarker()
        {
            _cellData.IsMarked = true;
            PlayOnHoveredExitActions();
            TTCEvents.CellDataSelected cellSelectedEvent = GameEventService.GetEvent<TTCEvents.CellDataSelected>();
            cellSelectedEvent.SelectedCellData = _cellData;
            GameEventService.TriggerEvent(cellSelectedEvent);
        }

        private void OnPlayerTurnChanged(TTCEvents.PlayerTurnChanged playerTurnChanged)
        {
            if (_isHovered && CanInteract())
            {
                PlayOnHoverEnterActions();
            }
        }

        private void RefreshActionLists()
        {
            _spawnActions = ConvertToIActionList(SpawnActionRefs);
            _highlightActions = ConvertToIActionList(HighlightActionRefs);
            _winningCellActions = ConvertToIActionList(WinningCellActionRefs);
        }

        private List<IAction> ConvertToIActionList(MonoBehaviour[] monoBehaviours)
        {
            List<IAction> actions = new List<IAction>();
            foreach (var mb in monoBehaviours)
            {
                if (mb is IAction action)
                {
                    actions.Add(action);
                }
            }
            return actions;
        }
    }
}
