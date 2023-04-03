using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotater : MonoBehaviour
{
    [SerializeField] bool smooth = true;
    [SerializeField] Vector3 rotate;
    [SerializeField] float unsmoothTime = 0.5f;

    float timer = 0;

    void Update()
    {
        if (smooth)
        {
            transform.Rotate(rotate * Time.deltaTime);

        }
        else
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                transform.Rotate(rotate);
                timer = unsmoothTime;
            }
        }
    }
}
