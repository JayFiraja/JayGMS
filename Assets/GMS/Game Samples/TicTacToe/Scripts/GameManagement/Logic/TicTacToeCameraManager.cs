using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cinemachine;

namespace GMS.Samples
{
    public class TicTacToeCameraManager: ISubManager
    {
        // Camera Components
        private CameraController _cameraController;
        private CinemachineVirtualCamera _cameraCinemachineVirtual;
        private CameraStoryBoard _cameraStoryboard;

        private TicTacToeCameraData _data;
        private GameObject _cameraParent;
        private Transform _cameraTarget;
        private const string CAMERA_TARGET_NAME = "CameraTarget";

        private bool _initialized = false;
        
        public TicTacToeCameraManager(TicTacToeCameraData data)
        {
            _data = data;
        }
        
        public bool Initialize(GameManager gameManager)
        {
            _cameraTarget = new GameObject(CAMERA_TARGET_NAME).transform;

            // use this single method for setting position and rotation Unity v 2022.3 or newer
            //_cameraTarget.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            _cameraTarget.position = Vector3.zero;
            _cameraTarget.rotation = Quaternion.identity;

            Addressables.InstantiateAsync(_data.CameraContainerPrefabAddress).Completed += OnCameraPrefabLoaded;
            return true;
        }
        
        public void UnInitialize()
        {
            GameEventService.UnregisterListener<TTCEvents.GameStateChanged>(OnGameStateChanged);
            _cameraController.UnInitialize();
            _cameraStoryboard.UnInitialize();
            Addressables.ReleaseInstance(_cameraParent);
        }
        
        public void OnUpdate()
        {
            if (!_initialized)
            {
                return;
            }
            // Ticked by GameManager every frame. Implement logic that requires constant update here
            _cameraController.OnUpdate();
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

        private void OnCameraPrefabLoaded(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("Failed to instantiate prefab.");
                return;
            }

            _cameraParent = handle.Result;
            _cameraCinemachineVirtual = _cameraParent.GetComponentInChildren<CinemachineVirtualCamera>();
            _cameraCinemachineVirtual.Follow = _cameraTarget;
            _cameraCinemachineVirtual.LookAt = _cameraTarget;

            _cameraController = _cameraParent.GetComponentInChildren<CameraController>();
            _cameraController.Initialize();

            _cameraStoryboard = _cameraParent.GetComponentInChildren<CameraStoryBoard>();
            _cameraStoryboard.Initialize();

            GameEventService.RegisterListener<TTCEvents.GameStateChanged>(OnGameStateChanged);
            _initialized = true;
        }

        private void OnGameStateChanged(TTCEvents.GameStateChanged gameStateChanged)
        {
            switch (gameStateChanged.ToGameState)
            {
                case GameState.Loading:
                    _cameraStoryboard.FadeIn(autoFadeOut: true, immediate: false);
                    break;

                case GameState.MainMenu:
                    if (gameStateChanged.FromGameState == GameState.Results)
                    {
                        _cameraStoryboard.FadeIn(true, immediate: false);
                    }
                    break;

                case GameState.GameLoop:
                case GameState.Results:
                    _cameraController.IsCameraInputAllowed = true;
                    break;

                default:
                    _cameraController.IsCameraInputAllowed = false;
                    break;
            }
        }
    }
}
