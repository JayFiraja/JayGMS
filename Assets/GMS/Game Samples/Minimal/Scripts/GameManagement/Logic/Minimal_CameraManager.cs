using Cinemachine;
using GMS;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GMS.Samples
{
    public class Minimal_CameraManager: ISubManager
    {

        private Minimal_CameraData _data;

        private CinemachineVirtualCamera _cameraCinemachineVirtual;
        private CameraStoryBoard _cameraStoryboard;
        private GameObject _cameraParent;
        private Transform _cameraTarget;

        private bool _initialized = false;

        public Minimal_CameraManager(Minimal_CameraData data)
        {
            _data = data;
        }

        public bool Initialize(GameManager gameManager)
        {
            Addressables.InstantiateAsync(_data.CameraContainerPrefabAddress).Completed += OnCameraPrefabInstanced;
            return true;
        }

        public void UnInitialize()
        {
            GameEventService.UnregisterListener<MinimalGameEvents.OnPlayerInstanced>(OnPlayerControllableCharacterInstanced);
            Addressables.ReleaseInstance(_cameraParent);
        }

        public void OnUpdate()
        {
            // Ticked by GameManager every frame. Implement logic that requires constant update here
        }

        public bool Equals(ISubManager other)
        {
            // Check if other is null
            if (other == null)
            {
                return false;
            }

            // Compare the runtime types of the current instance and the other instance
            return GetType() == other.GetType();
        }

        public override bool Equals(object obj)
        {
            if (obj is ISubManager other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            // Generate a hash code based on the fields that contribute to equality
            return GetType().GetHashCode();
        }

        private void OnCameraPrefabInstanced(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("Failed to instantiate prefab.");
                return;
            }

            _cameraParent = handle.Result;
            SetCameraTransformData(new TransformData(_data.StartCameraPosition, 
                                                     _data.StartCameraRotation,
                                                     _data.StartCameraScale));
            _cameraCinemachineVirtual = _cameraParent.GetComponentInChildren<CinemachineVirtualCamera>();
            _cameraCinemachineVirtual.Follow = _cameraTarget;
            _cameraCinemachineVirtual.LookAt = _cameraTarget;

            _cameraStoryboard = _cameraParent.GetComponentInChildren<CameraStoryBoard>();
            _cameraStoryboard.Initialize();
            _cameraStoryboard.FadeIn();

            GameEventService.RegisterListener<MinimalGameEvents.OnPlayerInstanced>(OnPlayerControllableCharacterInstanced);
            _initialized = true;
        }

        private void OnPlayerControllableCharacterInstanced(MinimalGameEvents.OnPlayerInstanced onPlayerInstanced)
        {
            if (onPlayerInstanced.PlayerGameObject == null)
            {
                Debug.LogError($"Null reference fired on global action OnPlayerInstanced");
                return;
            }

            SetCameraTarget(onPlayerInstanced.PlayerGameObject.transform);
        }

        private void SetCameraTarget(Transform target)
        {
            if (_cameraTarget == null)
            {
                _cameraStoryboard.FadeOut(autoFadeIn: false, immediate: false);
            }
            _cameraTarget = target;
            _cameraCinemachineVirtual.Follow = target;
            _cameraCinemachineVirtual.LookAt = target;
        }

        private void SetCameraTransformData(TransformData transformData)
        {
            _cameraParent.transform.position = transformData.Position;
            _cameraParent.transform.rotation = transformData.Rotation;
            _cameraParent.transform.localScale = transformData.Scale;
        }
    }
}
