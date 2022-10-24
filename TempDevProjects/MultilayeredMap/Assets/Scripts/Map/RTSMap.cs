using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RTSMap : MonoBehaviour
{
    public Terrain terrain;

    private void Start()
    {
        LoadMap();
    }

    private void Update()
    {
        
    }

    void LoadMap()
    {
        terrain.terrainData = MapController.main.mapData.terrainData;

    }

    /// <summary>
    /// Not Implemented
    /// </summary>
    bool IsValidMovePosition(Vector2 pos)
    {
        return false;
    }

    /// <summary>
    /// Not Implemented
    /// </summary>
    bool IsValidPlacementPosition(Vector2Int pos)
    {
        return false;
    }

    /// <summary>
    /// Not Implemented
    /// </summary>
    float SampleHeight(Vector2 pos)
    {
        return 0;
    }    
}