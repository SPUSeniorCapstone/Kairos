using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MultilayeredMap : MonoBehaviour
{
    MapLayer Ground;
    MapLayer Sky;

    Grid grid;
    Terrain terrain;

    [Range(0, 1)]
    public float scale = 1;
    [Range (0, 1)]
    public float eccentricity = 0.1f;

    float pScale;
    float pEccentricity;

    private void Start()
    {
        grid = GetComponent<Grid>();
        terrain = GetComponent<Terrain>();

        pScale = scale;
        pEccentricity = eccentricity;
        LoadTerrain();
    }

    private void Update()
    {
        if(pScale != scale || pEccentricity != eccentricity)
        {
            LoadTerrain();
            pScale = scale;
            pEccentricity = eccentricity;
        }
    }

    void LoadTerrain()
    {


        int resolution = Terrain.activeTerrain.terrainData.heightmapResolution;
        Debug.Log(resolution);
        float[,] tree = new float[resolution, resolution];
        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                float h = Mathf.PerlinNoise(x * scale, y * scale) * eccentricity;
                //if(h > 0.5f)
                //{
                //    h = 10;
                //}else
                //{
                //    h = 0;
                //}
                tree[x, y] = h;
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