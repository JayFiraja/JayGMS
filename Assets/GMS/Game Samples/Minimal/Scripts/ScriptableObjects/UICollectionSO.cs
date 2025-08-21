using UnityEngine;
using UnityEngine.EventSystems;

namespace GMS.Samples
{
    /// <summary>
    /// Scriptable object for holding base data for UI SubManagers
    /// </summary>
    [CreateAssetMenu(fileName = "newUIBase", menuName = "GMS Samples/Data Collections/UI Base")]
    public class UICollectionSO : ScriptableObject
    {
        [Header("Prefabs")]
        /// <summary>
        /// MainCanvas prefab with direct Canvas assignment
        /// </summary>
        public Canvas MainCanvas;
        /// <summary>
        /// Window prefab with direct class assignment
        /// </summary>
        public Window Window;
        /// <summary>
        /// Necessary for capturing input interactions in the View Side
        /// </summary>
        public EventSystem EventSystem;

        // More Basic UI elements to be added...
    }
}
