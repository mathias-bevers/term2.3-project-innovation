using System.IO.Compression;
using System.IO;
using System.Text;
using System;
using UnityEngine;
using System.Net;
using System.Linq;

public static class Utils
{
    public static Vector3 GetRandomInRange(Vector3 min, Vector3 max)
    {
        float x = UnityEngine.Random.Range(min.x, max.x);
        float y = UnityEngine.Random.Range(min.y, max.y);
        float z = UnityEngine.Random.Range(min.z, max.z);

        return new Vector3(x, y, z);
    }

    
}