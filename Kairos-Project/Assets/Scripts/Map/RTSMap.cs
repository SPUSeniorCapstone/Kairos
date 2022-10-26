using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Not Implemented
/// </summary>
public class RTSMap : MonoBehaviour
{
    Terrain terrain;

    private void Update()
    {
        LoadMap();
    }

    void LoadMap()
    {
        terrain.terrainData = MapController.main.mapData.terrainData;
    }

    /// <summary>
    /// Checks weather tiles are passable or not
    /// </summary>
    /// <param name="pos">Position</param>
    /// <returns>True/False</returns>
    bool IsValidMovePosition(Vector2Int pos)
    {
        if (MapController.main.mapData.tiles[pos.x,pos.y].isPassable == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Checks weather tiles are placeable or not
    /// </summary>
    /// <param name="pos">Position</param>
    /// <returns>True/False</returns>
    bool IsValidPlacementPosition(Vector2Int pos)
    {
        if (MapController.main.mapData.tiles[pos.x, pos.y].isPlaceable == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Gets the sample height of terrain
    /// </summary>
    /// <param name="pos">Position of tile</param>
    /// <returns>Vector3 of terrain height</returns>
    float SampleHeight(Vector2 pos)
    {
        return terrain.SampleHeight(new Vector3(pos.x, 0, pos.y));
    }
}
