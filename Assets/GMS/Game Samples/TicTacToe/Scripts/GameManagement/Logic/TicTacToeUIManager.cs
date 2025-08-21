using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GMS.Samples
{
    public class TicTacToeUIManager : ISubManager
    {
        private TicTacToeUIData _data;

        private UIMainCanvas _canvasInstance;
        private GameManager _gameManager;
        private TicTacToeStateManager _toeStateManager;
        private bool _initialized = false;
        private bool _canvasAvailable = false;

        public TicTacToeUIManager(TicTacToeUIData data)
        {
            _data = data;
        }

        public bool Initialize(GameManager gameManager)
        {
            _gameManager = gameManager;
            _gameManager.TryGetSubManager(out _toeStateManager);
            GameEventService.RegisterListener<TTCEvents.GameStateChanged>(OnGameStateChanged);
            // Load and instantiate the prefab asynchronously
            Addressables.InstantiateAsync(_data.MainCanvasPrefabAddress).Completed += OnMainMenuLoaded;
            _initialized = true;
            return _initialized;
        }

        public void UnInitialize()
        {
            GameEventService.UnregisterListener<TTCEvents.GameStateChanged>(OnGameStateChanged);
            Addressables.ReleaseInstance(_canvasInstance.gameObject);
        }

        public void OnUpdate()
        {
            if (!_canvasAvailable)
            {
                return;
            }
            _canvasInstance.OnUpdate();
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

        private void OnMainMenuLoaded(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded &&
                  handle.Result.TryGetComponent(out _canvasInstance))
            {
                if (_toeStateManager != null)
                {
                    _canvasInstance.Initialize(_toeStateManager.CurrentState);
                }
                _canvasAvailable = true;
                Debug.Log("Prefab instantiated successfully.");
            }
            else
            {
                Debug.LogError("Failed to instantiate prefab.");
            }
        }

        private void OnGameStateChanged(TTCEvents.GameStateChanged gameStateChanged)
        {
            if (!_initialized)
            {
                return;
            }

            _canvasInstance.GameStateChanged(gameStateChanged.FromGameState, gameStateChanged.ToGameState);
        }
    }
}
