using TMPro;
using UnityEngine;

namespace GMS
{
    /// <summary>
    /// Helper View side class to serve as component for displaying text.
    /// </summary>
    public class UITextItem : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField]
        private TextMeshProUGUI m_TextMeshPro;
        private string m_Text;

        // sizer tracked in order to preserve size
        private Vector2 _startingSizeDelta;
        private RectTransform _rectTransform;

        /// <summary>
        /// Sets text to a TextMeshProUGUI, for visualizing
        /// </summary>
        /// <param name="currentText"></param>
        /// <param name="usePreferredHeight">when true, this will acomodate the height to fit the text's preferred height.</param>
        public void SetText(string currentText, bool usePreferredHeight = true)
        {
            _rectTransform = (RectTransform)transform;
            Vector2 textSize = m_TextMeshPro.rectTransform.sizeDelta;
            _startingSizeDelta = _rectTransform.sizeDelta - textSize;

            m_Text = currentText;
            m_TextMeshPro.text = m_Text;

            if (usePreferredHeight)
            {
                AccomodateTextPreferredHeight();
            }
        }

        private void AccomodateTextPreferredHeight()
        {
            Vector2 newSize = _rectTransform.sizeDelta;
            newSize.y = _startingSizeDelta.y + m_TextMeshPro.preferredHeight;
            _rectTransform.sizeDelta = newSize;
        }
    }
}
