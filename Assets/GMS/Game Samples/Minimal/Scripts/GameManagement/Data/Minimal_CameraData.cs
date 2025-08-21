using UnityEngine;

namespace GMS.Samples
{
    [System.Serializable]
    [LinkDataLogic(typeof(Minimal_CameraData), dataDisplayName: "Minimal_CameraData", typeof(Minimal_CameraManager), displayName: "Minimal_CameraManager")]
    public struct Minimal_CameraData : ISubManagerData
    {
        [AddressableSelector]
        public string CameraContainerPrefabAddress;

        public Vector3 StartCameraPosition;
        public Vector3 StartCameraRotation;
        public Vector3 StartCameraScale;
    }
}
