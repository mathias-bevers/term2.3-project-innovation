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

    float retry = 0;

    private void Start()
    {
        ipField.text = Settings.ip.ToString();
        portField.text = Settings.port.ToString();
    }

    public string sceneName = "SampleScene";

    private void Update()
    {
        retry += Time.deltaTime;
        if(retry > 3.5f)
        {
            retry = 0;
#if DEBUG
            Connect();
#endif
        }
    }

    public void OnName(string name)
    {
        Settings.preferName = name;
    }

    public void OnIP(string ip)
    {
        IPAddress.TryParse(ip, out Settings.ip);
    }

    public void OnPort(string port)
    {
        int.TryParse(port, out Settings.port);
    }

    public void ResetTimer()
    {
        retry = 0;
    }

    public void Connect()
    {
        SceneManager.LoadScene(sceneName);
    }
}
