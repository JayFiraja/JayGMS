using UnityEngine;
namespace GMS.Samples
{
    [System.Serializable]
    [LinkDataLogic(typeof(Minimal_UIData), dataDisplayName: "Minimal_UIData", typeof(Minimal_UIManager), displayName: "Minimal_UIManager")]
    public struct Minimal_UIData : ISubManagerData
    {
        [AddressableSelector]
        public string UIBasicCollectionAddress;

        // Assigned in-game when loading the addressable key
        [HideInInspector]
        public UICollectionSO UICollectionSO;

        [Header("Prefabs")]
        public UITextItem BulletPointPanel;
        public ToggleUIContentFitter ToggleUIContentFitter;

        public MinimalInfoItems[] MinimalInfoItems;
        public float PopulateItemsRate;

        [Tooltip("The collectible holder needs a Layout group for being able to display multiple items.")]
        public Transform CollectiblesHolder;
        [Tooltip("The collectible item that shows up when collectibles are picked up.")]
        public UICollectible UICollectibleItem;
        public float CollectibleItemsFadeSpeed;
        public AnimationCurve CollectibleItemsFadeRate;
        public float CollectibleItemsFadeOutTimer;
    }

    [System.Serializable]
    public struct MinimalInfoItems
    {
        [TextArea]
        /// <summary>
        /// text to be dispaled in the toggle per subject
        /// </summary>
        public string ToggleSubjectText;
        [TextArea]
        /// <summary>
        /// text to be displayed as collapsable items under each toggle subject
        /// </summary>
        public string[] contentItems;
    }

    /// <summary>
    /// Wrapper for associating UI collected values with their visibility state and value
    /// </summary>
    public class UICollectedItems
    {
        public CollectibleSO CollectibleSO;
        public UICollectible ViewItem;
        public CanvasGroupTransition CanvasGroupTransition;
        public CanvasGroupTransitionTask CanvasGroupTransitionTask;
        public CooldownDelegate<int> FadeoutCooldown;
    }
}
