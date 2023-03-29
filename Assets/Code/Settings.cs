using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public static class Settings
{
    public static IPAddress serverIP = //IPAddress.Parse("145.53.12.100");
        IPAddress.Parse("192.168.2.10");
    public static IPAddress ip = IPAddress.Parse("192.168.2.10");
    public static int port = Convert.ToInt32(25565);
    public const int maxPlayerCount = 4;
}
