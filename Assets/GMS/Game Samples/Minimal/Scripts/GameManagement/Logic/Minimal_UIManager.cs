using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace GMS.Samples
{
    /// <summary>
    /// Handles the Instancing and management of the UI elements for the Minimal Game sample for GMS
    /// </summary>
    public class Minimal_UIManager: ISubManager
    {
        private Minimal_UIData _data;
        private Canvas _mainCanvas;
        private Window _infoWindow;
        private TimeManager _timeManager;
        private EventSystem _eventSystem;

        private List<UICollectedItems> _uICollectedItems;
        public Transform _collectiblesHolder;
        public LayoutGroup _collectiblesContent;

        public Minimal_UIManager(Minimal_UIData data)
        {
            _data = data;
        }
    
        public bool Initialize(GMS.GameManager gameManager)
        {
            gameManager.TryGetSubManager(out _timeManager);
            _uICollectedItems = new List<UICollectedItems>();
            Addressables.LoadAssetAsync<UICollectionSO>(_data.UIBasicCollectionAddress).Completed += OnUICollectionLoaded;
            return true;
        }

        public void UnInitialize()
        {
            if (_data.UICollectionSO != null)
            {
                Addressables.Release(_data.UICollectionSO);
            }
        }
    
        public void OnUpdate()
        {
            UpdateCollectibleItemsVisibility();
        }

        public bool Equals(ISubManager other)
        {
            // Check if other is null
            if (other == null)
            {
                return false;
            }

            // Compare the runtime types of the current instance and the other instance
            return GetType() == other.GetType();
        }

        public override bool Equals(object obj)
        {
            if (obj is ISubManager other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            // Generate a hash code based on the fields that contribute to equality
            return GetType().GetHashCode();
        }

        private void OnUICollectionLoaded(AsyncOperationHandle<UICollectionSO> obj)
        {
            if (obj.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to instantiate ScriptableObject with address{_data.UIBasicCollectionAddress}.");
                return;
            }

            _data.UICollectionSO = obj.Result;

            _eventSystem = EventSystem.Instantiate(_data.UICollectionSO.EventSystem);
            _mainCanvas = Canvas.Instantiate(_data.UICollectionSO.MainCanvas);
            _infoWindow = Window.Instantiate(_data.UICollectionSO.Window, _mainCanvas.transform);
            _infoWindow.Initialize();

            for (int i = 0; i < _data.MinimalInfoItems.Length; i++)
            {
                int index = i;
                float cooldown = index * _data.PopulateItemsRate;
                _timeManager?.AddCooldown(cooldown, PopulateInfoWindowItems, index);
            }

            _collectiblesHolder = Transform.Instantiate(_data.CollectiblesHolder, _mainCanvas.transform);
            _collectiblesContent = _collectiblesHolder.GetComponentInChildren<LayoutGroup>();
            if(_collectiblesContent == null)
            {
                Debug.LogError("Failed to get the LayoutGroup under Collectibles Holder");
            }
        }

        private void PopulateInfoWindowItems(int index)
        {
            MinimalInfoItems nextInfoItems = _data.MinimalInfoItems[index];
            ToggleUIContentFitter subject = ToggleUIContentFitter.Instantiate(_data.ToggleUIContentFitter, _infoWindow.GetContentRectTransform());

            subject.Initialize(_infoWindow.GetContentRectTransform());
            subject.SetText(nextInfoItems.ToggleSubjectText);

            foreach (string item in nextInfoItems.contentItems)
            {
                subject.AddContentTextItem(_data.BulletPointPanel, item);
            }
        }

        // Collected items Methods
        private void UpdateCollectibleItemsVisibility()
        {
            for (int i = _uICollectedItems.Count - 1; i >= 0; i--)
            {
                UICollectedItems currentItem = _uICollectedItems[i];

                if (currentItem.CanvasGroupTransitionTask == null)
                {
                    continue;
                }

                if (currentItem.CanvasGroupTransitionTask.IsComplete)
                {
                    if (currentItem.CanvasGroupTransition.transitionAlphaTarget == 1)
                    {
                        currentItem.CanvasGroupTransitionTask = null;
                        int index = i;
                        currentItem.FadeoutCooldown = _timeManager?.AddCooldown(_data.CollectibleItemsFadeOutTimer, SetCollectibleNewTransitionToFadeOut, index);
                    }
                    else
                    {
                        currentItem.CanvasGroupTransitionTask = null;
                        continue;
                    }
                }
                currentItem.CanvasGroupTransitionTask?.Update();
            }
        }

        private void SetCollectibleNewTransitionToFadeOut(int itemIndex)
        {
            if (itemIndex >= _uICollectedItems.Count)
            {
                Debug.Log("Index out of Range");
                return;
            }
            UICollectedItems currentItem = _uICollectedItems[itemIndex];
            currentItem.CanvasGroupTransition.transitionAlphaTarget = 0;
            currentItem.CanvasGroupTransitionTask = new CanvasGroupTransitionTask(currentItem.CanvasGroupTransition, 0, _data.CollectibleItemsFadeSpeed);
        }

        /// <summary>
        /// Displays the total collacted value for the specified collecti
        /// </summary>
        /// <param name="collectibleSO">data</param>
        /// <param name="newValue">total collected</param>
        public void DisplayCollectedItemNewValue(CollectibleSO collectibleSO, int newValue)
        {
            if (!TryGetOrCreateCollectibleItem(collectibleSO, out UICollectedItems uICollectedItem))
            {
                return;
            }

            uICollectedItem.CanvasGroupTransition.transitionAlphaTarget = 1;
            _timeManager?.StopCooldownDelegate(uICollectedItem.FadeoutCooldown);
            uICollectedItem.CanvasGroupTransitionTask = new CanvasGroupTransitionTask(uICollectedItem.CanvasGroupTransition, 1, _data.CollectibleItemsFadeSpeed);
            uICollectedItem.ViewItem.UpdateValueText(newValue.ToString());
        }

        private bool TryGetOrCreateCollectibleItem(CollectibleSO collectibleSO, out UICollectedItems uICollectedItem)
        {
            uICollectedItem = default;
            bool success = false;
            foreach (UICollectedItems item in _uICollectedItems)
            {
                if (!string.Equals(item.CollectibleSO.CollectibleKey,collectibleSO.CollectibleKey))
                {
                    continue;
                }
                uICollectedItem = item;
                success = true;
                break;
            }

            if (!success)
            {
                UICollectible newCollectibleItem = UICollectible.Instantiate(_data.UICollectibleItem, _collectiblesContent.transform);
                newCollectibleItem.Initialize(collectibleSO.Icon, string.Empty);

                uICollectedItem = new UICollectedItems
                {
                    CollectibleSO = collectibleSO,
                    ViewItem = newCollectibleItem,
                    CanvasGroupTransition = new CanvasGroupTransition
                    {
                        CanvasGroup = newCollectibleItem.CanvasGroup
                    }
                };

                _uICollectedItems.Add(uICollectedItem);
                success = true;
            }

            return success;
        }
    }
}
