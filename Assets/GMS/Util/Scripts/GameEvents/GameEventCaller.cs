using UnityEngine;
using System;

namespace GMS
{
    /// <summary>
    /// Select and call a game event in the inspector.
    /// Use case: UI elements like toggles and buttons. using UnityActions.
    /// </summary>
    public class GameEventCaller : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        public string selectedEventName;

        // Method to trigger the event by searching for it when needed
        public void TriggerEvent()
        {
            // Find the concrete type of the event based on its name
            Type eventType = GameEventService.FindEventTypeByName(selectedEventName);

            if (eventType != null)
            {
                // Create an instance of the event
                BaseGameEvent gameEvent = (BaseGameEvent)Activator.CreateInstance(eventType);

                // Trigger the event using GameEventService
                GameEventService.TriggerEvent(gameEvent);
            }
            else
            {
                Debug.LogError($"GameEvent with name '{selectedEventName}' not found!");
            }
        }
    }
}