using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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


    [Header("Scriptable Objects")]
    //Other Objects
    public MapData mapData;

    


    private void Awake()
    {
        Init(this);
        ReloadTerrain();
    }

    private void Start()
    {
        //ReloadTerrain();
    }

    /// <summary>
    /// Not Implemented
    /// Implement with 2D map
    /// </summary>
    public Color SampleColor(Vector2 pos)
    {
        return Color.white;
    }

    public void SaveMapData()
    {
        EditorUtility.SetDirty(mapData);
    }

    [SerializeField]
    [Button(nameof(ReloadTerrain))]
    bool ReloadTerrainButton;
    public void ReloadTerrain()
    {
        RTS.LoadMap(mapData);
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
