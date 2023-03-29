using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IPTyper : MonoBehaviour
{
    public string sceneName = "SampleScene";

    public void OnIP(string ip)
    {
        Settings.ip = IPAddress.Parse(ip);
        SceneManager.LoadScene(sceneName);
    }
}
