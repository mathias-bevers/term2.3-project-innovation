using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class BurgerManager : MonoBehaviour
{
	private readonly List<Burger> activeBurgers = new();

	[SerializeField] private float despawnDelay;
	[SerializeField] private GameObject burgerPrefab;
	[SerializeField] private int maxBurgers;
	[SerializeField] private float landingForce;

	private GameplayHandler gameplayHandler;
	private Transform cachedTransform;

	private float burgerRadius = -1;
	private float distanceCutoff;
	private float shockZone;

	private void Awake()
	{
		if (burgerPrefab == null) { throw new NullReferenceException("zonePrefab needs be set in the editor"); }

		gameplayHandler = FindObjectOfType<GameplayHandler>() ??
		                  throw new NoComponentFoundException($"The object {nameof(GameplayHandler)} could not be found in the scene");

		cachedTransform = transform;
	}

	[NaughtyAttributes.Button("Spawn burger")]
	public void InitializeNewZone()
	{
		if (activeBurgers.Count >= maxBurgers) { return; }

		float distance = Random.Range(3.0f, 9.0f);
		Vector2 position = Random.insideUnitCircle;
		position.Normalize();
		position *= distance;

		Debug.Log($"Spawning burger at {position}");

		GameObject burgerGO = Instantiate(burgerPrefab, new Vector3(position.x, 10, position.y), Quaternion.identity);
		burgerGO.transform.SetParent(cachedTransform);

		Burger burger = burgerGO.GetComponent<Burger>() ??
		                     throw new NoComponentFoundException("prefab does not have a \'Burger\' component.");

		burger.LandedEvent += OnBurgerLanded;

		activeBurgers.Add(burger);

		if (burgerRadius > 0)
		{
			return;
		}

		burgerRadius = burgerGO.GetComponent<Collider>().bounds.size.x * 0.5f;
		distanceCutoff = burgerRadius * 5f;
		shockZone = distanceCutoff - burgerRadius;
	}

	private void BurgerDespawn(Burger burger)
	{
		activeBurgers.Remove(burger);
		Destroy(burger.gameObject);
	}

	private void OnBurgerLanded(Burger burger)
	{
		burger.LandedEvent -= OnBurgerLanded;
		CooldownManager.Cooldown(despawnDelay, BurgerDespawn, burger);


		Vector3 burgerPosition = burger.CachedTransform.position;
		foreach (MarshMallowMovement player in gameplayHandler.SpawnedCharacters)
		{
			Vector3 playerPosition = player.transform.position;
			Vector3 direction = burgerPosition - playerPosition;
			float distance = direction.magnitude;

			if (distance >= distanceCutoff)
			{
				continue;
			}

			float forceMultiplier = 1.0f;

			if (distance > burgerRadius)
			{
				float reducedDistance = distance - burgerRadius;
				float normalizedDistance = reducedDistance / shockZone;
				forceMultiplier -= normalizedDistance;
			}

			direction.y = 0;
			direction.Normalize();
			
			player.AddForce(playerPosition + direction * 0.5f, landingForce * forceMultiplier, 1.0f);
		}
	}
}