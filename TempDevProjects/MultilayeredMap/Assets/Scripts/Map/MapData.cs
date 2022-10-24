using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class MapData
{
    public TerrainData terrainData;

    //Non-Player Structures

    /// <summary>
    /// Not Implemented
    /// </summary>
    public void Flatten(Vector2Int pos)
    {

    }

    /// <summary>
    /// Not Implemented
    /// </summary>
    public float SampleHeight(Vector2Int pos)
    {
        return 0;
    }

    /// <summary>
    /// Not Implemented
    /// </summary>
    public float SampleHeight(Vector2 pos)
    {
        return 0;
    }
}
