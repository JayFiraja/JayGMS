using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GMS.Samples
{
    public class TicTacToeLevelManager: ISubManager
    {
        private TicTacToeLevelData _data;
        private GameObject _levelRendering;
        
        public TicTacToeLevelManager(TicTacToeLevelData data)
        {
            _data = data;
        }
        
        public bool Initialize(GMS.GameManager gameManager)
        {
            // Load and instantiate the prefab asynchronously
            Addressables.InstantiateAsync(_data.LevelRenderingPrefabAddress).Completed += OnLevelRenderingLoaded;
            return true;
        }
        
        public void UnInitialize()
        {
            Addressables.ReleaseInstance(_levelRendering);
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

        private void OnLevelRenderingLoaded(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to instantiate prefab with Address{_data.LevelRenderingPrefabAddress}.");
                return;
            }

            _levelRendering = handle.Result;
            // this position can be added as a parameter in the data, keeping it here as 0,0,0 by design for simplicity for this prototype
            _levelRendering.transform.position = Vector3.zero;
        }
    }
}
