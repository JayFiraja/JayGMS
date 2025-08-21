using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GMS
{
    /// <summary>
    /// This class is the base for managing a pool of view side Behaviours.
    /// View Content are Behaviours that need to be spawened on GameObjects as view side handles.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ViewContent<T> where T : Component
    {
        private Dictionary<string, PoolData> _pools;
        private Transform _mainParent;
        private readonly object _lock = new object();

        // This inner class encapsulates the pooling logic and parent transform, keeping the code organized.
        private class PoolData
        {
            public List<T> Pool { get; set; }

            /// <summary>
            /// Parent of the pool
            /// </summary>
            public Transform parentTransform { get; set; }

            /// <summary>
            /// Constructor that initializes the pool and sets the parent as mainParent
            /// </summary>
            /// <param name="parent"></param>
            public PoolData(Transform parent)
            {
                Pool = new List<T>();
                parentTransform = parent;
            }

            /// <summary>
            /// Constructor for pools that create a new parent under the given parent
            /// </summary>
            /// <param name="name"></param>
            /// <param name="parent"></param>
            public PoolData(string name, Transform mainParent)
            {
                Pool = new List<T>();
                GameObject parentObj = new GameObject(name);
                parentObj.transform.SetParent(mainParent);
                parentTransform = parentObj.transform;
            }
        }

        /// <summary>
        /// Constructor for using passed in parent as parent of the pool.
        /// </summary>
        /// <param name="parent">existing transform parent for the pool</param>
        public ViewContent(Transform parent)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            _pools = new Dictionary<string, PoolData>();
            _mainParent = parent;
        }

        /// <summary>
        /// Constructor for creating a new parent transform with specified name
        /// </summary>
        /// <param name="mainParentName">pool name in the inspector</param>
        /// <param name="parent">Parent for the new pool's parent transform.</param>
        public ViewContent(string mainParentName, Transform parent)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (string.IsNullOrEmpty(mainParentName)) throw new ArgumentException("Parent name cannot be null or empty.", nameof(mainParentName));

            _pools = new Dictionary<string, PoolData>();
            GameObject mainParentObj = new GameObject(mainParentName);
            mainParentObj.transform.SetParent(parent);
            _mainParent = mainParentObj.transform;
        }

        /// <summary>
        /// Creates or gets a new pool instance
        /// </summary>
        /// <param name="category">used as key of the pool collection</param>
        /// <param name="prefab">Pool's prefab to instance</param>
        /// <returns></returns>
        public T GetOrCreate(T prefab, string category)
        {
            return GetOrCreate(prefab, category, string.Empty, TransformData.Default);
        }

        /// <summary>
        /// Gets or creates a new pooled Behaviour 
        /// this variation parents the pool under the main parent assigned on the pool initialization.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="categoryParentName">Parent name of the pool</param>
        /// <param name="prefab">Pool's prefab to instance</param>
        /// <returns></returns>
        public T GetOrCreate(T prefab, string category, string categoryParentName)
        {
            return GetOrCreate(prefab, category, categoryParentName, TransformData.Default);
        }

        /// <summary>
        /// Gets or creates a new pooled Behaviour ad a given position and rotation
        /// this variation parents the pool under the main parent assigned on the pool initialization.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="categoryParentName">Parent name of the pool</param>
        /// <param name="position">new world position of the pooled instance</param>
        /// <param name="rotation">new rotation of the new pooled instance</param>
        /// <param name="prefab">Pool's prefab to instance</param>
        /// <returns></returns>
        public T GetOrCreate(T prefab, string category, string categoryParentName, TransformData transformData)
        {
            // Ensure Thread Safety
            lock (_lock)
            {
                if (!_pools.ContainsKey(category))
                {
                    if (string.IsNullOrEmpty(categoryParentName))
                    {
                        _pools[category] = new PoolData(_mainParent);
                    }
                    else
                    {
                        _pools[category] = new PoolData(categoryParentName, _mainParent);
                    }
                }

                List<T> pool = _pools[category].Pool;
                T component = GetInactiveOrCreateNew(pool, prefab, _pools[category].parentTransform);

                Transform transform = component.transform;
                // this method SetPositionAndRotation is only available in Unity 2022 and onward,
                // use when possible as it's more optimized to perform in a single pass.
                //transform.SetPositionAndRotation(position, rotation);
                transform.transform.position = transformData.Position;
                transform.rotation = transformData.Rotation;

                if (!component.gameObject.activeSelf)
                {
                    component.gameObject.SetActive(true);
                }

                return component;
            }
        }

        private T GetInactiveOrCreateNew(List<T> pool, T prefab, Transform parentTransform)
        {
            foreach (T pooledComponent in pool)
            {
                if (!pooledComponent.gameObject.activeInHierarchy)
                {
                    return pooledComponent;
                }
            }

            GameObject obj = GameObject.Instantiate(prefab.gameObject, parentTransform);
            T component = obj.GetComponent<T>();
            pool.Add(component);
            return component;
        }

        /// <summary>
        /// Tries to get a valid pool from the specified category
        /// </summary>
        /// <returns>returns false if pool was not found</returns>
        public bool GetPool(string category, out List<T> pool)
        {
            pool = null;
            if (!_pools.ContainsKey(category))
            {
                return false;
            }

            pool = _pools[category].Pool;

            return pool.Count > 0;
        }

        /// <summary>
        /// Set's the component's gameobject active state to false.
        /// </summary>
        /// <param name="component"></param>
        public void ReturnToPool(T component)
        {
            component.gameObject.SetActive(false);
        }

        /// <summary>
        /// Destroys all the view content and parents and clear all pool's data.
        /// Never destroy the GameInstance's gameobject.
        /// </summary>
        public void DestroyViewContent(string parentNameToDelete)
        {
            // Ensure thread safety while modifying the pools
            lock (_lock)
            {
                // Iterate over each pool in the dictionary
                foreach (var poolEntry in _pools)
                {
                    PoolData poolData = poolEntry.Value;

                    // Deactivate and destroy each object in the pool
                    foreach (T component in poolData.Pool)
                    {
                        if (component != null)
                        {
                            // Deactivate the object
                            component.gameObject.SetActive(false);

                            // Destroy the game object
                            GameObject.Destroy(component.gameObject);
                        }
                    }

                    // Clear the pool list
                    poolData.Pool.Clear();

                    // Destroy the pool's parent transform
                    if (poolData.parentTransform != null)
                    {
                        GameObject.Destroy(poolData.parentTransform.gameObject);
                    }
                }

                // Destroy the main parent transform if it matches with the requested name.
                if (string.Equals(_mainParent.gameObject.name, parentNameToDelete))
                {
                    GameObject.Destroy(_mainParent.gameObject);
                }

                // Clear the entire pools dictionary
                _pools.Clear();
            }
        }
    }
}
