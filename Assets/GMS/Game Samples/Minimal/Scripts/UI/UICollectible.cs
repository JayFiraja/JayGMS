using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GMS.Samples
{
    /// <summary>
    /// View side class for handling the UI collectible visualizations
    /// this needs the components assigned in the inspector, stored in the prefab.
    /// </summary>
    [RequireComponent(typeof(LayoutElement))]
    [RequireComponent(typeof(CanvasGroup))]
    public class UICollectible : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField]
        private TextMeshProUGUI _valueText;
        [SerializeField]
        private Image _iconImage;
        private CanvasGroup _canvasGroup;
        private float _elapsedTime = 0f;

        public CanvasGroup CanvasGroup => _canvasGroup;

        /// <summary>
        /// Initialize the component and feed the associated icon's sprite and start value to display.
        /// </summary>
        /// <param name="iconSprite"></param>
        /// <param name="startValue"></param>
        public void Initialize(Sprite iconSprite, string startValue)
        {
            TryGetComponent(out _canvasGroup);
            _canvasGroup.ToggleCanvasGroup(false);
            _iconImage.sprite = iconSprite;
            UpdateValueText(startValue);
        }

        /// <summary>
        /// Transitions to the new canvas visibility.
        /// </summary>
        /// <param name="targetAlpha"></param>
        /// <param name="fadeSpeed"></param>
        /// <param name="fadeRate"></param>
        /// <returns>True if the transition has completed.</returns>
        public bool TransitionedCanvasToVisibility(float targetAlpha, float fadeSpeed, AnimationCurve fadeRate)
        {
            float startAlpha = _canvasGroup.alpha;
            if (Mathf.Approximately(startAlpha, targetAlpha))
            {
                return true;
            }
            _elapsedTime += Time.deltaTime;

            // Calculate the current fade rate using the animation curve
            float fadeMultiplier = fadeRate.Evaluate(_elapsedTime);

            // Move alpha towards the target alpha at a constant rate
            _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, targetAlpha, fadeSpeed * fadeMultiplier * Time.deltaTime);

            bool hasTransitioned = Mathf.Approximately(_canvasGroup.alpha, targetAlpha);
             if (hasTransitioned)
            {
                _canvasGroup.alpha = targetAlpha;
                _elapsedTime = 0;
            }

            return hasTransitioned;
        }

        /// <summary>
        /// Updates the TextMeshPro text
        /// </summary>
        public void UpdateValueText(string text)
        {
            _valueText.text = text;
        }
    }
}
