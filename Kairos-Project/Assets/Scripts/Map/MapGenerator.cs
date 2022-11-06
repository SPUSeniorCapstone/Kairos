using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapGenerator
{

    public int width, length;
    [Range(0, 1)]
    public float scale, eccentricity;
    [Range(0, 1)]
    public float layerDivider = 0.5f;
    public float offsetX, offsetZ;

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

        MapController.main.mapData.width = width;
        MapController.main.mapData.length = length;
        MapController.main.mapData.tiles = new MapTile[width, length];

        float cellSizeX = MapController.main.mapData.cellSizeX;
        float cellSizeZ = MapController.main.mapData.cellSizeZ;
        MapController.main.grid.cellSize = new Vector3(cellSizeX, 1, cellSizeZ);

        MapTile[,] tiles = new MapTile[width, length];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                float h = Mathf.PerlinNoise((x + offsetX) * scale, (z + offsetZ) * scale);

                
                if( h > layerDivider)
                {
                    //MapController.main.mapData.tiles[z, x].isPassable = false;

                    //int steps = (int)(0.1f / scale);
                    //if(steps > 5)
                    //{
                    //    steps = 5;
                    //}
                    BlockOffNode(tiles, new Vector2Int(x, z), 5, false, true);

                    //MapController.main.mapData.tiles[z, x].weight = 0;
                    h = 1f;
                    MapController.main.mapData.tiles[z, x].height = h;
                }
                //else if(h < layerDivider && h > layerDivider - 0.1f)
                //{
                //    MapController.main.mapData.tiles[z, x].isPassable = true;
                //    MapController.main.mapData.tiles[z, x].weight = 0.01f;
                //    MapController.main.mapData.tiles[z, x].height = h;
                //    h = 0.5f;
                //}
                else
                {
                    if(!tiles[z, x].isAdjacentToUnpassable)
                    {
                        tiles[z, x].isPassable = true;
                    }
                    tiles[z, x].weight = 1;
                    h = 0;
                    tiles[z, x].height = h;
                }
            }


        }

        MapController.main.mapData.tiles = tiles;
        MapController.main.mapData.ReloadTerrainData();
        MapController.main.SaveMapData();

        var display = MapController.main.debugDisplayObject;
        if (MapController.main.showDebugTerrainTexture)
        {
            display.gameObject.SetActive(true);
            display.GetComponent<MapDisplay>().DrawTerrainMap();
        }
        else
        {
            display.gameObject.SetActive(false);
        }

    }

    void BlockOffNode(MapTile[,] tiles, Vector2Int pos, int dist = 0, bool full = false, bool disable = false)
    {
        int width = MapController.main.mapData.width;
        int length = MapController.main.mapData.length;

        if(pos.y > length - 1 || pos.y < 0 || pos.x > width - 1|| pos.x < 0)
        {
            return;
        }
        if (disable)
        {
            tiles[pos.y, pos.x].isPassable = false;
            tiles[pos.y, pos.x].weight = -1;
            tiles[pos.y, pos.x].isAdjacentToUnpassable = false;
        }
        else
        {
            if(tiles[pos.y, pos.x].weight >= 0)
            {
                tiles[pos.y, pos.x].isPassable = true;
                tiles[pos.y, pos.x].weight = 0.01f;
                tiles[pos.y, pos.x].isAdjacentToUnpassable = true;
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

