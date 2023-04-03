using UnityEngine;

public class Follow : MonoBehaviour
{
	[SerializeField, ForceInitialization] private Transform target;

	private Transform cachedTransform;
	private Vector3 offset;

	private void Start()
	{
		cachedTransform = transform;
		offset = cachedTransform.position - target.position;
	}

	private void LateUpdate() { cachedTransform.position = target.position + offset; }
}