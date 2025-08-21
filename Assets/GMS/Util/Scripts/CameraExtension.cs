using UnityEngine;

namespace GMS
{
    public static class CameraExtension
    {
        /// <summary>
        /// Gets the normalized XZ plane relative directions from the given camera, i.e., the camera transform for use with input axis.
        /// </summary>
        /// <returns>Returns true if the camera was available so the calculations could be performed.</returns>
        public static bool TryGetCameraRelativeDirectionsXZPlane(this Camera camera, out Vector3 cameraForward, out Vector3 cameraRight)
        {
            if (camera == null)
            {
                cameraForward = Vector3.zero;
                cameraRight = Vector3.zero;
                return false;
            }

            // Calculate the camera-relative directions
            cameraForward = camera.transform.forward;
            cameraRight = camera.transform.right;

            // Normalize the camera directions on the XZ plane
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();

            return true;
        }
    }
}
