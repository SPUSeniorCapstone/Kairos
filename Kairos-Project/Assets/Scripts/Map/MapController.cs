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

    public MapData mapData;
    public RTSMap RTS;
    public MiniMap miniMap;
    public StrategicMap strategic;

    /// <summary>
    /// Not Implemented
    /// Implement with 2D map
    /// </summary>
    public Color SampleColor(Vector2 pos)
    {
        return Color.white;
    }
}
