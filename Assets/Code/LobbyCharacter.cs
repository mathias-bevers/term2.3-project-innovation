﻿using UnityEngine;
using UnityEngine.UI;

public class LobbyCharacter : MonoBehaviour
{
    [HideInInspector]
    public int ID;
    string Name;

    [HideInInspector]
    public bool ready = false;

    [HideInInspector] public int assignedNum;

    [SerializeField] Text nameText;
    [SerializeField] Text idText;


    public void SetID(int id)
    {
        ID = id;
        //idText.text = id.ToString();
    }

    public void SetReady(bool ready)
    {
        this.ready = ready;
        idText.text = ready ? "✓" : "X";
    }

    public void SetName(string name)
    {
        Name = name;
        nameText.text = name;
    }
}
