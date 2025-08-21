using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class GameEventAttribute : Attribute
{
    public string Name { get; }
    public string Description { get; }

    public GameEventAttribute(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
