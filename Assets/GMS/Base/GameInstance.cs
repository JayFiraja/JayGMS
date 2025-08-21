using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GMS
{
    /// <summary>
    /// Singleton
    /// ServiceLocator
    /// Single point of contact for subManagers Initialization
    /// Holds the content Transforms for the subManager View elements.
    /// </summary>
    public class GameInstance : MonoBehaviour
    {
        [AddressableSelector, SerializeField, Tooltip("This one is left accessible in order to have the freedom to swap and test quickly")]
        private string gameManagerDataAddressableKey = "GameManagerData";

        private GameManager _gameManager;

        /// <summary>
        /// Has Game Instance been initialized.
        /// </summary>
        public bool Initialized { get; private set; }
        
        #region Singleton
        /// <summary>
        /// Singleton Access Property
        /// </summary>
        public static GameInstance S =>_S;
        private static GameInstance _S;
        private void Awake()
        {
            if (_S != null)
            {
                Debug.Log("GameManager: Only one instance can exist at any time. Destroying potential usurper.", _S.gameObject);
                Destroy(gameObject);
                return;
            }
            else 
            {
                _S = this;
                Initialize();
            }
        }
        #endregion Singleton
        
        private void Initialize()
        {
            _gameManager = new GameManager();
            // Load GameManagementData ScriptableObject
            Addressables.LoadAssetsAsync<GameManagerData>(key: gameManagerDataAddressableKey, callback: OnGameManagerDataLoaded);
        }

        private void OnGameManagerDataLoaded(GameManagerData gameManagerData)
        {
            _gameManager.LoadData(gameManagerData);
            Initialized = true;
        }
        
        private void Update()
        {
            _gameManager.UpdateSubManagers();
        }
        
        /// <summary>
        /// Attempts to get an existing SubManager by type
        /// </summary>
        /// <typeparam name="T">Type of the subManager</typeparam>
        /// <returns>True if subManager is found and valid</returns>
        public static bool TryGetSubManager<T>(out T subManager) where T : ISubManager
        {
            subManager = default;
            if (_S == null || !_S.Initialized)
            {
                return false;
            }
            
            return _S._gameManager.TryGetSubManager(out subManager);
        }

        /// <summary>
        /// Tries to add a new unique instance to the list of subManagers, will return false if it already exists or data was invalid
        /// </summary>
        public bool TryEnqueueSubManager(GameManagerData data, SubManagerData subManagerData, out ISubManager subManager)
        {
            return _gameManager.TryEnqueueSubManager(data, subManagerData, out subManager);
        }

        /// <summary>
        /// Tries to remove an instance from the list of subManagers, will return false if it doesn't exist from Game Manager
        /// </summary>
        public bool TryEnqueueSubManagerForRemoval(SubManagerData subManagerData)
        {
            return _gameManager.TryEnqueueSubManagerForRemoval(subManagerData);
        }
    }
}
