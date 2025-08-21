using System.Collections.Generic;
using UnityEngine;

namespace GMS.Samples
{
    public class UIMainCanvas : MonoBehaviour
    {
        [Header("Set Children CanvasGroups"), SerializeField]
        private List<StateCanvasGroup> StateCanvasGroups = new List<StateCanvasGroup>();
        [SerializeField]
        private float fadeSpeed = 3f;

        private Queue<StateCanvasGroup> _canvasesToTransition = new Queue<StateCanvasGroup>();
        private List<CanvasGroupTransitionTask> _activeTransitions = new List<CanvasGroupTransitionTask>();

        public void Initialize(GameState startingState = GameState.UnInitialized)
        {
            SetAllCanvasAlpha(0);
            GameStateChanged(GameState.UnInitialized, startingState);
        }

        public void GameStateChanged(GameState fromState, GameState toState)
        {
            _canvasesToTransition.Clear();
            StateCanvasGroup nextVisibleCanvas = new StateCanvasGroup();
            bool nextStateFound = false;

            for (int i = 0; i < StateCanvasGroups.Count; i++)
            {
                StateCanvasGroup stateCanvasGroup = StateCanvasGroups[i];
                bool otherStateAndVisible = stateCanvasGroup.GameState != toState && stateCanvasGroup.GetCanvasAlpha() > 0;
                bool sameAsToState = stateCanvasGroup.GameState == toState;

                if (otherStateAndVisible)
                {
                    stateCanvasGroup.transitionAlphaTarget = 0;
                    _canvasesToTransition.Enqueue(stateCanvasGroup);
                }

                if (!nextStateFound && sameAsToState)
                {
                    nextVisibleCanvas = stateCanvasGroup;
                    nextVisibleCanvas.transitionAlphaTarget = 1;
                    nextStateFound = true;
                }
            }

            if (nextStateFound)
            {
                _canvasesToTransition.Enqueue(nextVisibleCanvas);
            }

            // Process the pending transitions immediately
            ProcessPendingTransitions();
        }

        public void OnUpdate()
        {
            // Update active transitions
            for (int i = _activeTransitions.Count - 1; i >= 0; i--)
            {
                _activeTransitions[i].Update();
                if (_activeTransitions[i].IsComplete)
                {
                    _activeTransitions.RemoveAt(i);
                }
            }
        }

        private void ProcessPendingTransitions()
        {
            while (_canvasesToTransition.Count > 0)
            {
                StateCanvasGroup stateCanvasGroup = _canvasesToTransition.Dequeue();
                float targetAlpha = stateCanvasGroup.transitionAlphaTarget;
                CanvasGroupTransitionTask task = new CanvasGroupTransitionTask(stateCanvasGroup, targetAlpha, fadeSpeed);
                _activeTransitions.Add(task);
            }
        }

        private void SetAllCanvasAlpha(float alpha)
        {
            foreach (StateCanvasGroup stateCanvasGroup in StateCanvasGroups)
            {
                stateCanvasGroup.SetCanvasAlpha(alpha);
                stateCanvasGroup.UpdateCanvasInteractivity();
            }
        }
    }
}
