using System;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class IPTyperNew : MonoBehaviour
{
    [SerializeField] Sprite cross;
    [SerializeField] Sprite checkmark;

    [SerializeField] InputField ipField;
    [SerializeField] InputField portField;
    [SerializeField] InputField nameField;

    [SerializeField] Image ipImage;
    [SerializeField] Image nameImage;
    [SerializeField] Image portImage;
    [SerializeField] Button connectButton;

    [SerializeField] Toggle automaticReconnect;

    [SerializeField] Text pingText;

    static bool hasStarted = false;

    float timer = 0;

    float pingTimer = 0;

    Ping lastPing = null;

    private void OnEnable()
    {
#if DEBUG
        Settings.automaticReconnect = true;
#endif
        automaticReconnect.isOn = Settings.automaticReconnect;
#if !DEBUG
        if(!hasStarted) return;
#endif
        nameField.text = Settings.preferName;
        ipField.text = Settings.ip.ToString();
        portField.text = Settings.port.ToString();
    }

    public void OnStart()
    {
        hasStarted = true;
        SceneManager.LoadScene("Lobby");
    }

    public void SetAutoReconnect(bool value)
    {
        Settings.automaticReconnect = value;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer >= 3.5f)
        {
            if(Settings.automaticReconnect)
                OnStart();
            timer = 0;
        }

        pingTimer += Time.deltaTime;
        if(pingTimer >= 3)
        {
            pingTimer = 0;
            if (lastPing == null) return;
            {
                int time = lastPing.time;

                if (time >= 0)
                {
                    pingText.text = time.ToString();
                    lastPing?.DestroyPing();
                    lastPing = new Ping(ipField.text);
                }
                else pingText.text = ". . .";
            }
        }
    }

    public void OnValueChanged()
    {
        timer = 0;
        bool state1 = PortState(portField.text);
        bool state2 = IPState(ipField.text);

        if (!state2)
        {
            lastPing?.DestroyPing(); 
            lastPing = null;
            pingText.text = "X";
        }
        else
        {
            if(lastPing != null) 
                lastPing?.DestroyPing();
            lastPing = new Ping(ipField.text);
        }

        if (!state1 || !state2)
            connectButton.interactable = false;
        else
            connectButton.interactable = true;
    }

    public void TypingName(string name)
    {
        bool state = NameState(name);
        nameImage.sprite = state ? checkmark : cross;
    }

    public void TypingPort(string port)
    {
        bool state = PortState(port);
        portImage.sprite = state ? checkmark : cross;
    }

    public void TypingIP(string ip)
    {
        bool state = IPState(ip);
        ipImage.sprite = state ? checkmark : cross;
    }

    bool NameState(string name)
    {
        if (name.Length < 2 || name.Length > 12) return false;
        return true;
    }

    bool IPState(string ip)
    {
        ip = ip.Trim();
        if(ip.ToLower() == "self")
        {
            Settings.ip = IPAddress.Parse("127.0.0.1".Trim());
            return true;
        }
        else if(ip.ToLower() == "localhost")
        {
            Settings.ip = Settings.GetIPAddress();
            return true;
        }
        else if(ip.ToLower() == "any")
        {
            Settings.ip = IPAddress.Any;
            return true;
        }
        else if(ip.ToLower() == "loopback")
        {
            Settings.ip = IPAddress.Loopback;
            return true;
        }
        else if (ip.ToLower() == "broadcast")
        {
            Settings.ip = IPAddress.Broadcast;
            return true;
        }
        if (ip.Length < 7 || ip.Length > 16) return false;
        if (ip.Split('.').Length != 4) return false;

        try
        {
            Settings.ip = IPAddress.Parse(ip.Trim());
        }
        catch(Exception e) { Debug.Log(e.Message); return false; }
        return true;
    }

        bool PortState(string port)
    {
        if (port.Length <= 1 || port.Length > 5) return false;

        try
        {
            Settings.port = int.Parse(port);
        }
        catch { return false; }
        return true;
    }
}
