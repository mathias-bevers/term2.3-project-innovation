using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Linq;
using System.Reflection;
using System;
using UnityEngine.UI;

public static partial class Utils
{
    public static Vector3 GetRandomInRange(Vector3 min, Vector3 max)
    {
        float x = UnityEngine.Random.Range(min.x, max.x);
        float y = UnityEngine.Random.Range(min.y, max.y);
        float z = UnityEngine.Random.Range(min.z, max.z);

		return new Vector3(x, y, z);
	}

    public static float Map(float x, float in_min, float in_max, float out_min, float out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

    public static bool IsNullOrEmpty(this ICollection collection) => collection == null || collection.Count == 0;

    public static T GetComponentThrow<T>(this GameObject gObject) where T : Component
    {
        T t = gObject.GetComponent<T>();
        if (t == null) 
            throw new NoComponentFoundException($"No Component of type {nameof(T)} found on object.");
        return t;
    }

    public static T FindObjectOfTypeThrow<T>(bool includeInactive = true) where T : Component
    {
        T t = GameObject.FindObjectOfType<T>(includeInactive);
        if (t == null)
            throw new NoComponentFoundException($"No Object of type {nameof(T)} found.");
        return t;
    }

	public static List<Toggle> GetTogglesFromToggleGroup(this ToggleGroup group)
	{
		FieldInfo toggleField = typeof(ToggleGroup).GetField("m_Toggles", BindingFlags.Instance | BindingFlags.NonPublic);
		return toggleField == null
			? throw new NotSupportedException("\'m_Toggles\' no longer exist in this context")
			: toggleField.GetValue(group) as List<Toggle>;
	}
}