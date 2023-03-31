using shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Reflection;
using UnityEngine;

public class NetworkingBehaviour : MonoBehaviour
{
    [SerializeField] NetworkTarget networkTarget;

    internal IRegistrable overrideClient;
    Dictionary<Type, List<DirectionalMethodInfo>> invokables = new Dictionary<Type, List<DirectionalMethodInfo>>();
    //The method count variable might look a bit spaghetti, but it's used to track if it should or should not run the more expensive invoke algorithms
    int methodCount = 0;

    public int ID { get => overrideClient?.ID ?? -1; }

    private void Awake()
    {
        if(overrideClient == null)
        {
            if (networkTarget == NetworkTarget.Client) overrideClient = FindObjectOfType<UserClient>();
            else if (networkTarget == NetworkTarget.Server) overrideClient = FindObjectOfType<MainServer>();
        }

        if(overrideClient == null)
        {
            Destroy(this);
            return;
        }
        HandleAttributes();
        Awoken();
    }

    private void OnEnable()
    {
        overrideClient?.Register(BaseReceivePacket);
        Enabled();
    }

    private void OnDisable()
    {
        overrideClient?.Unregister(BaseReceivePacket);
        Disabled();
    }

    void HandleAttributes()
    {
        MethodInfo[] infos = GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (infos.Length == 0) return;
        foreach (MethodInfo info in infos)
        {
            NetworkRegistryAttribute attr = info.GetCustomAttribute<NetworkRegistryAttribute>();
            if (attr == null) continue;
            ParameterInfo[] paras = info.GetParameters();
            if (paras.Length != 3) continue;
            if (paras[0].ParameterType != typeof(ServerClient)) continue;
            if (paras[1].ParameterType != attr.Type) continue;
            if (paras[2].ParameterType != typeof(TrafficDirection)) continue;
            methodCount++;
            if (!invokables.ContainsKey(attr.Type)) invokables.Add(attr.Type, new List<DirectionalMethodInfo>() { new DirectionalMethodInfo(attr.TrafficDirection, info) });
            else invokables[attr.Type].Add(new DirectionalMethodInfo(attr.TrafficDirection, info));
        }
    }

    void BaseReceivePacket(ServerClient client, ISerializable serializable, TrafficDirection direction)
    {
        //if (direction == TrafficDirection.Send ) return;
        ReceivePacket(client, serializable);
        if (methodCount == 0) return;
        Type seriType = serializable.GetType();
        if (!invokables.ContainsKey(seriType)) return;
        foreach (DirectionalMethodInfo info in invokables[seriType])
        {
            bool pass = false;
            if (direction == TrafficDirection.Both) pass = true;
            if (direction == info.Direction) pass = true;
            if (!pass) continue;
            info.MethodInfo.Invoke(this, new object[] { client, serializable, direction });

        }
    }

    internal virtual void Awoken() { }
    internal virtual void Enabled() { }
    internal virtual void Disabled() { }
    internal virtual void ReceivePacket(ServerClient client, ISerializable serializable) { }

    internal void SendMessage(ISerializable message)
    {
        overrideClient?.SendPacket(message);
    }
}

public class DirectionalMethodInfo
{
    public TrafficDirection Direction { get; private set; }
    public MethodInfo MethodInfo { get; private set; }
    
    public DirectionalMethodInfo(TrafficDirection direction, MethodInfo methodInfo)
    {
        Direction = direction;
        MethodInfo = methodInfo;
    }
}