using UnityEngine;

namespace GMS
{
    /// <summary>
    /// View side class for handling all the view side logic for the instanced CollectibleItems
    /// - Used as intermediary for being able to get the interface when this instance has a collider.
    /// </summary>
    public class CollectibleItem : MonoBehaviour, IPickUp
    {
        public bool HasBeenPickedUp => _isPickedUp;
        private bool _isPickedUp;

        private CollectibleSO _data;
        [Header("Components")]
        [SerializeField, Tooltip("Set the parent gameobject of all the rendering elements for this instance.")]
        private GameObject renderering;
        private float disableTimer = 0;

        public void Initialize(CollectibleSO data, TransformData transformData)
        {
            _data = data;
            transform.ApplyTransformData(transformData);
            _isPickedUp = false;

            // Ensure the renderer is enabled when the item is initialized
            renderering.SetActive(true);
        }

        public void PickUp()
        {
            if (_isPickedUp)
            {
                return;
            }
            SetToDormant();
        }

        public void SetToDormant()
        {
            _isPickedUp = true;

            if (GameInstance.TryGetSubManager(out TimeManager timeManager))
            {
                timeManager.AddCooldown(disableTimer, DisableRendering);
            }
            else
            {
                DisableRendering();
            }
        }

        private void DisableRendering()
        {
            renderering.SetActive(false);
        }

        public CollectibleSO GetCollectibleData()
        {
            return _data;
        }
    }
}
