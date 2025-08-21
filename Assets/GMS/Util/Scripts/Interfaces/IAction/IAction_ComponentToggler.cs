using System;
using UnityEngine;

namespace GMS
{
    /// <summary>
    /// Toggles an array of behaviours when action is performed
    /// </summary>
    public class IAction_ComponentToggler : MonoBehaviour, IAction
    {
        [Header("Assign Components to Toggle"), SerializeField]
        private MonoBehaviour[] monobehaviours = Array.Empty<MonoBehaviour>();
        [SerializeField]
        private Renderer[] renderers = Array.Empty<Renderer>();

        /// <inheritdoc/>
        public void Do()
        {
            ToggleComponents(true);
        }

        /// <inheritdoc/>
        public void Undo()
        {
            ToggleComponents(false);
        }

        private void ToggleComponents(bool isActive)
        {
            foreach (Behaviour element in monobehaviours)
            {
                element.enabled = isActive;
            }

            foreach (Renderer render in renderers)
            {
                render.enabled = isActive;
            }
        }
    }
}
