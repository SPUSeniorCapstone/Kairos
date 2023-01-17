using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Grid), typeof(TileMap))]
public class Map : MonoBehaviour
{
    public static Map main;


    public Tile defaultTile;

    [Range(10, 10000)]
    public int width, length; //width=x, length=z 

    public Grid Grid
    {
        get
        {
            if(grid == null)
            {
                grid = GetComponent<Grid>();
            }
            return grid;
        }
    }
    Grid grid;

    public TileMap TileMap
    {
        get
        {
            if (tileMap == null)
            {
                tileMap = GetComponent<TileMap>();
            }
            return tileMap;
        }
    }
    TileMap tileMap;

    private void Awake()
    {
        main = this;
    }

    private void Start()
    {

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);

            i++;
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);
    }

    [SerializeField]
    [Button("GenerateMap")]
    bool Button_GenerateMap = false;
    public void GenerateMap()
    {
        //foreach(GameObject g in transform.GetComponentsInChildren<GameObject>())
        //{
        //    DestroyImmediate(g);
        //}

        float[,] heightMap = new float[width,length]; 

        for(int x = 0; x < width; x++)
        {
            for(int z = 0; z < length; z++)
            {
                heightMap[x,z] = Mathf.PerlinNoise(x,z);

            }
        }

        TileMap.LoadTileMeshesFromHeightMap(heightMap);
    }

}
