using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GMS
{
    /// <summary>
    /// Base window class for adding expandable window based on content.
    /// </summary>
    public class Window : MonoBehaviour
    {
        public Vector3 AnchoredPosition { get; private set; }

        [Header("Components")]
        public RectTransform headerScrollRect;
        public RectTransform footerScrollRect;
        public ScrollRect contentScrollRect;

        [Header("Parameters"), SerializeField]
        private Vector2 windowMinSize;
        [SerializeField]
        private Vector2 windowMaxSize;
        [SerializeField]
        private bool expandWidth;
        [SerializeField]
        private bool expandHeight;
        [SerializeField, Tooltip("Takes into account extra spacings")]
        private Vector2 expandOffset;

        private RectTransform _contentRectTransform;
        private RectTransform _rectTransform;

        public bool IsHidden = false;

        public void Initialize()
        {
            _rectTransform = transform as RectTransform;
            AnchoredPosition = _rectTransform.anchoredPosition;
            if (contentScrollRect == null)
            {
                Debug.LogError("Needs ScrollRect to function.");
                return;
            }
            _contentRectTransform = contentScrollRect.content.transform as RectTransform;
            contentScrollRect.onValueChanged.AddListener(OnScrollValueChanged);

            ResizeWindow();
        }

        private void OnScrollValueChanged(Vector2 newSize)
        {
            ResizeWindow();
        }

        private void ResizeWindow()
        {
            Vector2 newSize = Vector2.zero;
            if (headerScrollRect != null)
            {
                newSize += headerScrollRect.sizeDelta;
            }

            if (footerScrollRect != null)
            {
                newSize += footerScrollRect.sizeDelta;
            }

            newSize += _contentRectTransform.sizeDelta;
            newSize += expandOffset;

            // clamp the values before setting them.
            newSize.x = Mathf.Clamp(newSize.x, windowMinSize.x, windowMaxSize.x);
            newSize.y = Mathf.Clamp(newSize.y, windowMinSize.y, windowMaxSize.y);
            if (!expandWidth) 
            {
                newSize.x = _rectTransform.sizeDelta.x;
            }
            if (!expandHeight)
            {
                newSize.y = _rectTransform.sizeDelta.y;
            }
            _rectTransform.sizeDelta = newSize;
        }

        public void SetAnchoredPosition(Vector3 newPosition)
        {
            _rectTransform.anchoredPosition = newPosition;
            AnchoredPosition = newPosition;
        }

        public RectTransform GetContentRectTransform()
        {
            return contentScrollRect.content;
        }
    }
}
