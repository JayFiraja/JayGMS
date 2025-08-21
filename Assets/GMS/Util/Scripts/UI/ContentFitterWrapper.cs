using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GMS
{
    /// <summary>
    /// Wrapper class that ensures the content has both a canvas group and a layout element.
    /// This allows us to avoid having to disable gameobjects to handle visibility, and to get properties like Content Height
    /// </summary>
    [RequireComponent(typeof(LayoutElement))]
    [RequireComponent(typeof(CanvasGroup))]
    public class ContentFitterWrapper : MonoBehaviour
    {
        // Members
        [Header("Spacing from optional layout Group"), 
         Tooltip("Edit ONLY if there is a layout group with spacing, and set the same value from it."),
         SerializeField]
        private float spacing = 0;

        // Properties
        public LayoutElement ContentLayoutElement => _contentLayoutElement;
        public CanvasGroup ContentCanvasGroup => _contentCanvasGroup;
        public float Spacing => spacing;

        private ViewContent<UITextItem> _textItems;
        private const string poolCateogry = "descriptionItems";

        // variables
        private RectTransform _contentRectTransform;
        private LayoutElement _contentLayoutElement;
        private CanvasGroup _contentCanvasGroup;

        public void Initialize()
        {
            _contentRectTransform = transform as RectTransform;
            TryGetComponent(out _contentLayoutElement);
            TryGetComponent(out _contentCanvasGroup);
            _textItems = new ViewContent<UITextItem>(parent:transform);
        }

        /// <summary>
        /// Returns this rect transform size delta.
        /// </summary>
        public Vector2 GetSizeDelta()
        {
            return _contentRectTransform.sizeDelta;
        }

        /// <summary>
        /// Adds new item, ensure it has a text component in it's children
        /// </summary>
        /// <param name="prefab">prefab to instance and parent under  <see cref="_contentRectTransform"/>></param>
        /// <param name="text">text to set in it's</param>
        public void AddTextItem(UITextItem prefab, string text)
        {
            UITextItem newItem = _textItems.GetOrCreate(prefab, category: poolCateogry);
            newItem.SetText(text);
        }
    }
}
