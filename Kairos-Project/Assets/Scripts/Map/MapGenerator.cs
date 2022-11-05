using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapGenerator
{
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
        if(MapController.main.mapData.terrainData == null)
        {
            return;
        }

        float cellSizeX = MapController.main.mapData.cellSizeX;
        float cellSizeZ = MapController.main.mapData.cellSizeZ;

        MapController.main.grid.cellSize = new Vector3(cellSizeX, 1, cellSizeZ);

        int width = MapController.main.mapData.width * (int)cellSizeX;
        int length = MapController.main.mapData.length * (int)cellSizeZ;
        MapController.main.mapData.tiles = new MapTile[length, width];

        TerrainData terrainData = MapController.main.mapData.terrainData;
        terrainData.heightmapResolution = Mathf.Max(width, length);
        terrainData.size = new Vector3(width, 100, length);

        var T = terrainData.GetAlphamaps(0, 0, width, length);
 

        float[,] heightMap = new float[width, length];
        float[,] tree = new float[width, length];

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
                    BlockOffNode(new Vector2Int(x, z), 5, false, true);

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
                    if(!MapController.main.mapData.tiles[z, x].isAdjacentToUnpassable)
                    {
                        MapController.main.mapData.tiles[z, x].isPassable = true;
                    }
                    MapController.main.mapData.tiles[z, x].weight = 1;
                    h = 0;
                    MapController.main.mapData.tiles[z, x].height = h;
                }

                heightMap[x, z] = h * eccentricity;
            }


        }

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                tree[x, z] = heightMap[(int)(x / cellSizeX), (int)(z / cellSizeZ)];
            }
        }
        terrainData.SetHeightsDelayLOD(0, 0, tree);
        terrainData.SyncHeightmap();

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

    void BlockOffNode(Vector2Int pos, int dist = 0, bool full = false, bool disable = false)
    {
        int width = MapController.main.mapData.width;
        int length = MapController.main.mapData.length;

        if(pos.y > length - 1 || pos.y < 0 || pos.x > width - 1|| pos.x < 0)
        {
            return;
        }
        if (disable)
        {
            MapController.main.mapData.tiles[pos.y, pos.x].isPassable = false;
            MapController.main.mapData.tiles[pos.y, pos.x].weight = -1;
            MapController.main.mapData.tiles[pos.y, pos.x].isAdjacentToUnpassable = false;
        }
        else
        {
            if(MapController.main.mapData.tiles[pos.y, pos.x].weight >= 0)
            {
                MapController.main.mapData.tiles[pos.y, pos.x].isPassable = true;
                MapController.main.mapData.tiles[pos.y, pos.x].weight = 0.01f;
                MapController.main.mapData.tiles[pos.y, pos.x].isAdjacentToUnpassable = true;
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



        BlockOffNode(pos + Vector2Int.up, dist - 1, full);
        BlockOffNode(pos + Vector2Int.down, dist - 1, full);
        BlockOffNode(pos + Vector2Int.left, dist - 1, full);
        BlockOffNode(pos + Vector2Int.right, dist - 1, full);

        if (full)
        {
            BlockOffNode(pos + Vector2Int.one, dist - 1, full);
            BlockOffNode(pos - Vector2Int.one, dist - 1, full);
            BlockOffNode(pos + Vector2Int.left + Vector2Int.up, dist - 1, full);
            BlockOffNode(pos + Vector2Int.right + Vector2Int.down, dist - 1, full);
        }

    }
}

