using System;
using UnityEngine;

public class Burger : MonoBehaviour
{
	public event Action<Burger> LandedEvent;
	public Transform CachedTransform { get; private set; }

	private void Awake()
	{
		CachedTransform = transform;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.TryGetComponent(out MarshMallowMovement _)) { return; }

		LandedEvent?.Invoke(this);
	}
}