using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRotation : MonoBehaviour
{
    [SerializeField] Vector3 eulerRotation;

    private void Update()
    {
        transform.rotation = Quaternion.Euler(eulerRotation);
    }
}
