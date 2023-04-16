using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class MarshMallowMovement : IDedNetworkingBehaviour
{
    [SerializeField] float maxSpeed = 100;
    [SerializeField] float speed = 8;
    [SerializeField] float blockTime = 0.8f;
    [SerializeField] float rotationSpeed = 90;
    //[SerializeField] MeshRenderer indicator;

    [SerializeField] Animator animator;

    [SerializeField] List<ColourToObj> colours = new List<ColourToObj>();

    public float currentBurnedCounter = 0;

    float blockTimer = 0;
    bool blockInput = false;
    private new Rigidbody rigidbody;
    private Vector2 input;

    [SerializeField] UnityEvent onBounce;

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
    
    public void SetColour(ColourType type)
    {
        foreach(var colour in colours)
        {
            if (colour.Colour != type) continue;
            colour.obj.SetActive(true);
        }
    }

    void Update()
    {
        blockTimer -= Time.deltaTime;
        if (blockTimer <= 0) blockInput = false;

        animator.SetFloat("vel", rigidbody.velocity.magnitude);
        animator.SetBool("stun", blockInput);
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
        if (actInput.y < 0) actInput.y *= 0.68f;
        // transform.Rotate(new Vector3(0, rotationSpeed * actInput.x * Time.fixedDeltaTime, 0));
        transform.rotation = Quaternion.LookRotation(new Vector3(actInput.x, 0, actInput.y));

         Vector3 newInput = new Vector3(0, 0, actInput.magnitude);
        newInput *= speed * Time.fixedDeltaTime;
        rigidbody.AddRelativeForce(newInput, ForceMode.Impulse);

        if (rigidbody.velocity.magnitude <= maxSpeed) { return; }

        Vector3 velocity = rigidbody.velocity;
        float division = maxSpeed / velocity.magnitude;
        Vector3 newVelocity = velocity * division;
        rigidbody.velocity = newVelocity;


      
        
    }
    public void OnCollisionStay(Collision collision)
    {
        Component movement;
        if ((movement = GetOneOfThe(collision)) == null) return;

        Vector3 newPos = movement.transform.position;
        newPos.y = transform.position.y;
        AddForce(newPos, 10f, blockTime);
    }

    Component GetOneOfThe(Collision collision)
    {
        Collider collider = collision.collider;

        MarshMallowMovement movement = collider.GetComponent<MarshMallowMovement>();
        Burger burger = collider.GetComponent<Burger>();

        if(movement != null) return movement;
        if(burger != null) return burger;

        return null;
    }

    public void AddForce(Vector3 impactPosition, float force, float stunTime)
    {
	    input = Vector3.zero;
	    rigidbody.velocity = Vector3.zero;
        rigidbody.AddExplosionForce(force, impactPosition, 2,0,ForceMode.VelocityChange);
        rigidbody.transform.LookAt(impactPosition);
        blockInput = true;
        blockTimer = stunTime;
        onBounce?.Invoke();
    }
}

[System.Serializable]
public struct ColourToObj
{
    public ColourType Colour;
    public GameObject obj;
}