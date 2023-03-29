using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class BallMovement : MonoBehaviour
{
	[SerializeField] private bool blockPlayerInput;
	[SerializeField] private bool isPlayerControlled;
	[SerializeField] private float maxSpeed = 100.0f;
	[SerializeField] private float speed = 5.0f;

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

		input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
	}

	private void FixedUpdate() { Move(input); }

	private void Move(Vector3 input)
	{
		if (input.magnitude <= 0)
		{
			rigidbody.velocity *= 0.9f;
			return;
		}

		input.Normalize();
		input *= speed;
		rigidbody.AddForce(input);

		if (rigidbody.velocity.magnitude <= maxSpeed) { return; }

		Vector3 velocity = rigidbody.velocity;
		float division = maxSpeed / velocity.magnitude;
		Vector3 newVelocity = velocity * division;
		rigidbody.velocity = newVelocity;
	}

	private void OnCollisionEnter(Collision collision)
	{
		// This is an other player.
		if (collision.collider.GetComponent<BallMovement>() == null) { return; }

		rigidbody.AddExplosionForce(25f, collision.contacts[0].point, 0.5f, 1, ForceMode.Impulse);
		blockPlayerInput = true;

		CooldownManager.Cooldown(1.5f, () => blockPlayerInput = false);
	}

	private void OnGUI() { GUI.Label(new Rect(0, 0, 250, 25), $"Velocity: {rigidbody.velocity.magnitude}"); }
}