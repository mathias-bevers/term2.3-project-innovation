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
    [Tooltip("This can be NULL! but if you want to overwrite the find, then make it not be")]
    [SerializeField] UserClient overrideClient;
    Dictionary<Type, List<MethodInfo>> invokables = new Dictionary<Type, List<MethodInfo>>();
    //The method count variable might look a bit spaghetti, but it's used to track if it should or should not run the more expensive invoke algorithms
    int methodCount = 0;

    private void Awake()
    {
        HandleAttributes();
        Awoken();
    }

    private void OnEnable()
    {
        overrideClient ??= FindObjectOfType<UserClient>();
        overrideClient?.Register(BaseReceivePacket);
        Enabled();
    }

    private void OnDisable()
    {
        overrideClient ??= FindObjectOfType<UserClient>();
        overrideClient?.Unregister(BaseReceivePacket);
        Disabled();
    }

    void HandleAttributes()
    {
        MethodInfo[] infos = GetType().GetMethods();
        if (infos.Length == 0) return;
        foreach (MethodInfo info in infos)
        {
            NetworkRegistryAttribute attr = info.GetCustomAttribute<NetworkRegistryAttribute>();
            if (attr == null) continue;
            ParameterInfo[] paras = info.GetParameters();
            if (paras.Length != 2) continue;
            if (paras[0].ParameterType != typeof(ServerClient)) continue;
            if (paras[1].ParameterType != attr.Type) continue;
            methodCount++;
            if (!invokables.ContainsKey(attr.Type)) invokables.Add(attr.Type, new List<MethodInfo>() { info });
            else invokables[attr.Type].Add(info);
        }
    }

    void BaseReceivePacket(ServerClient client, ISerializable serializable)
    {
        ReceivePacket(client, serializable);
        if (methodCount == 0) return;
        Type seriType = serializable.GetType();
        if (!invokables.ContainsKey(seriType)) return;
        foreach (MethodInfo info in invokables[seriType])
            info.Invoke(this, new object[] { client, serializable });
    }

    internal virtual void Awoken() { }
    internal virtual void Enabled() { }
    internal virtual void Disabled() { }
    internal virtual void ReceivePacket(ServerClient client, ISerializable serializable) { Debug.Log("Received a packet: " + serializable.GetType()); }

    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                Debug.Log(ip.ToString());
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
}
