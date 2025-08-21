using UnityEngine;

namespace GMS
{
    /// <summary>
    /// Struct to define rotation ranges for each axis.
    /// </summary>
    [System.Serializable]
    public struct RotationRange
    {
        public Vector2 XRange;
        public Vector2 YRange;
        public Vector2 ZRange;

        /// <summary>
        /// Initializes a new instance of the RotationRange struct with specific ranges for each axis.
        /// </summary>
        /// <param name="xRange">The range for the X-axis rotation.</param>
        /// <param name="yRange">The range for the Y-axis rotation.</param>
        /// <param name="zRange">The range for the Z-axis rotation.</param>
        public RotationRange(Vector2 xRange, Vector2 yRange, Vector2 zRange)
        {
            XRange = xRange;
            YRange = yRange;
            ZRange = zRange;
        }
    }
}
