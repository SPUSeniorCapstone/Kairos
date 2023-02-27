using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(DisableOnPlayAttribute))]
public class DisableOnPlayPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        bool enabled = !Application.isPlaying;

        bool wasEnabled = GUI.enabled;
        GUI.enabled = enabled;
        if (enabled)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
        else
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, false);
            GUI.enabled = true;
        }

        GUI.enabled = wasEnabled;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        DisableOnPlayAttribute att = (DisableOnPlayAttribute)attribute;

        if (!att.Hide)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }

        bool enabled = !Application.isPlaying;

        if (enabled)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        else
        {
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }
}
#endif

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class DisableOnPlayAttribute : PropertyAttribute
{
    public bool Hide;

    public DisableOnPlayAttribute()
    {
        Hide = false;
    }

    public DisableOnPlayAttribute(bool hide)
    {
        Hide = hide;
    }
}
