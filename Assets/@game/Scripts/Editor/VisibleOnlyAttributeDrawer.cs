using UnityEngine;
using UnityEditor;

// Values of fields are shown in the inspector, but never editable
[CustomPropertyDrawer(typeof(VisibleOnlyAttribute))]
public class VisibleOnlyAttributeDrawer : PropertyDrawer
{
	// Necessary since some properties tend to collapse smaller than their content
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUI.GetPropertyHeight(property, label, true);
	}
 
	// Draw a disabled property field
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		GUI.enabled = false;
		EditorGUI.PropertyField(position, property, label, true);
		GUI.enabled = true;
	}
}