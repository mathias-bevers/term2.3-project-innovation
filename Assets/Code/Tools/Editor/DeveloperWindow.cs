using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

public class DeveloperWindow : EditorWindow
{

	[MenuItem("Window/Developer Window")]
	private static void Initialize()
	{
		DeveloperWindow window = GetWindow<DeveloperWindow>();
		window.titleContent = new GUIContent("Developer Window");
		window.Show();
	}

	private void OnGUI()
	{
		GetCurrentTextSize();
	}

	private void GetCurrentTextSize()
	{
		if (!GUILayout.Button("Get selected text size"))
		{
			return;
		}

		GameObject[] selection = Selection.gameObjects;

		if (selection.Length == 0)
		{
			Debug.LogWarning("Need to select at least one game-object!");
			return;
		}

		StringBuilder stringBuilder = new();

		foreach (GameObject selected in selection)
		{
			if (!selected.TryGetComponent(out UnityEngine.UI.Text text))
			{
				Debug.LogError($"Selected game-object \'{selected.name}\' does not have a text component!");
				continue;
			}

			stringBuilder.Append("The text size of ");
			stringBuilder.Append(selected.name);
			stringBuilder.Append(" is:");
			stringBuilder.Append(' ', 25 - selected.name.Length);
			stringBuilder.AppendLine(text.fontSize.ToString());
		}

		Debug.Log(stringBuilder.ToString());
	}
}
