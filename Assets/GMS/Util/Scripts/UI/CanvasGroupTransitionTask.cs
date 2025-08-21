using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMS
{
    /// <summary>
    /// Helper class utilized for making canvas group transitions to a target alpha value.
    /// </summary>
    public class CanvasGroupTransitionTask
    {
        private CanvasGroupTransition _canvasGroupTransition;
        private float _targetAlpha;
        private float _fadeSpeed;

        public bool IsComplete { get; private set; }

        public CanvasGroupTransitionTask(CanvasGroupTransition canvasGroupTransition, float targetAlpha, float fadeSpeed)
        {
            this._canvasGroupTransition = canvasGroupTransition;
            this._targetAlpha = targetAlpha;
            this._fadeSpeed = fadeSpeed;
            this.IsComplete = false;
        }

        public void Update()
        {
            if (IsComplete)
                return;

            float currentAlpha = _canvasGroupTransition.GetCanvasAlpha();
            float newAlpha = Mathf.MoveTowards(currentAlpha, _targetAlpha, _fadeSpeed * Time.deltaTime);

            _canvasGroupTransition.SetCanvasAlpha(newAlpha);

            if (Mathf.Approximately(newAlpha, _targetAlpha))
            {
                _canvasGroupTransition.SetCanvasAlpha(_targetAlpha);
                _canvasGroupTransition.UpdateCanvasInteractivity();
                IsComplete = true;
            }
        }
    }
}
