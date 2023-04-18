using System;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsToggles : MonoBehaviour
{
	[SerializeField] private Toggle[] toggles;

	private void Awake()
	{
		if (toggles.IsNullOrEmpty())
		{
			Debug.LogError("Toggles cannot be null or empty, set in the inspector!");
			return;
		}

		for (int i = 0; i < toggles.Length; i++)
		{
			Toggle toggle = toggles[i];
			GraphicsMode mode = (GraphicsMode)i;
			toggle.onValueChanged.AddListener(isOn => OnToggled(isOn, mode));
		}
	}

	private void OnToggled(bool isOn, GraphicsMode mode)
	{
		if (!isOn) { return; }

		ClientSettings.graphicsMode = mode;
		QualitySettings.SetQualityLevel((int)mode, true);

		switch (mode)
		{
			case GraphicsMode.VeryLow:
				Application.targetFrameRate = 30;
				break;

			case GraphicsMode.Medium:
				Application.targetFrameRate = 45;
				break;

			case GraphicsMode.Ultra:
				Application.targetFrameRate = 60;
				break;

			default: throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
		}
	}
}

public enum GraphicsMode { VeryLow, Medium, Ultra }