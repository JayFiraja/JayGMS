using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GMS.Samples
{
    public class AnimationTriggerCatcher : MonoBehaviour
    {
        public OnAnimationTriggerEvent[] TriggerEvents;

        /// <summary>
        /// Tries to invoke event by key
        /// </summary>
        /// <returns>True if event was found with given key</returns>
        public bool TryInvokeEvent(string key)
        {
            bool eventCalled = false;
            for (int i = 0; i < TriggerEvents.Length; i++)
            {
                OnAnimationTriggerEvent triggerEvent = TriggerEvents[i];
                if (string.Equals(triggerEvent.Key, key))
                {
                    triggerEvent.Event?.Invoke();
                    eventCalled = true;
                    break;
                }
            }
            return eventCalled;
        }
    }

    [System.Serializable]
    public struct OnAnimationTriggerEvent
    {
        /// <summary>
        /// Readable Key for recognizing which action is linked to what.
        /// </summary>
        public string Key;
        /// <summary>
        /// Unity event to freely call any public method on any component.
        /// </summary>
        public UnityEvent Event;
    }
}
