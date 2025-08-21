using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GMS
{
    /// <summary>
    /// Wrapper for being able to easily select SubManagers.
    /// Once a submanager is selected the corresponding submanager data struct shows up.
    /// <see cref="GameManagerDataEditor"/> for editor drawing.
    /// </summary>
    [CreateAssetMenu(fileName = "GameManagerData", menuName = "GMS/GameManagerData")]
    public class GameManagerData : ScriptableObject
    {
        /// <summary>
        /// Select which subManagers to spawn
        /// </summary>
        [Tooltip("Select which subManagers to spawn")]
        public List<SubManagerData> subManagerDataList = new List<SubManagerData>();
        
        /// <summary>
        /// List to hold subManager data
        /// </summary>
        [SerializeReference]
        public List<ISubManagerData> subManagers = new List<ISubManagerData>();

#if UNITY_EDITOR
        private void OnEnable()
        {
            CleanupSubManagers();
        }
        
        private void OnValidate()
        {
            CleanupSubManagers();
        }

        [ContextMenu("Cleanup SubManagers")]
        public void CleanupSubManagers()
        {
            subManagers.RemoveAll(subManagerData => subManagerData == null || !IsSubManagerTypeValid(subManagerData.GetType()));
        }

        private bool IsSubManagerTypeValid(Type subManagerType)
        {
            // Check if the type still exists
            return Type.GetType(subManagerType.AssemblyQualifiedName) != null;
        }
#endif
    }
}