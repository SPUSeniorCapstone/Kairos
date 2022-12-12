using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Slider = UnityEngine.UI.Slider;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Not Implemented
/// </summary>
public class MapController : MonoBehaviour
{
    public static MapController main;
    public static void Init(MapController controller)
    {
        main = controller;
        PathManager.Init(new PathManager(main.mapData.cellSizeX));
        main.grid.cellSize = new Vector3(main.mapData.cellSizeX, 1, main.mapData.cellSizeZ);
    }



    public bool showDebugTerrainTexture;

    [Header("Game Objects")]
    //GameObjects
    public RTSMap RTS;
    public MiniMap miniMap;
    public StrategicMap strategic;
    public Grid grid;
    public MapGenerator mapGenerator;
    public Renderer debugDisplayObject;
    [SerializeField] Slider scale;
    [SerializeField] Slider eccentricity;
    [SerializeField] Slider layer_divisor;
    public bool realTime = false;
    public bool OldGeneration = false;

    [Header("Scriptable Objects")]
    //Other Objects
    public MapData mapData;

    public void ScaleChange()
    {
        mapGenerator.settings.scale = scale.value;
        scale.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = scale.value.ToString();
        if (realTime)
        {
            GenerateMap();
        }
    }

    public void EccentricityChange()
    {
        mapGenerator.settings.eccentricity = eccentricity.value;
        eccentricity.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = eccentricity.value.ToString();
        if (realTime)
        {
            GenerateMap();
        }
    }

    public void LayerChange()
    {
        mapGenerator.settings.layerDivider = layer_divisor.value;
        layer_divisor.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = layer_divisor.value.ToString();
        if (realTime)
        {
            GenerateMap();
        }
    }

    public void GenerateMap()
    {
        if (OldGeneration)
        {
            mapGenerator.OLD_GenerateTerrain();
        }
        else
        {
            mapGenerator.GenerateTerrain();
        }
    }

    public void SetRealTime()
    {
        if (!realTime)
        {
            GenerateMap();
        }
        realTime = !realTime;
    }

    float oldEcc;
    public void SetOldGeneration()
    {

        OldGeneration = !OldGeneration;
        if(!OldGeneration)
        {
            if(mapGenerator.settings.eccentricity > 0.1)
            {
                oldEcc = mapGenerator.settings.eccentricity;
                mapGenerator.settings.eccentricity = 0.1f;
            }
        }
        else
        {
            mapGenerator.settings.eccentricity = oldEcc;
        }
        GenerateMap();
    }



    private void Awake()
    {
        Init(this);
        ReloadTerrain();
        // brute force
        if (SceneManager.GetActiveScene().name == "Terrain Demo")
        {
            layer_divisor.value = mapGenerator.settings.layerDivider;
            layer_divisor.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = layer_divisor.value.ToString();
            eccentricity.value = mapGenerator.settings.eccentricity;
            eccentricity.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = eccentricity.value.ToString();
            scale.value = mapGenerator.settings.scale;
            scale.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = scale.value.ToString();
        }

    }

    private void Start()
    {
        GenerateMap();
        ReloadTerrain();
    }

    /// <summary>
    /// Not Implemented
    /// Implement with 2D map
    /// </summary>
    public Color SampleColor(Vector2 pos)
    {
        return Color.white;
    }


    [Button(nameof(SaveMapData))]
    [SerializeField] bool _SaveMapDataButton;
    public void SaveMapData()
    {
        // comment out when editors moved
        //EditorUtility.SetDirty(mapData);
    }

    [SerializeField]
    [Button(nameof(ReloadTerrain))]
    bool ReloadTerrainButton;
    public void ReloadTerrain()
    {
        RTS.LoadMap(mapData);
        miniMap.DrawTerrainMap();
        var display = debugDisplayObject;
        if (showDebugTerrainTexture)
        {
            display.gameObject.SetActive(true);
            display.GetComponent<MapDisplay>().DrawTerrainMap();
        }
        else
        {
            display.gameObject.SetActive(false);
        }
    }
}
