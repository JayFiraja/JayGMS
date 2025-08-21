using UnityEngine;

public interface IGameEvent
{
    string EventName { get; }
    void Trigger();
}
