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

    //GameObjects
    public RTSMap RTS;
    public MiniMap miniMap;
    public StrategicMap strategic;
    public Grid grid;

    public Renderer debugDisplayObject;

    //Other Objects
    public MapData mapData;
    public MapGenerator mapGenerator;


    private void Awake()
    {
        Init(this);
        //mapData.tiles = new MapTile[mapData.width, mapData.length];
    }

    private void Start()
    {
        //mapGenerator.OLD_GenerateTerrain();
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
        if(mapData == null)
        {
            AssetDatabase.CreateAsset(mapData, "Assets/mapdata.asset");
        }
        else
        {
            AssetDatabase.SaveAssets();
        }
    }
}
