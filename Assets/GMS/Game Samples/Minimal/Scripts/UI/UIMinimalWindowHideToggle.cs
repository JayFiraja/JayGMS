using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GMS.Samples
{
    /// <summary>
    /// Simple view class for handling a window hidding toggle
    /// </summary>
    public class UIMinimalWindowHideToggle : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField]
        private Toggle toggle;
        [SerializeField]
        private Window window;
        [SerializeField]
        private UnityEvent onValueTrue;
        [SerializeField]
        private UnityEvent onValueFalse;

        [Header("Parameters")]
        [SerializeField]
        private Vector3 HiddenLocalPosition;
        private Vector3 _startingLocalPosition;

        private void Start()
        {
            _startingLocalPosition = window.AnchoredPosition;
            OnToggle(window.IsHidden);
            toggle.onValueChanged.AddListener(OnToggle);
        }

        private void OnDestroy()
        {
            toggle.onValueChanged.RemoveListener(OnToggle);
        }

        private void OnToggle(bool bValue)
        {
            Vector3 newLocalPosition = bValue ? HiddenLocalPosition : _startingLocalPosition;

            if (bValue)
            {
                onValueTrue.Invoke();
            }
            else
            {
                onValueFalse.Invoke();
            }

            window.SetAnchoredPosition(newLocalPosition);
            window.IsHidden = bValue;
        }
    }
}
