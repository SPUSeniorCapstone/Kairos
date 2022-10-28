using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Not Implemented
/// </summary>
[Serializable]
public class MapData
{
    public TerrainData terrainData;
    public int width;
    public int length;
    public MapTile[,] tiles;


    public MapData(TerrainData terrainData, MapTile[,] tiles)
    {
        this.terrainData = terrainData;
        this.width = tiles.GetLength(0);
        this.length = tiles.GetLength(1);
        this.tiles = tiles;
    }

    public MapData(MapTile[,] tiles)
    {
        this.width = tiles.GetLength(0);
        this.length = tiles.GetLength(1);
        this.tiles = tiles;
    }

    public MapData(int width, int length)
    {
        tiles = new MapTile[width, length];
    }

    public MapData(int width, int length, TerrainData terrainData)
    {
        this.width = width;
        this.length = length;
        tiles = new MapTile[width, length];

        this.terrainData = terrainData;
    }




    //Non-Player Structures

    /// <summary>
    /// Flattens a rectangle to a given height
    /// </summary>
    /// <param name="bottemLeft">Bottem left coordinates of rectangle</param>
    /// <param name="topRight">Top right coordinates of rectangle</param>
    /// <param name="height">Desired height</param>
    public void Flatten(Vector2Int bottemLeft, Vector2Int topRight, float height)
    {
        int width = topRight.x - bottemLeft.x;
        int length = topRight.y - bottemLeft.y;
        float[,] newHeights = new float[length, width];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < length; y++)
            {
                newHeights[y, x] = height;
            }
        }
        terrainData.SetHeightsDelayLOD(bottemLeft.x, bottemLeft.y, newHeights);
    }

    /// <summary>
    /// Gets the tile height
    /// </summary>
    /// <param name="pos">Position of tile</param>
    /// <returns>Tile height</returns>
    public float SampleHeight(Vector2Int pos)
    {
        return tiles[pos.x, pos.y].height;
    }
}