using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using JetBrains.Annotations;

/// <summary>
/// Not Implemented
/// </summary>
[CreateAssetMenu(fileName = "mapdata", menuName = "ScriptableObjects/MapData", order = 1)]
public class MapData : ScriptableObject 
{
    public int width = 128;
    public int length = 128;
    public int height = 100;
    public int cellSizeX = 1, cellSizeZ = 1;
    public MapTile[] tiles;

    public int GetIndex(int x, int y)
    {
        return (y * length) + x; 
    }

    public float[,] GetHeightMap()
    {
        int terrainWidth = width * cellSizeX, terrainLength = length * cellSizeZ;

        float[,] heightMap = new float[terrainWidth, terrainLength];
        for (int x = 0; x < terrainWidth - 1; x++)
        {
            for (int z = 0; z < terrainLength - 1; z++)
            {
                heightMap[x, z] = tiles[GetIndex( x / cellSizeX, z / cellSizeZ)].height;
            }
        }
        return heightMap;
    }

    public void MarkOffPassability(Vector2Int pos, int width, int length, bool isPassable)
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < length; y++)
            {
                tiles[GetIndex(pos.y + y, pos.x + x)].isPassable = isPassable;
            }
        }
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
                tiles[GetIndex(y, x)].height = height;
            }
        }

    }

    /// <summary>
    /// Gets the tile height
    /// </summary>
    /// <param name="pos">Position of tile</param>
    /// <returns>Tile height</returns>
    public float SampleHeight(Vector2Int pos)
    {
        return tiles[GetIndex(pos.x, pos.y)].height;
    }
}