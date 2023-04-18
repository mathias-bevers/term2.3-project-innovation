using UnityEditor;
using UnityEngine;

public static class ClientSettings
{
    public static int volume = 80;
    public static bool useGyro = true;
    public static GraphicsMode graphicsMode;

    [RuntimeInitializeOnLoadMethod]
    public static void ResetSettings()
    {
        volume = 80;
        useGyro = true;
        graphicsMode = GraphicsMode.Medium;
    }
}
