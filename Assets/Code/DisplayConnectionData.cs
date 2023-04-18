using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayConnectionData : MonoBehaviour
{
    [SerializeField] Text ipText;
    [SerializeField] Text portText;

    void Start()
    {
        ipText.text = Settings.serverIP.ToString();
        portText.text = Settings.port.ToString();
    }
}
