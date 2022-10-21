using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapController))]
public class MapControllerEditor : Editor
{
    SerializedProperty generator;

    SerializedProperty seed;

    bool UpdateMapOnChange = false;

    private void OnEnable()
    {
        generator = serializedObject.FindProperty("mapGenerator");
    }

    public override void OnInspectorGUI()
    {
        var controller = (MapController)target;


        base.OnInspectorGUI();


        if(GUILayout.Button(new GUIContent("Generate Map")))
        {
            controller.GenerateMap();
        }


    }


}
