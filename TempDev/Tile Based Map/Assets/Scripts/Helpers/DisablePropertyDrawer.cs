using UnityEngine;
using UnityEditor;
using System;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(DisableAttribute))]
public class DisablePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, false);
        GUI.enabled = true;
    }
}
#endif

public class DisableAttribute : PropertyAttribute
{

}
