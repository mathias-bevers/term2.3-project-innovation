using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinScreen : NetworkingBehaviour
{
    [SerializeField] GameObject winScreenPanel;
    [SerializeField] Text typeWinText;
    [SerializeField] Text playerWinNamesText;

    Dictionary<int, string> idToName = new Dictionary<int, string>();

    internal override void Awoken()
    {
        winScreenPanel?.gameObject?.SetActive(false);
    }

    [NetworkRegistry(typeof(UserList), TrafficDirection.Both)]
    public void OnUserList(ServerClient client, UserList userList, TrafficDirection direction)
    {
        foreach(DeclareUser user in userList.users) 
        { 
            if(!idToName.ContainsKey(user.ID)) idToName.Add(user.ID, user.Name);
            else idToName[user.ID] = user.Name;
        }
    }

    internal void OnWinnerPacket(WinnerPacket packet)
    {
        winScreenPanel?.gameObject?.SetActive(true);

        WinWay winWay = packet.winway;
        int[] winIDs = packet.winnerIDs.ToArray();

        string names = string.Empty;

        foreach(int id in winIDs)
        {
            names += idToName[winIDs[0]] + ", ";
        }

        if (winWay == WinWay.SoloWin) typeWinText.text = "WINNER!";
        if (winWay == WinWay.TieWin) typeWinText.text = "WINNERS!";
        if (winWay == WinWay.TieLose) typeWinText.text = "TIE!";
        if (winWay == WinWay.AllLose) typeWinText.text = "NO WINNER!";

        playerWinNamesText.text = names.TrimEnd(new char[2] { ',', ' '});
    }
}
