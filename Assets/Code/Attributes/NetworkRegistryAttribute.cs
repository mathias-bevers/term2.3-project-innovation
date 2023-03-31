using System;

[System.AttributeUsage(AttributeTargets.Method)]
public class NetworkRegistryAttribute : Attribute
{
    public Type Type { get; private set; }
    public TrafficDirection TrafficDirection { get; private set; }

    public NetworkRegistryAttribute(Type type, TrafficDirection direction)
    {
        Type = type;
        TrafficDirection = direction;
    }
}
