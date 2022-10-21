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
        LoadTerrain();
    }
    void LoadTerrain()
    {
        terrain.terrainData = MapController.main.mapData.terrainData; 
    }


}