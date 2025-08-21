using UnityEngine;
using UnityEngine.UI;

namespace GMS
{
    /// <summary>
    /// Canvas Group component Extension class with common and useful methods 
    /// </summary>
    public static class CanvasGroupExtension
    {
        /// <summary>
        /// Toggle the Canvas groups alpha, interactable and blockRaycasts values.
        /// </summary>
        /// <param name="canvasGroup"> the canvas group calling the extension method</param>
        /// <param name="bValue">determines the following: 
        /// - alpha for transparency 0 if false, 1 if true
        /// - interactable
        /// - blocks raycasts 
        ///             </param>
        public static void ToggleCanvasGroup(this CanvasGroup canvasGroup, bool bValue)
        {
            canvasGroup.alpha = bValue ? 1 : 0;
            canvasGroup.interactable = bValue ? true : false;
            canvasGroup.blocksRaycasts = bValue ? true : false;
        }

        /// <summary>
        /// Toggle the Canvas groups alpha
        /// </summary>
        /// <param name="canvasGroup"> the canvas group calling the extension method</param>
        /// <param name="bValue">determines the following: 
        /// - alpha for transparency 0 if false, 1 if true
        ///             </param>
        public static void ToggleCanvasGroupAlpha(this CanvasGroup canvasGroup, bool bValue)
        {
            canvasGroup.alpha = bValue ? 1 : 0;
        }

        /// <summary>
        /// Sets the interactable and blocksRaycasts properties of the CanvasGroup.
        /// </summary>
        /// <param name="canvasGroup">The CanvasGroup calling the extension method</param>
        /// <param name="interactable">Whether the CanvasGroup should be interactable</param>
        /// <param name="blocksRaycasts">Whether the CanvasGroup should block raycasts</param>
        public static void SetInteractableAndRaycasts(this CanvasGroup canvasGroup, bool interactable, bool blocksRaycasts)
        {
            canvasGroup.interactable = interactable;
            canvasGroup.blocksRaycasts = blocksRaycasts;
        }

        /// <summary>
        /// We ensure that invisible or almost invisible canvases never block Raycasting and
        /// are not marked as interactive
        /// </summary>
        public static void UpdateCanvasInteractivity(this CanvasGroup canvasGroup)
        {
            bool allowsInteractivity = canvasGroup.alpha > 0.1f;
            canvasGroup.blocksRaycasts = allowsInteractivity;
            canvasGroup.interactable = allowsInteractivity;
        }
    }
}
