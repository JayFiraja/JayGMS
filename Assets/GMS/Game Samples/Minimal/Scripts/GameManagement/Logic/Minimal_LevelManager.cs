using GMS;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GMS.Samples
{
    public class Minimal_LevelManager: ISubManager
    {

        private Minimal_LevelData _data;
        private GameObject _levelGameObject;

        public Minimal_LevelManager(Minimal_LevelData data)
        {
            _data = data;
        }

        public bool Initialize(GMS.GameManager gameManager)
        {
            Addressables.InstantiateAsync(_data.LevelPrefabAddress).Completed += OnLevelPrefabLoaded;
            return true;
        }

        public void UnInitialize()
        {
            Addressables.ReleaseInstance(_levelGameObject);
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

        private void OnLevelPrefabLoaded(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("Failed to instantiate prefab.");
                return;
            }

            _levelGameObject = handle.Result;
        }
    }
}
