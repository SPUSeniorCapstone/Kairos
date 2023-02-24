#if (UNITY_EDITOR) 
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BlockManager))]
public class BlockManagerEditor : Editor
{

    SerializedProperty blockTypes;

    int removeAt, insertAt;
    bool disabled = false;
    Vector2 scrollPos;

    private void OnEnable()
    {
        blockTypes = serializedObject.FindProperty("blockTypes");

    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        var b = target as BlockManager;



        EditorGUI.BeginChangeCheck();

        ///GUILayout.Label("Block Types", EditorStyles.boldLabel);
        ///scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));
        ///for (int i = 0; i < blockTypes.arraySize; i++)
        ///{
        ///    EditorGUILayout.PropertyField(blockTypes.GetArrayElementAtIndex(i));
        ///}
        ///GUILayout.EndScrollView();
        ///GUILayout.BeginHorizontal();
        ///if (GUILayout.Button("Add Block"))
        ///{
        ///    b.blockTypes.Add(new BlockType("NONE", b.blockTypes.Count));
        ///}
        ///if (GUILayout.Button("Remove Block"))
        ///{
        ///    b.blockTypes.RemoveAt(0);
        ///}
        ///GUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(blockTypes);
        b.ReloadIDs();

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "Modified Block Types");
            serializedObject.ApplyModifiedProperties();
        }


    }
}

#endif
