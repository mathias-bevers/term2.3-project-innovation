using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IPTyper : MonoBehaviour
{

    [SerializeField] InputField ipField;
    [SerializeField] InputField portField;

    private void Start()
    {
        ipField.text = Settings.ip.ToString();
        portField.text = Settings.port.ToString();
    }

    public string sceneName = "SampleScene";

    public void OnIP(string ip)
    {
        IPAddress.TryParse(ip, out Settings.ip);
    }

    public void OnPort(string port)
    {
        int.TryParse(port, out Settings.port);
    }

    public void Connect()
    {
        SceneManager.LoadScene(sceneName);
    }
}
