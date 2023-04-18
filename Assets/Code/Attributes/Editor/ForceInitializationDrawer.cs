using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Code.Attributes.Editor
{
	[CustomPropertyDrawer(typeof(ForceInitializationAttribute))]
	public class ForceInitializationDrawer : PropertyDrawer
	{
		private const float MARGIN_HEIGHT = 2.0f;
		private const float MIN_BOX_HEIGHT = PADDING_HEIGHT * 5.0f;
		private const float PADDING_HEIGHT = 8.0f;

		private float propertyHeight = 0.0f;
		private string helpBoxMessage = string.Empty;

		/*
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void EnforceAttributes()
		{
			Debug.Log("ENFORCE....");

			StringBuilder stringBuilder = new();
			Type[] assemblyTypes = Assembly.GetAssembly(typeof(ForceInitializationAttribute)).GetTypes();

			List<FieldInfo> forcedFields = new();
			foreach (Type type in assemblyTypes)
			{

				foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
				{
					if (!field.IsDefined(typeof(ForceInitializationAttribute), false))
					{
						continue;
					}
					 
					forcedFields.Add(field);
				}
			}

			stringBuilder.AppendLine($"There are {forcedFields.Count} forced fields");
			Debug.Log(stringBuilder.ToString());
		}
		*/

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (attribute is not ForceInitializationAttribute initAttribute)
			{
				throw new NotSupportedException($"Can only draw for {nameof(ForceInitializationAttribute)}");
			}

			if (property.propertyType != SerializedPropertyType.ObjectReference)
			{
				helpBoxMessage = $"{attribute.GetType().Name} can only be used on a reference type";
				DrawHelpBox(position, property, label, MessageType.Warning);
				return;
			}

			if (!ReferenceEquals(property.objectReferenceValue, null))
			{
				EditorGUI.PropertyField(position, property, label);
				return;
			}

			helpBoxMessage = $"The \'{property.name}\' needs to be initialized";
			DrawHelpBox(position, property, label, MessageType.Error);
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			propertyHeight = base.GetPropertyHeight(property, label);

			if (!ShouldDrawHelpBox(property)) { return propertyHeight; }

			GUIContent content = new(helpBoxMessage);
			GUIStyle style = GUI.skin.GetStyle("helpbox");
			style.fontSize = 14;
			float boxHeight = style.CalcHeight(content, EditorGUIUtility.currentViewWidth);
			boxHeight += MARGIN_HEIGHT;


			return boxHeight > MIN_BOX_HEIGHT ? propertyHeight + boxHeight : MIN_BOX_HEIGHT + boxHeight;
		}

		private bool ShouldDrawHelpBox(SerializedProperty property) =>
			property.propertyType != SerializedPropertyType.ObjectReference ||
			(property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null);

		private void DrawHelpBox(Rect position, SerializedProperty property, GUIContent label, MessageType messageType)
		{
			EditorGUI.BeginProperty(position, label, property);
			Rect positionCopy = position;

			positionCopy.height -= propertyHeight + MARGIN_HEIGHT;
			EditorGUI.HelpBox(positionCopy, helpBoxMessage, messageType);
			position.y += positionCopy.height;
			position.height = propertyHeight;

			EditorGUI.PropertyField(position, property, label);
			EditorGUI.EndProperty();
		}
	}
}