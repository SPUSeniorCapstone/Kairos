using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public static TerrainData defaultTerrain;

    public static MapController main;
    public static void Init(MapController controller)
    {
        main = controller;
        MapData mapData = new MapData();
        mapData.terrainData = defaultTerrain;
    }


    public MapData mapData;
    public MapGenerator mapGenerator;
    
    //GameObjects
    public RTSMap map;



    #region UnityMessages

    private void Awake()
    {
        Init(this);
    }

    #endregion

    public void GenerateMap()
    {
        mapData.terrainData = mapGenerator.GenerateTerrainData();
        map.terrain.terrainData = mapData.terrainData;
    }

    public bool IsValidMovePosition(Vector3 position)
    {
        return false;
    }
    public bool IsValidBuildPosition(Vector3 position)
    {
        return false;
    }
}
