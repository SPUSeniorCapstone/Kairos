using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(DisableArrayAttribute))]
public class DisableArrayProperty : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        bool b = true;
        DrawPropertyArray(property, ref b);
        //EditorGUI.PropertyField(position, property, label, false);
        //GUI.enabled = true;
    }


    private void DrawPropertyArray(SerializedProperty property, ref bool fold)
    {

        EditorGUILayout.LabelField(property.name);
        int size = property.arraySize;
        property.Next(true);       //generic field (list)
        property.Next(true);       //arrays size field
        {
            GUI.enabled = false;
            EditorGUILayout.PropertyField(property, true);
        }
        for (int i = 0; i < size; i++)
        {
            property.Next(false);       //first array element        
            EditorGUILayout.PropertyField(property, true);
        }

        /**
        //if (!property.isArray)
        //{
        //    Debug.LogError("Only use propery \"DisableArray\" on an array object");
        //    return;
        //}

        //fold = EditorGUILayout.Foldout(fold, new GUIContent(
        //property.displayName,
        //"These are the waypoints that will be used for the moving object's path."), true);
        //if (!fold) return;


        //var arraySizeProp = property.FindPropertyRelative("Array.size");
        //EditorGUILayout.PropertyField(arraySizeProp);

        //EditorGUI.indentLevel++;

        //for (var i = 0; i < arraySizeProp.intValue; i++)
        //{
        //    GUI.enabled = false;
        //    EditorGUILayout.PropertyField(property.GetArrayElementAtIndex(i));
        //    GUI.enabled = true;
        //}

        //EditorGUI.indentLevel--;
        */
    }

}
#endif

public class DisableArrayAttribute : PropertyAttribute
{

}
