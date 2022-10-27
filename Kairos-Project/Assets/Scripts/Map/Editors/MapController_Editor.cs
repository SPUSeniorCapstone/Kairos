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

    private void OnEnable()
    {
        MapController.Init(target as MapController);

        var map = target as MapController;

        terrainResolution = map.mapGenerator.terrainResolution;
        cellSize = map.mapGenerator.cellSize;
        scale = map.mapGenerator.scale;
        eccentricity = map.mapGenerator.eccentricity;
        layerDivider = map.mapGenerator.layerDivider;
        seed = map.mapGenerator.seed;

        if(CheckChanges != null)
        {
            CheckChanges.Dispose();
            CheckChanges = null;
        }
        CheckChanges = CheckForChanges();
        
    }

    public int terrainResolution;
    public float cellSize;
    public float scale, eccentricity;
    public float layerDivider;
    public int seed;

    public async Task CheckForChanges()
    {
        var map = target as MapController;

        while (true)
        {
            if (GenerateOnUpdate)
            {
                if (
                terrainResolution != map.mapGenerator.terrainResolution ||
                cellSize != map.mapGenerator.cellSize ||
                scale != map.mapGenerator.scale ||
                eccentricity != map.mapGenerator.eccentricity ||
                layerDivider != map.mapGenerator.layerDivider ||
                seed != map.mapGenerator.seed
                )
                {
                    terrainResolution = map.mapGenerator.terrainResolution;
                    cellSize = map.mapGenerator.cellSize;
                    scale = map.mapGenerator.scale;
                    eccentricity = map.mapGenerator.eccentricity;
                    layerDivider = map.mapGenerator.layerDivider;
                    seed = map.mapGenerator.seed;

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

        if (GUILayout.Button("Generate Map"))
        {
            map.mapGenerator.OLD_GenerateTerrain();
        }
        else if (GenerateOnUpdate)
        {
            //if (
            //    terrainResolution != map.mapGenerator.terrainResolution ||
            //    cellSize != map.mapGenerator.cellSize ||
            //    scale != map.mapGenerator.scale ||
            //    eccentricity != map.mapGenerator.eccentricity ||
            //    layerDivider != map.mapGenerator.layerDivider ||
            //    seed != map.mapGenerator.seed
            //    )
            //{
            //    terrainResolution = map.mapGenerator.terrainResolution;
            //    cellSize = map.mapGenerator.cellSize;
            //    scale = map.mapGenerator.scale;
            //    eccentricity = map.mapGenerator.eccentricity;
            //    layerDivider = map.mapGenerator.layerDivider;
            //    seed = map.mapGenerator.seed;

            //    map.mapGenerator.OLD_GenerateTerrain();
            //}
        }

        GenerateOnUpdate = EditorGUILayout.Toggle(GenerateOnUpdate);

    }
}
