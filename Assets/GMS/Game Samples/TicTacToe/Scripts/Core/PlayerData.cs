using UnityEngine;

namespace GMS.Samples
{
    /// <summary>
    /// Contains the data for players, in order to add customization options.
    /// </summary>
    [System.Serializable]
    public struct PlayerData
    {
        /// <summary>
        /// The player turn this data represents
        /// </summary>
        public PlayerTurn PlayerTurn;
        /// <summary>
        /// Color used for Player's Markers on the grid.
        /// </summary>
        public Color Color;
        /// <summary>
        /// Default Prefab
        /// </summary>
        public GameObject DefaultMarkPrefab;
        /// <summary>
        /// Alternative Mark Prefab, Unlockable on Player's first win.
        /// Selectable on the Main Menu Screen.
        /// </summary>
        public GameObject AlternativeMarkPrefab;
        /// <summary>
        /// When true spawn AlternativeMarkPrefab upon Mark instance
        /// </summary>
        public bool usingAlternativeMark;
    }
}

