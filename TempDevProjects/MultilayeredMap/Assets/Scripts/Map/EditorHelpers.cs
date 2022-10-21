using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

static public class EditorHelpers
{
    public static float FloatSlider(string label, float value, float min, float max)
    {
        float ret;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label);

        ret = EditorGUILayout.Slider(value, min, max);
        EditorGUILayout.EndHorizontal();
        return ret;
    }

    public static int IntSlider(string label, int value, int min, int max)
    {
        int ret;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label);

        ret = EditorGUILayout.IntSlider(value, min, max);
        EditorGUILayout.EndHorizontal();
        return ret;
    }

    public static float FloatInput(string label, float value)
    {
        float ret;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label);

        ret = EditorGUILayout.FloatField(value);
        EditorGUILayout.EndHorizontal();
        return ret;
    }

    public static int IntInput(string label, int value)
    {
        int ret;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label);

        ret = EditorGUILayout.IntField(value);
        EditorGUILayout.EndHorizontal();
        return ret;
    }
}
