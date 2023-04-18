using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGoldenState : MonoBehaviour
{
    Material golden;

    private void Awake()
    {
        golden = GetComponent<SkinnedMeshRenderer>().materials[0];
    }

    public void SetState(float amount)
    {
        Color oldCol = golden.color;
        oldCol.a = amount;
        golden.color = oldCol;
    }
}
