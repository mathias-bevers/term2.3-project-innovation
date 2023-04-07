using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointList : MonoBehaviour
{
    [SerializeField] Transform[] points;

    public Transform this[int index]
    {
        get => points[index];
        set => points[index] = value;
    }

    public int Count { get => points.Length; }
}
