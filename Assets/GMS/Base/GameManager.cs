using System;
using System.Collections.Generic;
using System.Reflection;

namespace GMS
{
    /// <summary>
    /// Logic class for managing all submanagers
    /// </summary>
    public class GameManager
    {
        private GameManagerData _gameManagerData;
        /// <summary>
        /// Base subManagers are intended to never be deloaded or unninitialized until game is done running.
        /// </summary>
        private List<ISubManager> _baseSubManagers = new List<ISubManager>();
        /// <summary>
        /// Working Queue that adds or removes subManagers dynamically and safely after the update method
        /// in the current frame is done.
        /// </summary>
        private Queue<SubManagerEdit> _enqueuedSubMangers = new Queue<SubManagerEdit>();
        /// <summary>
        /// SubManager list, intended to be flexible and dynamic for different gameplay and scenes.
        /// Allows loading and deloading the subManagers when needed.
        /// </summary>
        private List<ISubManager> _subManagers = new List<ISubManager>();
        
        
        #region Internals
        
        public void LoadData(GameManagerData gameManagerData)
        {
            _gameManagerData = gameManagerData;
            
            foreach (SubManagerData subManagerData in gameManagerData.subManagerDataList)
            {
                TryEnqueueSubManager(gameManagerData, subManagerData, out ISubManager subManager);
            }
        }
        
        private void InitializeSubManager(ISubManager subManager)
        {
            subManager.Initialize(this);
        }
        
        private void UnInitializeSubManager(ISubManager subManager)
        {
            subManager.UnInitialize();
        }

        /// <summary>
        /// Tries to add a new unique instance to the list of subManagers, will return false if it already exists or data was invalid
        /// </summary>
        public bool TryEnqueueSubManager(GameManagerData data, SubManagerData subManagerData, out ISubManager subManager)
        {
            subManager = default;
            bool added = false;
            if (!subManagerData.Loads)
            {
                return false;
            }
            
            Type dataType = Type.GetType(subManagerData.dataTypeName);
            if (dataType == null)
            {
                return added;
            }

            LinkDataLogicAttribute subManagerAttr = dataType.GetCustomAttribute<LinkDataLogicAttribute>();
            if (subManagerAttr != null)
            {
                Type subManagerLogicClassType = subManagerAttr.LogicClassType;
               
                // Find the corresponding data struct
                ISubManagerData dataInstance = null;
                foreach (ISubManagerData subManagerDataItem in data.subManagers)
                {
                    if (subManagerDataItem == null)
                    {
                        continue;
                    }
                    if (subManagerDataItem.GetType() == subManagerAttr.DataClassType)
                    {
                        dataInstance = subManagerDataItem;
                        break;
                    }
                }

                try
                {
                    if (dataInstance == null)
                    {
                        // If no matching data instance found, create the subManager without data
                        subManager = (ISubManager)Activator.CreateInstance(subManagerLogicClassType);
                    }
                    else
                    {
                        // Create an instance of the subManager logic class with the data instance
                        subManager = (ISubManager)Activator.CreateInstance(subManagerLogicClassType, dataInstance);
                    }

                    _enqueuedSubMangers.Enqueue(
                        new SubManagerEdit(
                            subManager,
                            add: true,
                            subManagerData.IsBase
                        ));
                    
                    added = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            
            return added;
        }

        /// <summary>
        /// Tries to remove an instance from the list of subManagers, will return false if it doesn't exist
        /// </summary>
        public bool TryEnqueueSubManagerForRemoval(SubManagerData subManagerData)
        {
            bool enqueuedForRemoval = false;

            Type dataType = Type.GetType(subManagerData.dataTypeName);
            if (dataType == null)
            {
                return enqueuedForRemoval;
            }

            LinkDataLogicAttribute subManagerAttr = dataType.GetCustomAttribute<LinkDataLogicAttribute>();
            if (subManagerAttr != null)
            {
                Type subManagerLogicClassType = subManagerAttr.LogicClassType;

                // Find the existing subManager
                ISubManager subManager = null;
                List<ISubManager> listToSearch = subManagerData.IsBase ? _baseSubManagers : _subManagers;

                foreach (ISubManager manager in listToSearch)
                {
                    if (manager.GetType() == subManagerLogicClassType)
                    {
                        subManager = manager;
                        break;
                    }
                }

                if (subManager != null)
                {
                    _enqueuedSubMangers.Enqueue(
                        new SubManagerEdit(
                            subManager,
                            add: false,
                            subManagerData.IsBase
                        ));

                    enqueuedForRemoval = true;
                }
            }

            return enqueuedForRemoval;
        }

        #endregion Internals

        public void UpdateSubManagers()
        {
            foreach (ISubManager baseSubManager in _baseSubManagers)
            {
                baseSubManager.OnUpdate();
            }
            
            foreach (ISubManager subManager in _subManagers)
            {
                subManager.OnUpdate();
            }

            EditPendingSubManagers();
        }

        private void EditPendingSubManagers()
        {
            if (_enqueuedSubMangers.Count == 0)
            {
                return;
            }

            Queue<ISubManager> subManagersAdded = new Queue<ISubManager>();

            while (_enqueuedSubMangers.Count > 0)
            {
                ProcessSubManagerEdit(_enqueuedSubMangers.Dequeue(), subManagersAdded);
            }

            InitializePendingSubManagers(subManagersAdded);
        }

        private void ProcessSubManagerEdit(SubManagerEdit edit, Queue<ISubManager> subManagersAdded)
        {
            List<ISubManager> listToEdit = edit.IsBase ? _baseSubManagers : _subManagers;
            ISubManager subManager = edit.SubManager;

            if (edit.ADD)
            {
                if (!listToEdit.Contains(subManager))
                {
                    listToEdit.Add(subManager);
                    subManagersAdded.Enqueue(subManager);
                }
            }
            else
            {
                UnInitializeSubManager(subManager);
                listToEdit.Remove(subManager);
                subManager = null;
            }
        }

        private void InitializePendingSubManagers(Queue<ISubManager> subManagersAdded)
        {
            while (subManagersAdded.Count > 0)
            {
                InitializeSubManager(subManagersAdded.Dequeue());
            }
        }

        /// <summary>
        /// Attempts to get an existing SubManager by type
        /// </summary>
        /// <typeparam name="T">Type of the subManager</typeparam>
        /// <returns>True if subManager is found and valid</returns>
        public bool TryGetSubManager<T>(out T subManager) where T : ISubManager
        {
            bool found = false;

            found = TryGetSubManagerFromList(_baseSubManagers, out subManager);

            if (!found)
            {
                found = TryGetSubManagerFromList(_subManagers, out subManager);
            }
            
            return found;
        }

        private bool TryGetSubManagerFromList<T>(List<ISubManager> list, out T subManager)
        {
            subManager = default;
            foreach (ISubManager element in list)
            {
                if (element is T manager)
                {
                    subManager = manager;
                    return true;
                }
            }
            return false;
        }
    }
}
