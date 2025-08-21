using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// This class serves as a static service provider for holding Event Listeners.
/// </summary>
public static class GameEventService
{
    // Dictionary to store listeners for each event type
    private static Dictionary<Type, Delegate> eventListeners = new Dictionary<Type, Delegate>();

    private static readonly Dictionary<Type, Queue<BaseGameEvent>> _pools = new Dictionary<Type, Queue<BaseGameEvent>>();

    /// <summary>
    /// Cache for event types by name, used for optimizing the method <see cref="FindEventTypeByName"/>
    /// </summary>
    private static Dictionary<string, Type> eventTypeCache = new Dictionary<string, Type>();

    /// <summary>
    /// Register a listener for a specific event type
    /// </summary>
    public static void RegisterListener<T>(Action<T> listener) where T : BaseGameEvent
    {
        Type eventType = typeof(T);
        if (eventListeners.TryGetValue(eventType, out var existingListeners))
        {
            // Combine the delegates
            eventListeners[eventType] = Delegate.Combine(existingListeners, listener);
        }
        else
        {
            eventListeners[eventType] = listener;
        }
    }

    /// <summary>
    /// Unregister a listener for a specific event type
    /// </summary>
    public static void UnregisterListener<T>(Action<T> listener) where T : BaseGameEvent
    {
        Type eventType = typeof(T);
        if (eventListeners.TryGetValue(eventType, out var existingListeners))
        {
            // Remove the delegate
            var newListeners = Delegate.Remove(existingListeners, listener);
            if (newListeners == null)
            {
                eventListeners.Remove(eventType);
            }
            else
            {
                eventListeners[eventType] = newListeners;
            }
        }
    }

    /// <summary>
    /// Trigger an event of a specific type
    /// </summary>
    public static void TriggerEvent<T>(T gameEvent) where T : BaseGameEvent
    {
        Type eventType = typeof(T);
        if (eventListeners.TryGetValue(eventType, out var listeners))
        {
            // Cast the delegate to the correct type and then invoke it
            var typedListeners = listeners as Action<T>;

            if (typedListeners != null)
            {
                typedListeners.Invoke(gameEvent);
            }
        }

        // Return the event to the pool after triggering
        ReturnEventToPool(gameEvent);
    }

    /// <summary>
    /// Non-generic version of TriggerEvent
    /// Used when calling the game event from reflection ie. Event selector fields.
    /// </summary>
    /// <param name="gameEvent"></param>
    public static void TriggerEvent(BaseGameEvent gameEvent)
    {
        Type eventType = gameEvent.GetType();
        if (eventListeners.TryGetValue(eventType, out var listeners))
        {
            // Dynamically invoke the listener
            listeners.DynamicInvoke(gameEvent);
        }

        // Return the event to the pool after triggering
        ReturnEventToPool(gameEvent);
    }

    /// <summary>
    /// Get an event instance from the pool or create a new one if the pool is empty.
    /// </summary>
    public static T GetEvent<T>() where T : BaseGameEvent, new()
    {
        Type eventType = typeof(T);

        // Check if there's a pool for this type
        if (!_pools.TryGetValue(eventType, out var pool))
        {
            pool = new Queue<BaseGameEvent>();
            _pools[eventType] = pool;
        }

        // Get an event from the pool or create a new one if empty
        if (pool.Count > 0)
        {
            return (T)pool.Dequeue();
        }

        return new T();
    }

    /// <summary>
    /// Return an event instance to the pool for reuse.
    /// </summary>
    private static void ReturnEventToPool(BaseGameEvent gameEvent)
    {
        Type eventType = gameEvent.GetType();

        if (!_pools.TryGetValue(eventType, out var pool))
        {
            pool = new Queue<BaseGameEvent>();
            _pools[eventType] = pool;
        }

        gameEvent.Reset(); // Reset the event state before pooling
        pool.Enqueue(gameEvent);
    }


    /// <summary>
    /// Method to find the concrete type of the event by its name
    /// </summary>
    /// <param name="eventName"></param>
    /// <returns></returns>
    public static Type FindEventTypeByName(string eventName)
    {
        // Check if the event type is already cached
        if (eventTypeCache.TryGetValue(eventName, out Type cachedType))
        {
            return cachedType;
        }

        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        PropertyInfo eventNameField = null;

        // Loop through each assembly
        foreach (var assembly in assemblies)
        {
            Type[] types = assembly.GetTypes();

            foreach (var type in types)
            {
                if (type.IsAbstract)
                {
                    continue;
                }
                if (type.IsSubclassOf(typeof(BaseGameEvent)))
                {
                    // Use reflection only to get the name
                    eventNameField = type.GetProperty("EventName", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (eventNameField != null)
                    {
                        // Create an instance of the event type to retrieve the name
                        var eventInstance = (BaseGameEvent)Activator.CreateInstance(type);
                        string eventTypeName = eventNameField.GetValue(eventInstance) as string;

                        // Cache the event type
                        eventTypeCache[eventTypeName] = type;

                        // Return the type if it matches the requested event name
                        if (eventTypeName == eventName)
                        {
                            return type;
                        }
                    }
                }
            }
        }

        return null; // Return null if no matching event is found
    }
}
