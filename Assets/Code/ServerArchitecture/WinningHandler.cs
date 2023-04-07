using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinningHandler : NetworkingBehaviour
{
    List<int> winningIds;
    WinWay winWay;

    bool startup = false;

    public void Setup(List<int> winningIds, WinWay winWay)
    {
        this.winningIds = winningIds;
        this.winWay = winWay;
        startup = true;
    }

    private void Update()
    {
        if (!startup) return;


    }
}
