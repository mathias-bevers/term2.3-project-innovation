using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class DamageZoneManager : MonoBehaviour
{
	[SerializeField] private float zoneDuration;
	[SerializeField] private GameObject zonePrefab;
	[SerializeField] private int maxZones;
	[SerializeField, MinMaxSlider(10, 25)] private Vector2 radiusRange;

	private List<DamageZone> activeZones = new();
	private Transform cachedTransform;

	private void Awake()
	{
		if (zonePrefab == null) { throw new NullReferenceException("zonePrefab needs be set in the editor"); }

		cachedTransform = transform;

		InitializeNewZone();
	}


	public void InitializeNewZone()
	{
		if (activeZones.Count >= maxZones) { return; }

		float radius = Random.Range(radiusRange.x, radiusRange.y);
		Vector3 position = Random.insideUnitSphere * 10;
		position.y = 0;

		GameObject zoneGO = Instantiate(zonePrefab, position, Quaternion.identity);
		zoneGO.transform.parent = cachedTransform;

		DamageZone newZone = zoneGO.GetComponent<DamageZone>() ??
		                     throw new NoComponentFoundException("prefab does not have a \'DamageZone\' component.");
		newZone.Initialize(radius);
		activeZones.Add(newZone);

		CooldownManager.Cooldown(zoneDuration, OnZoneEnd, newZone);
	}

	private void OnZoneEnd(DamageZone zone)
	{
		// TODO: deal damage to players in the zone.
		GameObject[] players = zone.GetPlayers();

		activeZones.Remove(zone);
		Destroy(zone.gameObject);
	}
}