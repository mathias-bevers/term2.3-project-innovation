using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using UnityEditor;

public static class Settings
{
    public static string preferName = string.Empty;
    public static IPAddress serverIP = IPAddress.Any; 
    //public static IPAddress serverIP = IPAddress.Parse("192.168.2.10");
    //public static IPAddress serverIP = GetIPAddress();
    //public static IPAddress serverIP = IPAddress.Any;
    //IPAddress.Broadcast;
    public static IPAddress ip = IPAddress.Parse("145.53.12.100");
    public static int port = Convert.ToInt32(45000);
    public const int maxPlayerCount = 4;
    public const float ticksPerSecond = 20;


    public const float bakingMax = 500;

    public static IPAddress GetIPAddress()
    {
        IPHostEntry hostEntry = Dns.GetHostEntry(Environment.MachineName);

        foreach (IPAddress address in hostEntry.AddressList)
        {
            if (address.AddressFamily == AddressFamily.InterNetwork)
                return address;
        }

        return null;
    }

}
