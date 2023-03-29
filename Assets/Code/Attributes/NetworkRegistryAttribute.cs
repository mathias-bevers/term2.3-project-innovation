using System;

[System.AttributeUsage(AttributeTargets.Method)]
public class NetworkRegistryAttribute : Attribute
{
    public Type Type { get; private set; }

    public NetworkRegistryAttribute(Type type)
    {
        Type = type;
    }
}
