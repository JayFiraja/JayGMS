using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GMS.Samples
{
    /// <summary>
    /// Swaps GameManagerData from parameters
    /// </summary>
    public class SwapSubManagerTrigger : MonoBehaviour
    {
        [AddressableSelector]
        public string gameManagerDataToAddAddress;
        [AddressableSelector]
        public string gameManagerDataToRemoveAddress;

        private GameManagerData _gameManagerDataToAdd;
        private GameManagerData _gameManagerDataToRemove;

        private void Start()
        {
            Addressables.LoadAssetAsync<GameManagerData>(gameManagerDataToAddAddress).Completed += OnGameDataToAddLoaded;
            Addressables.LoadAssetAsync<GameManagerData>(gameManagerDataToRemoveAddress).Completed += OnGameDataToRemoveLoaded;
        }

        private void OnGameDataToAddLoaded(AsyncOperationHandle<GameManagerData> handle)
        {
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to load GameManagerData to Add with address{gameManagerDataToAddAddress}.");
                return;
            }
            _gameManagerDataToAdd = handle.Result;
        }

        private void OnGameDataToRemoveLoaded(AsyncOperationHandle<GameManagerData> handle)
        {
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to load GameManagerData to Remove with address{gameManagerDataToRemoveAddress}.");
                return;
            }
            _gameManagerDataToRemove = handle.Result;
        }

        private void OnDestroy()
        {
            Addressables.Release(_gameManagerDataToAdd);
            Addressables.Release(_gameManagerDataToRemove);
        }

        private void OnTriggerEnter(Collider other)
        {
            for (int i = 0; i < _gameManagerDataToAdd.subManagers.Count; i++)
            {
                SubManagerData subManagerData = _gameManagerDataToAdd.subManagerDataList[i];
                if (!subManagerData.Loads)
                {
                    continue;
                }
                GameInstance.S.TryEnqueueSubManager(_gameManagerDataToAdd, subManagerData, out ISubManager subManager);
            }

            for (int i = 0; i < _gameManagerDataToRemove.subManagers.Count; i++)
            {
                SubManagerData subManagerData = _gameManagerDataToRemove.subManagerDataList[i];
                if (!subManagerData.Loads)
                {
                    continue;
                }
                GameInstance.S.TryEnqueueSubManagerForRemoval(subManagerData);
            }
        }
    }
}

