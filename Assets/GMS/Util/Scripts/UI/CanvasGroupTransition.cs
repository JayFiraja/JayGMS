using UnityEngine;

namespace GMS
{
    [System.Serializable]
    public class CanvasGroupTransition
    {
        public CanvasGroup CanvasGroup;

        /// <summary>
        /// This is set when added on a queue for processing transitions
        /// </summary>
        public float transitionAlphaTarget;

        public bool IsCanvasGroupVisible()
        {
            return CanvasGroup.alpha > 0;
        }
        public float GetCanvasAlpha()
        {
            return CanvasGroup.alpha;
        }
        public void SetCanvasAlpha(float newAlpha)
        {
            CanvasGroup.alpha = newAlpha;
        }

        /// <summary>
        /// We ensure that invisible or almost invisible canvases never block Raycasting and
        /// are not marked as interactive
        /// </summary>
        public void UpdateCanvasInteractivity()
        {
            CanvasGroup.UpdateCanvasInteractivity();
        }
    }
}
