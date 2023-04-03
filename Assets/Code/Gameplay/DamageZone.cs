using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class DamageZone : MonoBehaviour
{
	private static readonly Collider[] PLAYERS = new Collider[4];
	private float radius;

	private Light coneLight;
	private Transform cachedTransform;

	private void Awake() { cachedTransform = transform; }


	public void Initialize(float radius)
	{
		radius = 20;
		this.radius = radius;

		coneLight = GetComponentInChildren<Light>() ?? throw new NoComponentFoundException(typeof(Light));
		if (coneLight.type != LightType.Spot)
		{
			throw new NotSupportedException("light of an AOE object should always be a spotlight");
		}

		Debug.Log($"position: {transform.position}, radius: {radius}");

		float height = coneLight.transform.position.y;
		float angle = (float)(Math.Atan(radius / height) * Mathf.Rad2Deg);
		//angle = Mathf.Abs(angle);

		coneLight.spotAngle = angle;


		Debug.Log($"tan({radius}/{height}) = \u2103 {angle}");
	}

	//TODO: change this to player component
	public GameObject[] GetPlayers()
	{
		int hitsCount = Physics.OverlapSphereNonAlloc(cachedTransform.position, radius, PLAYERS);

		List<GameObject> players = new(hitsCount);
		for (int i = 0; i < hitsCount; ++i) { players.Add(PLAYERS[i].gameObject); }


		Array.Clear(PLAYERS, 0, PLAYERS.Length);
		return players.ToArray();
	}
}