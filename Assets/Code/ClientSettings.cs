using UnityEditor;
using UnityEngine;

public static class ClientSettings
{
    public static int volume = 80;
    public static bool useGyro = true;

    [RuntimeInitializeOnLoadMethod]
    public static void ResetSettings()
    {
        volume = 80;
        useGyro = true;
    }
}
