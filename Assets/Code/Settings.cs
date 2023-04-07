using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using UnityEditor;

public static class Settings
{
    public static string preferName = string.Empty;
    public static IPAddress serverIP = IPAddress.Parse("127.0.0.1");
    //public static IPAddress serverIP = IPAddress.Any;
        //IPAddress.Broadcast;
    public static IPAddress ip = IPAddress.Parse("127.0.0.1");
    public static int port = Convert.ToInt32(25565);
    public const int maxPlayerCount = 4;
    public const float ticksPerSecond = 20;


    public const float bakingMax = 500;
}
