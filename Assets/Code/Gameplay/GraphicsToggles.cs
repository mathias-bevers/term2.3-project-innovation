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
		Debug.Log(ClientSettings.graphicsMode.ToString());
	}
}

public enum GraphicsMode { VeryLow, Medium, Ultra }