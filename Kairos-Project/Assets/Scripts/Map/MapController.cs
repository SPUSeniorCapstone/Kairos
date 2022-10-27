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
    }

    //GameObjects
    public RTSMap RTS;
    public MiniMap miniMap;
    public StrategicMap strategic;

    //Other Objects
    public MapData mapData;
    public MapGenerator mapGenerator;



    private void Start()
    {
        Init(this);
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
