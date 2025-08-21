using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GMS
{
    /// <summary>
    /// Updates the size of the scroll view depending on the ContentFitterWrappers
    /// </summary>
    public class ToggleUIContentFitter : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField]
        private Toggle toggle;
        [SerializeField]
        private TextMeshProUGUI textMeshPro;

        [Header("Parameters")]
        [SerializeField, Tooltip("Set here the base width and height for this element")]
        private Vector2 baseSize;
        [SerializeField, Tooltip("When true this element will acomodate the total height from the content")]
        private bool fitHeight = true;
        [SerializeField, Tooltip("When true this element will acomodate the total width from the content")]
        private bool fitWidth = false;
        [SerializeField, Tooltip("(OPTIONAL) Set these manually or search for them automatically if getContentFitterWrappersFromChildren is true")]
        private ContentFitterWrapper contentFitterWrapper;

        private RectTransform _rectTransform;
        private RectTransform _scrollViewRectTransform;

        public void Initialize(RectTransform scrollViewRectTransform)
        {
            contentFitterWrapper.Initialize();
            _scrollViewRectTransform = scrollViewRectTransform;
            _rectTransform = transform as RectTransform;
            toggle.isOn = false;
            toggle.onValueChanged.AddListener(OnToggled);
            UpdateSize(false);
        }

        private void OnToggled(bool bValue)
        {
            UpdateSize(bValue);
        }

        private void UpdateSize(bool setContentVisible)
        {
            Vector2 contentSize = Vector2.zero;

            contentFitterWrapper.ContentLayoutElement.ignoreLayout = !setContentVisible;
            contentFitterWrapper.ContentCanvasGroup.ToggleCanvasGroup(setContentVisible);

            contentSize += contentFitterWrapper.GetSizeDelta();
            contentSize.y += contentFitterWrapper.Spacing;
            
            Vector2 newPreferedSize = baseSize;
            if (fitWidth)
            {
                newPreferedSize.x += setContentVisible ? contentSize.y : 0;
            }
            if (fitHeight) 
            {
                newPreferedSize.y += setContentVisible ? contentSize.y : 0;
            }

            _rectTransform.sizeDelta = newPreferedSize;

            // Force the layout to update
            LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollViewRectTransform);
        }

        /// <summary>
        ///  Set the text for this item's Text mesh pro component
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string text)
        {
            textMeshPro.text = text;
        }

        public void AddContentTextItem(UITextItem prefab, string text)
        {
            contentFitterWrapper.AddTextItem(prefab, text);
        }
    }
}