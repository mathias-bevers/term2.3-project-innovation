using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public static class Settings
{
    public static IPAddress serverIP = IPAddress.Parse("127.0.0.1");
        //IPAddress.Broadcast;
    public static IPAddress ip = IPAddress.Parse("127.0.0.1");
    public static int port = Convert.ToInt32(25565);
    public const int maxPlayerCount = 4;
}
