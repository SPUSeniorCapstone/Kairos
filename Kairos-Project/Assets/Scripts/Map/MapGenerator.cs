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

        int width = MapController.main.mapData.width;
        int length = MapController.main.mapData.length;
        MapController.main.mapData.tiles = new MapTile[length, width];
        float cellSizeX = MapController.main.grid.cellSize.x;
        float cellSizeZ = MapController.main.grid.cellSize.z;

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
                if (h > layerDivider)
                {
                    h = 1f;
                    MapController.main.mapData.tiles[z, x].isPassable = false;
                }
                else if (h < layerDivider - 0.05f)
                {
                    h = 0;
                    MapController.main.mapData.tiles[z, x].isPassable = true;
                }
                else
                {
                    MapController.main.mapData.tiles[z, x].isPassable = false;
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

    }
}

