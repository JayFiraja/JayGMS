using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the base class for all game events.
/// Provides core functionality for event data management, pooling, and triggering mechanisms.
/// </summary>
public abstract class BaseGameEvent : IGameEvent, IPoolable
{
    public string EventName { get; private set; }
    public string EventDescription { get; private set; }

    private Dictionary<string, object> eventData;

    protected BaseGameEvent()
    {
        // Use reflection to get the event name from the attribute
        var attribute = (GameEventAttribute)Attribute.GetCustomAttribute(this.GetType(), typeof(GameEventAttribute));
        if (attribute != null)
        {
            EventName = attribute.Name;
            EventDescription = attribute.Description;
        }
        else
        {
            Debug.LogWarning($"GameEventAttribute not found on {this.GetType().Name}. EventName and EventDescription will be empty.");
            EventName = string.Empty;
            EventDescription = string.Empty;
        }

        // Initialize event data dictionary
        eventData = new Dictionary<string, object>();
    }

    /// <summary>
    /// Adds data to the game event
    /// </summary>
    public void AddData<T>(string key, T value)
    {
        eventData[key] = value;
    }

    /// <summary>
    /// Tries to get the data Type with the given key
    /// </summary>
    public T GetData<T>(string key)
    {
        if (eventData.TryGetValue(key, out var value))
        {
            return (T)value;
        }

        throw new KeyNotFoundException($"Key '{key}' not found in event data.");
    }

    /// <summary>
    /// Validates if the data exists with the given key
    /// </summary>
    public bool HasData(string key)
    {
        return eventData.ContainsKey(key);
    }

    public virtual void Trigger()
    {
        GameEventService.TriggerEvent(this);
    }

    /// <summary>
    /// Resets the event data and state for reuse
    /// </summary>
    public virtual void Reset()
    {
        // Clear all event data
        eventData.Clear();
        // Reset any additional state here if necessary
    }

    public bool IsDormant()
    {
        throw new NotImplementedException();
    }
}
