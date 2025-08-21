using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMS
{
    /// <summary>
    /// Transform extension methods
    /// Avoids code duplication
    /// </summary>
    public static class TransformExtension
    {
        /// <summary>
        /// Applies world position values to the transform with the given TransformData
        /// </summary>
        /// <param name="transform">The transform to which the TransformData values will be applied.</param>
        /// <param name="transformData">The TransformData containing the position, rotation, and scale values to be applied.</param>
        public static void ApplyTransformData(this Transform transform, TransformData transformData)
        {
            transform.position = transformData.Position;
            transform.rotation = transformData.Rotation;
            transform.localScale = transformData.Scale;
        }

        /// <summary>
        /// Gets the topmost parent of the given transform.
        /// </summary>
        /// <param name="transform">The transform to start from.</param>
        /// <returns>The topmost parent transform.</returns>
        public static Transform GetTopmostParent(this Transform transform)
        {
            if (transform == null)
            {
                return null;
            }

            Transform currentTransform = transform;

            // Traverse up the hierarchy until we reach the topmost parent
            while (currentTransform.parent != null)
            {
                currentTransform = currentTransform.parent;
            }

            return currentTransform;
        }

        /// <summary>
        /// Searches for a parent transform by name.
        /// </summary>
        /// <param name="transform">The transform to start from.</param>
        /// <param name="parentName">The name of the parent transform to search for.</param>
        /// <returns>The parent transform with the specified name, or null if not found.</returns>
        public static Transform GetParentByName(this Transform transform, string parentName)
        {
            if (transform == null || string.IsNullOrEmpty(parentName))
            {
                return null;
            }

            Transform currentTransform = transform.parent;

            // Traverse up the hierarchy until we find a parent with the specified name or reach the top
            while (currentTransform != null)
            {
                if (currentTransform.name == parentName)
                {
                    return currentTransform;
                }

                currentTransform = currentTransform.parent;
            }

            // Return null if no parent with the specified name is found
            return null;
        }
    }
}
