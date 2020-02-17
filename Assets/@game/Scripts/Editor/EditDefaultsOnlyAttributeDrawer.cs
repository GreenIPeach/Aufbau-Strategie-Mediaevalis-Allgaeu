using UnityEngine;
using UnityEditor;

// Values of fields are not editable when playing, intended to set start- values in the inspector
[CustomPropertyDrawer(typeof(EditDefaultsOnlyAttribute))]
public class EditDefaultsOnlyAttributeDrawer : PropertyDrawer
{
	// Necessary since some properties tend to collapse smaller than their content
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUI.GetPropertyHeight(property, label, true);
	}

	// Draw a disabled property field when playing
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		GUI.enabled = !Application.isPlaying;
		EditorGUI.PropertyField(position, property, label, true);
		GUI.enabled = true;
	}
}