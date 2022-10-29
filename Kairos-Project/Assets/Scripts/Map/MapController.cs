using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Not Implemented
/// </summary>
public class MapController : MonoBehaviour
{
    public static MapController main;
    public static void Init(MapController controller)
    {
        main = controller;
        PathManager.Init(new PathManager(MapController.main.grid.cellSize.x));
    }

    //GameObjects
    public RTSMap RTS;
    public MiniMap miniMap;
    public StrategicMap strategic;
    public Grid grid;

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
        mapGenerator.OLD_GenerateTerrain();
    }

    /// <summary>
    /// Not Implemented
    /// Implement with 2D map
    /// </summary>
    public Color SampleColor(Vector2 pos)
    {
        return Color.white;
    }
}
