#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SimplePluginManager
{
	public static class SimplePluginManagerUIUtils
	{
		public static void ErrorLabel(string label)
		{
			EditorGUILayout.HelpBox(label, MessageType.Error);
			
			// var prevColor = GUI.color;
			// GUI.color = Color.red;
			// GUILayout.Label(label, EditorStyles.boldLabel);
			// GUI.color = prevColor;
		}

		public static void Line(int height = 1)
		{
			Rect rect = EditorGUILayout.GetControlRect(false, height );
			rect.height = height;
			EditorGUI.DrawRect(rect, new Color ( 0.5f, 0.5f, 0.5f, 1 ) );
		}
	}
}
#endif