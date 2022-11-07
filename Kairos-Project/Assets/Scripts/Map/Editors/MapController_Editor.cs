using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;

[CustomEditor(typeof(MapController))]
public class MapController_Editor : Editor
{
    bool GenerateOnUpdate = false;

    Task CheckChanges;

    bool FlattenSquare = false;

    Vector2Int BottomLeft, TopRight;

    private void OnEnable()
    {
        MapController.Init(target as MapController);

        var map = target as MapController;

        scale = map.mapGenerator.settings.scale;
        eccentricity = map.mapGenerator.settings.eccentricity;
        layerDivider = map.mapGenerator.settings.layerDivider;
        offsetX = map.mapGenerator.settings.offsetX;
        offsetZ = map.mapGenerator.settings.offsetZ;

        if (CheckChanges != null)
        {
            CheckChanges.Dispose();
            CheckChanges = null;
        }
        CheckChanges = CheckForChanges();

        FlattenSquare = false;
        
    }

    public float scale, eccentricity;
    public float layerDivider;
    public float offsetX, offsetZ;

    public async Task CheckForChanges()
    {
        var map = target as MapController;

        while (true)
        {
            if (GenerateOnUpdate)
            {
                if (
                scale != map.mapGenerator.settings.scale ||
                eccentricity != map.mapGenerator.settings.eccentricity ||
                layerDivider != map.mapGenerator.settings.layerDivider ||
                offsetX != map.mapGenerator.settings.offsetX ||
                offsetZ != map.mapGenerator.settings.offsetZ
                )
                {
                    scale = map.mapGenerator.settings.scale;
                    eccentricity = map.mapGenerator.settings.eccentricity;
                    layerDivider = map.mapGenerator.settings.layerDivider;
                    offsetX = map.mapGenerator.settings.offsetX;
                    offsetZ = map.mapGenerator.settings.offsetZ;

                    map.mapGenerator.OLD_GenerateTerrain();
                }
            }
            await Task.Delay(10);
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var map = target as MapController;

        if (!FlattenSquare)
        {
            if (GUILayout.Button("Begin Flatten Square"))
            {
                FlattenSquare = true;
                BottomLeft = Vector2Int.zero;
                TopRight = Vector2Int.one;
            }
        }

        if (FlattenSquare)
        {
            if (GUILayout.Button("Flatten"))
            {
                MapController.main.mapData.Flatten(BottomLeft, TopRight, 0);
                FlattenSquare = false;
                BottomLeft = Vector2Int.zero;
                TopRight = Vector2Int.one;
            }
        }


        if (GUILayout.Button("Generate Map"))
        {
            if(map.mapData == null)
            {
                Debug.LogError("missing mapdata");
            }
            else
            {
                map.mapGenerator.OLD_GenerateTerrain();
            }
        }

        GenerateOnUpdate = EditorGUILayout.Toggle(GenerateOnUpdate);

        if (GUILayout.Button("Generate Debug Map"))
        {
            FindObjectOfType<MapDisplay>().DrawTerrainMap();
        }

        if (GUILayout.Button("Save Map"))
        {
            map.SaveMapData();
        }
    }

    private void OnSceneGUI()
    {
        if (FlattenSquare)
        {
            var BL = Handles.FreeMoveHandle(new Vector3(BottomLeft.x, 1, BottomLeft.y), Quaternion.identity, 0.5f, Vector3.one, Handles.CubeHandleCap);

            var TR = Handles.FreeMoveHandle(new Vector3(TopRight.x, 1, TopRight.y), Quaternion.identity, 0.5f, Vector3.one, Handles.CubeHandleCap);
            
            if(BL.x > TR.x)
            {
                BL.x = TR.x - 1;
            }
            if(BL.z > TR.z)
            {
                BL.z = TR.z - 1;
            }

            if (TR.x < BL.x)
            {
                TR.x = BL.x + 1;
            }
            if (TR.z < BL.z)
            {
                TR.z = BL.z + 1;
            }

            BottomLeft = Helpers.ToVector2Int(new Vector2(BL.x, BL.z));
            TopRight = Helpers.ToVector2Int(new Vector2(TR.x, TR.z));

            Vector2Int bottomRight = BottomLeft;
            bottomRight.x = TopRight.x;

            Vector2Int topLeft = BottomLeft;
            topLeft.y = TopRight.y;

            Vector3[] lines =
            {
                Helpers.Vector2IntToVector3(BottomLeft),
                Helpers.Vector2IntToVector3(bottomRight),
                Helpers.Vector2IntToVector3(bottomRight),
                Helpers.Vector2IntToVector3(TopRight),
                Helpers.Vector2IntToVector3(TopRight),
                Helpers.Vector2IntToVector3(topLeft),
                Helpers.Vector2IntToVector3(topLeft),
                Helpers.Vector2IntToVector3(BottomLeft)
            };

            Handles.DrawLines(lines);
        }
    }
}
