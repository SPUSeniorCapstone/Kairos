using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Not Implemented
/// </summary>
public class RTSMap : MonoBehaviour
{
    public Terrain terrain;

    private void Awake()
    {
        //LoadMap(MapController.main.mapData);
    }

    private void Update()
    {

    }

    public void LoadMap(MapData map)
    {
        float[,] heightMap = map.GetHeightMap();


        int terrainWidth = heightMap.GetLength(0), terrainLength = heightMap.GetLength(1);

        terrain.terrainData.heightmapResolution = Mathf.Max(terrainWidth, terrainLength);
        terrain.terrainData.size = new Vector3(terrainWidth, map.height, terrainLength);
        terrain.terrainData.SetHeightsDelayLOD(0, 0, heightMap);
        terrain.terrainData.SyncHeightmap();
    }

    /// <summary>
    /// Checks whether tiles are passable or not
    /// </summary>
    /// <param name="pos">Position</param>
    /// <returns>True/False</returns>
    bool IsValidMovePosition(Vector2Int pos)
    {
        if (MapController.main.mapData.tiles[MapController.main.mapData.GetIndex(pos.x,pos.y)].isPassable == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Checks whether tiles are placeable or not
    /// </summary>
    /// <param name="pos">Position</param>
    /// <returns>True/False</returns>
    bool IsValidPlacementPosition(Vector2Int pos)
    {
        if (MapController.main.mapData.tiles[MapController.main.mapData.GetIndex(pos.x, pos.y)].isPlaceable == true)
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
    public float SampleHeight(Vector3 pos)
    {
        return terrain.SampleHeight(pos);
        //return terrain.SampleHeight(new Vector3(pos.x, 0, pos.y));
    }
}
