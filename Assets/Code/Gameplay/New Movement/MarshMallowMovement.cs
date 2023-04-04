using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class MarshMallowMovement : MonoBehaviour
{
    [SerializeField] bool controlledByPlayer;
    [SerializeField] float maxSpeed = 100;
    [SerializeField] float speed = 8;


    bool blockInput = false;
    private new Rigidbody rigidbody;
    private new CapsuleCollider collider;
    private Transform cachedTransform;
    private Vector2 input;


    private void Awake()
    {
        collider = GetComponent<CapsuleCollider>();
        rigidbody = GetComponent<Rigidbody>();
        cachedTransform = transform;
    }

    
    void Update()
    {
        if (!controlledByPlayer) return;
        if (blockInput)  return; 
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
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

        if(input.magnitude > 1) input.Normalize();
        Vector3 newInput = new Vector3(input.x, 0, input.y);
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
    }
}
