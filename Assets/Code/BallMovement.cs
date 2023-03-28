using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class BallMovement : MonoBehaviour
{
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

	private void Update() { input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")); }

	private void FixedUpdate() { Move(input); }

	private void Move(Vector3 input)
	{
		if (input.magnitude > 0)
		{
			input.Normalize();
			input *= speed;
			rigidbody.AddForce(input);

			if (rigidbody.velocity.magnitude <= maxSpeed) { return; }

			rigidbody.velocity *= maxSpeed / rigidbody.velocity.magnitude;
			return;
		}

		rigidbody.velocity *= 0.9f;
	}

	private void OnCollisionEnter(Collision collision)
	{
		// This is an other player.
		if (collision.collider.GetComponent<BallMovement>() == null)
		{
			return;
		}


		Debug.Log("Other player was hit!");
	}

	private void OnGUI() { GUI.Label(new Rect(0, 0, 250, 25), $"Velocity: {rigidbody.velocity.magnitude}"); }
}