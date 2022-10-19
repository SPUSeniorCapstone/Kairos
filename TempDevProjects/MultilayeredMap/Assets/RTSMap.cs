using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RTSMap : MonoBehaviour
{
    MapLayer Ground;
    MapLayer Sky;

    Grid grid;
    public Terrain terrain;

    public TerrainData terrainData;

    [Range(0, 1)]
    public float scale = 1;
    [Range (0, 1)]
    public float eccentricity = 0.1f;
    [Range(0, 1)]
    public float layerDivider = 0.1f;

    public float gridScale = 100;

    public int seed = 0;

    float pScale;
    float pEccentricity;
    float pLayerDivider;
    float pGridScale;
    float pSeed;

    private void Awake()
    {
    }

    private void Start()
    {
        grid = GetComponent<Grid>();

        pScale = scale;
        pEccentricity = eccentricity;
        pLayerDivider = layerDivider;
        pGridScale = gridScale;
        pSeed = seed;
        LoadTerrain();
    }

    private void Update()
    {
        if(pScale != scale || pEccentricity != eccentricity || layerDivider != pLayerDivider || pGridScale != gridScale || pSeed != seed)
        {
            LoadTerrain();
            pScale = scale;
            pEccentricity = eccentricity;
            pLayerDivider= layerDivider;
            pGridScale = gridScale;
            pSeed = seed;
        }
    }

    void LoadTerrain()
    {
        if(terrainData != null)
        {
            Terrain.activeTerrain.terrainData = terrainData;
            return;
        }

        int resolution = Terrain.activeTerrain.terrainData.heightmapResolution;
        int gridSize = (int)(resolution / gridScale) + 1;
        float[,] heightMap = new float[gridSize, gridSize];
        Debug.Log(resolution);
        float[,] tree = new float[resolution, resolution];

        for(int x = 0; x < gridSize; x++)
        {
            for(int y = 0; y < gridSize; y++)
            {
                float h = Mathf.PerlinNoise((x + seed) * scale, (y + seed) * scale);
                if (h > layerDivider)
                {
                    h = 1f;
                }
                else if ( h < layerDivider - 0.05f)
                {
                    h = 0;
                }
                heightMap[x, y] = h * eccentricity;
            }
        }

        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                
                tree[x, y] = heightMap[(int)(x / gridScale), (int)(y / gridScale)];
            }
        }
        Terrain.activeTerrain.terrainData.SetHeightsDelayLOD(0, 0, tree);
        Terrain.activeTerrain.terrainData.SyncHeightmap();
        
    }
}

public class MapLayer
{
    Dictionary<Vector2Int, MapTile> tiles;
}

public struct MapTile
{
    /// <summary>
    /// EMPTY - Structures and Units can use this tile
    /// TRAVERSABLE - Units can traverse this tile but structures cannot be placed
    /// BLOCKED - Neither Structures nor Units can use this tile
    /// </summary>
    public enum TileState
    {
        EMPTY,
        TRAVERSABLE,
        BLOCKED
    }
    public Vector2Int position;
    public TileState state;
}