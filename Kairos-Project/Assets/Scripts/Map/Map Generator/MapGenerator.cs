using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class MapGenerator : MonoBehaviour
{

    public GeneratorSettings settings;

    MapData GenerateMap()
    {
        return null;
    }

    /// <summary>
    /// Simple terrain Generation using Perlin noise 
    /// Automatically updates the active MapData object
    /// </summary>
    public void OLD_GenerateTerrain()
    {

        MapController.main.mapData.width = settings.width;
        MapController.main.mapData.length = settings.length;
        MapController.main.mapData.tiles = new MapTile[settings.width * settings.length];

        float cellSizeX = MapController.main.mapData.cellSizeX;
        float cellSizeZ = MapController.main.mapData.cellSizeZ;
        MapController.main.grid.cellSize = new Vector3(cellSizeX, 1, cellSizeZ);

        MapTile[] tiles = new MapTile[settings.width * settings.length];

        for (int x = 0; x < settings.width; x++)
        {
            for (int z = 0; z < settings.length; z++)
            {
                float h = Mathf.PerlinNoise((x + settings.offsetX) * settings.scale, (z + settings.offsetZ) * settings.scale);


                if (h > settings.layerDivider)
                {
                    BlockOffNode(tiles, new Vector2Int(x, z), settings.blockOffDepth, false, true);
                    h = 1f;
                }
                else
                {
                    if (!tiles[MapController.main.mapData.GetIndex(x, z)].isAdjacentToUnpassable)
                    {
                        tiles[MapController.main.mapData.GetIndex(x, z)].weight = 1;
                    }
                    else
                    {
                        tiles[MapController.main.mapData.GetIndex(x, z)].weight = 0.5f;
                    }
                    tiles[MapController.main.mapData.GetIndex(x, z)].isPassable = true;
                    h = 0;
                }


                tiles[MapController.main.mapData.GetIndex(x, z)].height = h * settings.eccentricity;
            }


        }

        MapController.main.mapData.tiles = tiles;
        MapController.main.ReloadTerrain();
        MapController.main.SaveMapData();
    }

    void BlockOffNode(MapTile[] tiles, Vector2Int pos, int dist = 0, bool full = false, bool disable = false)
    {
        int width = MapController.main.mapData.width;
        int length = MapController.main.mapData.length;

        if (pos.x > width - 1 || pos.x < 0 || pos.y > length - 1 || pos.y < 0)
        {
            return;
        }
        if (disable)
        {
            tiles[MapController.main.mapData.GetIndex(pos.x, pos.y)].isPassable = false;
            tiles[MapController.main.mapData.GetIndex(pos.x, pos.y)].weight = -1;
            tiles[MapController.main.mapData.GetIndex(pos.x, pos.y)].isAdjacentToUnpassable = false;
        }
        else
        {
            if (tiles[MapController.main.mapData.GetIndex(pos.x, pos.y)].weight >= 0)
            {
                tiles[MapController.main.mapData.GetIndex(pos.x, pos.y)].isAdjacentToUnpassable = true;
            }
            else
            {
                return;
            }
        }
        if (dist == 0)
        {
            return;
        }



        BlockOffNode(tiles, pos + Vector2Int.up, dist - 1, full);
        BlockOffNode(tiles, pos + Vector2Int.down, dist - 1, full);
        BlockOffNode(tiles, pos + Vector2Int.left, dist - 1, full);
        BlockOffNode(tiles, pos + Vector2Int.right, dist - 1, full);

        if (full)
        {
            BlockOffNode(tiles, pos + Vector2Int.one, dist - 1, full);
            BlockOffNode(tiles, pos - Vector2Int.one, dist - 1, full);
            BlockOffNode(tiles, pos + Vector2Int.left + Vector2Int.up, dist - 1, full);
            BlockOffNode(tiles, pos + Vector2Int.right + Vector2Int.down, dist - 1, full);
        }

    }
}

