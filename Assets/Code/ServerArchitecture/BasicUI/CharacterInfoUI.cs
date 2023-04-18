using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoUI : NetworkingBehaviour
{
    [SerializeField] GameObject backgroundPanel;
    [SerializeField] Image backgroundColourImage;
    [SerializeField] Text usernameText;


    [SerializeField] List<ColourVector> colours = new List<ColourVector>();

    internal override void Awoken()
    {
        backgroundPanel.SetActive(false);
    }

    public void SetName(string name)
    {
        usernameText.text = name;
    }

    public void SetColor(ColourType type)
    {
        foreach (var item in colours)
        {
            if (item.colourType != type) continue;
            backgroundColourImage.color = item.colourMaterial;
        }
    }

    [NetworkRegistry(typeof(UserList), TrafficDirection.Received)]
    public void Receive(ServerClient client, UserList e, TrafficDirection direction)
    {
        backgroundPanel.SetActive(true);
        foreach (DeclareUser user in e.users)
        {
            if (user.ID != FindObjectOfType<UserClient>().ID) continue;
            SetName(user.Name);
            SetColor(user.Colour);
        }
    }

}
