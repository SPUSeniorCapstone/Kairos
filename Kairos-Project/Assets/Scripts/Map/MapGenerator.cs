using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapGenerator
{
    public int terrainResolution;
    public float cellSize;

    [Range(0, 1)]
    public float scale, eccentricity;
    [Range(0, 1)]
    public float layerDivider = 0.5f;
    public int seed;

    MapData GenerateMap()
    {
        return null;
    }
}
