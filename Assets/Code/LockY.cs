using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockY : MonoBehaviour
{

    [SerializeField] float yPos;
   
    void Update()
    {
        transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
    }
}
