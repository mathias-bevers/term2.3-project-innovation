using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using UnityEditor;

public static class Settings
{
    public static string preferName = string.Empty;
    public static IPAddress serverIP = IPAddress.Parse("192.168.25.182");
        //IPAddress.Broadcast;
    public static IPAddress ip = IPAddress.Parse("192.168.25.182");
    public static int port = Convert.ToInt32(25566);
    public const int maxPlayerCount = 4;
    public const float ticksPerSecond = 50;

    [InitializeOnLoadMethod]
    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                UnityEngine.Debug.Log(ip.ToString());
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
}
