using UnityEngine;

namespace GMS.Samples
{
    [System.Serializable]
    /// <summary>
    /// Data class for allowing a cell to echo when interacting with it.
    /// </summary>
    public struct CellData
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CellData(Vector3 localPosition, Vector2 coords, bool isMarked,
                        Vector3 boxColliderCenter, Vector3 boxColliderSize)
        {
            LocalPosition = localPosition;
            Coords = coords;
            IsMarked = isMarked;
            BoxColliderCenter = boxColliderCenter;
            BoxColliderSize = boxColliderSize;
        }

        public Vector3 LocalPosition;
        /// <summary>
        /// X for Row and Y for Column
        /// </summary>
        public Vector2 Coords;
        /// <summary>
        /// Is already marked
        /// </summary>
        public bool IsMarked;
        /// <summary>
        /// Center offset
        /// </summary>
        public Vector3 BoxColliderCenter;
        /// <summary>
        /// Box Collider Size
        /// </summary>
        public Vector3 BoxColliderSize;

    }
}
