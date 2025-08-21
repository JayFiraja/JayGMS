using UnityEngine;

namespace GMS
{
    /// <summary>
    /// Provides utility methods for creating and manipulating Quaternions.
    /// </summary>
    public static class QuaternionUtilities
    {
        /// <summary>
        /// Creates a new Quaternion with random rotations on specified axes within the given range.
        /// </summary>
        /// <param name="randomizeX">If true, randomizes rotation around the X-axis.</param>
        /// <param name="randomizeY">If true, randomizes rotation around the Y-axis.</param>
        /// <param name="randomizeZ">If true, randomizes rotation around the Z-axis.</param>
        /// <param name="rotationRange">The rotation ranges for each axis.</param>
        /// <returns>A new Quaternion with randomized rotations on specified axes.</returns>
        public static Quaternion CreateRandomQuaternion(bool randomizeX, bool randomizeY, bool randomizeZ, RotationRange rotationRange)
        {
            float xRotation = randomizeX ? Random.Range(rotationRange.XRange.x, rotationRange.XRange.y) : 0f;
            float yRotation = randomizeY ? Random.Range(rotationRange.YRange.x, rotationRange.YRange.y) : 0f;
            float zRotation = randomizeZ ? Random.Range(rotationRange.ZRange.x, rotationRange.ZRange.y) : 0f;

            return Quaternion.Euler(xRotation, yRotation, zRotation);
        }
    }
}
