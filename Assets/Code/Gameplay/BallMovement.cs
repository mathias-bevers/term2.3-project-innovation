using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class BallMovement : MonoBehaviour
{
	[SerializeField] private bool isPlayerControlled;
	[SerializeField] private float maxSpeed = 100.0f;
	[SerializeField] private float speed = 5.0f;

	private bool blockPlayerInput;
	private new Rigidbody rigidbody;
	private new SphereCollider collider;
	private Transform cachedTransform;
	private Vector3 input;

	private void Awake()
	{
		collider = GetComponent<SphereCollider>() ?? throw new NoComponentFoundException(typeof(SphereCollider));
		rigidbody = GetComponent<Rigidbody>() ?? throw new NoComponentFoundException(typeof(Rigidbody));
		cachedTransform = transform;
	}

	private void Update()
	{
		if (!isPlayerControlled) { return; }

		if (blockPlayerInput) { return; }


		input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        rigidbody.position = new Vector3(rigidbody.position.x, 0.5f, rigidbody.position.z); Move(input);
    }

	//private void FixedUpdate() { rigidbody.position = new Vector3(rigidbody.position.x, 0.5f, rigidbody.position.z); Move(input); }

	private void Move(Vector3 input)
	{
		if (input.magnitude <= 0)
		{
			rigidbody.velocity *= 0.9f;
			return;
		}

		input.Normalize();
		input *= speed * Time.deltaTime;
		rigidbody.AddForce(input, ForceMode.Impulse);

		if (rigidbody.velocity.magnitude <= maxSpeed) { return; }

		Vector3 velocity = rigidbody.velocity;
		float division = maxSpeed / velocity.magnitude;
		Vector3 newVelocity = velocity * division;
		rigidbody.velocity = newVelocity;
	}

	private void OnCollisionStay(Collision collision)
	{
		//if (blockPlayerInput) return;
		// This is an other player.
		BallMovement ballmovement;

        if ((ballmovement = collision.collider.GetComponent<BallMovement>()) == null) { return; }
		input = Vector3.zero;
		rigidbody.velocity = Vector3.zero;
		Vector3 newPos = ballmovement.transform.position;
		newPos.y = transform.position.y;
        rigidbody.AddExplosionForce(100f, newPos, 0.5f, 1, ForceMode.VelocityChange);

		blockPlayerInput = true;

		CooldownManager.Cooldown(0.5f, () => blockPlayerInput = false);
	}

	private void OnGUI() { GUI.Label(new Rect(0, 0, 250, 25), $"Velocity: {rigidbody.velocity.magnitude}"); }
}