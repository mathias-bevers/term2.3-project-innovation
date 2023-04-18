using System;
using UnityEngine;
using UnityEngine.Events;

public class Burger : MonoBehaviour
{
	private const float FORCE_Y = 0.56f;


	public event Action<Burger> LandedEvent;
	public Transform CachedTransform { get; private set; }
	private new Rigidbody rigidbody;

	[SerializeField] UnityEvent onLand;


	private void Awake()
	{
		CachedTransform = transform;

		rigidbody = GetComponent<Rigidbody>() ?? throw new NoComponentFoundException(typeof(Rigidbody));
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.TryGetComponent(out MarshMallowMovement _)) { return; }

		rigidbody.useGravity = false;
		rigidbody.velocity = Vector3.zero;

		Vector3 landPosition = CachedTransform.position;
		landPosition.y = FORCE_Y;
		CachedTransform.position = landPosition;

		LandedEvent?.Invoke(this);
		onLand?.Invoke();

    }
}