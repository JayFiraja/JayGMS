using UnityEngine;

namespace GMS.Samples
{
    /// <summary>
    /// This is intended for demonstration purposes only.
    /// </summary>
    public class LookAtCamera : MonoBehaviour
    {
        [Header("Optional")]
        // If you want to specify a camera other than the main camera
        public Camera targetCamera;

        // You can specify an axis to lock the rotation, for example, only rotate on the Y-axis
        public bool lockX = false;
        public bool lockY = false;
        public bool lockZ = false;

        private void Update()
        {
            if (targetCamera != null)
            {
                // Calculate direction from object to camera
                Vector3 direction = targetCamera.transform.position - transform.position;

                // Optionally lock specific axes
                if (lockX) direction.x = 0;
                if (lockY) direction.y = 0;
                if (lockZ) direction.z = 0;

                // Calculate the new rotation for the object
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                // Apply the new rotation to the object
                transform.rotation = targetRotation;
            }
            else
            {
                targetCamera = Camera.main;
            }
        }
    }
}

