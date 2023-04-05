using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class MarshMallowMovement : IDedNetworkingBehaviour
{
    [SerializeField] float maxSpeed = 100;
    [SerializeField] float speed = 8;
    [SerializeField] float blockTime = 0.8f;

    float blockTimer = 0;
    bool blockInput = false;
    private new Rigidbody rigidbody;
    private Vector2 input;

    internal override void Awoken()
    {
        rigidbody = GetComponent<Rigidbody>();
    }


    [NetworkRegistry(typeof(InputPacket), TrafficDirection.Received)]
    public void Receive(ServerClient client, InputPacket packet, TrafficDirection direction)
    {
        if (blockInput) return;
        SerializableVector2 input = packet.input;
        this.input = new Vector2(input.x, input.y);
    }

    [NetworkRegistry(typeof(RequestRespawn), TrafficDirection.Received)]
    public void Receive(ServerClient client, RequestRespawn packet, TrafficDirection direction)
    {
        transform.position = new Vector3(0, 1, 0);
    }

    void Update()
    {
        blockTimer -= Time.deltaTime;
        if (blockTimer <= 0) blockInput = false;
    }

    private void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        if (input.magnitude <= 0)
        {
            rigidbody.velocity *= 0.98f;
            return;
        }

        Vector2 actInput = input;
        if(actInput.magnitude > 1) actInput.Normalize();
        Vector3 newInput = new Vector3(actInput.x, 0, actInput.y);
        newInput *= speed * Time.fixedDeltaTime;
        rigidbody.AddForce(newInput, ForceMode.Impulse);

        if (rigidbody.velocity.magnitude <= maxSpeed) { return; }

        Vector3 velocity = rigidbody.velocity;
        float division = maxSpeed / velocity.magnitude;
        Vector3 newVelocity = velocity * division;
        rigidbody.velocity = newVelocity;
    }
    public void OnCollisionStay(Collision collision)
    {
        MarshMallowMovement movement;
        if ((movement = collision.collider.GetComponent<MarshMallowMovement>()) == null) return;

        input = Vector3.zero;
        rigidbody.velocity = Vector3.zero;
        Vector3 newPos = movement.transform.position;
        newPos.y = transform.position.y;
        rigidbody.AddExplosionForce(10f, newPos, 2, 0, ForceMode.VelocityChange);
        blockInput = true;
        blockTimer = blockTime;
    }
}
