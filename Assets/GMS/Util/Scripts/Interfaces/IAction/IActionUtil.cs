using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GMS
{
    public static class IActionUtil
    {
        /// <summary>
        /// Casts IAction over all given references, and adds the valid ones. 
        /// </summary>
        /// <returns>True if at least one action was found.</returns>
        public static bool TryGetAllActions(MonoBehaviour[] monoBehaviours, out List<IAction> actions)
        {
            actions = new List<IAction>();
            foreach (MonoBehaviour mono in monoBehaviours)
            {
                IAction newAction = mono as IAction;
                if (newAction == null)
                {
                    continue;
                }
                actions.Add(newAction);
            }

            return actions.Count > 0;
        }

        /// <summary>
        /// Calls IActions do and undo
        /// </summary>
        /// <param name="callDoAction"> Calls IAction.Do if True, Calls IAction.Undo if False</param>
        public static void RunActions(List<IAction> actions, bool callDoAction)
        {
            foreach (IAction action in actions)
            {
                if (callDoAction)
                {
                    action.Do();
                }
                else
                {
                    action.Undo();
                }
            }
        }
    }
}
