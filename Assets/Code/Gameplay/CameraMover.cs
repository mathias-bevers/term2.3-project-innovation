using System.Collections.Generic;
using UnityEngine;


public class CameraMover : MonoBehaviour
{
    [SerializeField] List<Transform> targets = new List<Transform>();

    public Vector3 offset;
    public float smoothTime = .5f;

    public float minZoom = 40f;
    public float maxZoom = 10f;
    public float zoomLimiter = 50f;

    private Vector3 velocity;
    [SerializeField] Camera cam;

    public void Register(List<MarshMallowMovement> movements)
    {
        foreach(var movement in movements)
        {
            targets.Add(movement.transform);
        }
    }

    private void LateUpdate()
    {
        CleanTargets();
        if (targets.Count == 0)
            return;



        move();
        zoom();

    }

    void CleanTargets()
    {
        for(int i = targets.Count - 1; i >= 0; i--)
        {
            if (targets[i] == null)
            {
                targets.RemoveAt(i);
                continue;
            }
            if (targets[i].position.y <= -1)
                targets.RemoveAt(i);
        }
    }

    void zoom()
    {

        float newZoom = Mathf.Lerp(maxZoom, minZoom, getGreatestDistance() / zoomLimiter);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);


    }


    void move()
    {

        Vector3 centerPoint = getCenterPoint();
        Vector3 newPosition = centerPoint + offset;
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }
    float getGreatestDistance()
    {

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {

            bounds.Encapsulate(targets[i].position);


        }

        return bounds.size.x;

    }


    Vector3 getCenterPoint()
    {

        if (targets.Count == 1)
        {

            return targets[0].position;


        }

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {


            bounds.Encapsulate(targets[i].position);

        }

        return bounds.center;


    }


}