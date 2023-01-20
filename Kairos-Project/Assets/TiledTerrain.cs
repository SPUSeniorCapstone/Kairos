using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TiledTerrain : MonoBehaviour
{
    public MapData map;

    public float height = 50;
    MeshFilter meshFilter;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();

    }

    [SerializeField]
    [Button("LoadMap")]
    bool BUTTON_LoadMap = false;
    public void LoadMap()
    {
        Debug.Log("Loading Map");

        if(meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
        }

        int vWidth = map.width + 1;
        int vLength = map.length + 1;

        Vector3[] vertices = new Vector3[vWidth * vLength];
        int[] triangles = new int[map.width * map.length * 6];
        int k = 0;
        for(int z = 0; z < vLength; z++)
        {
            for(int x = 0; x < vWidth; x++)
            {
                float h = 0;
                if(x != vWidth - 1 && z != vLength - 1)
                {
                    h = map.SampleHeight(new Vector2Int(x, z));
                }
                vertices[k] = new Vector3(x, h * height, z);
                k++;
            }
        }

        int j = 0;
        for(int i = 0; i < map.width * map.length; i++)
        {


            int tl = j;
            int tr = tl + 1;
            int bl = tr + map.length;
            int br = bl + 1;

            int index = i * 6;
            triangles[index] = tl;
            triangles[index + 1] = bl;
            triangles[index + 2] = tr;

            triangles[index + 3] = tr;
            triangles[index + 4] = bl;
            triangles[index + 5] = br;

            j++;
            if ((i + 1) % map.width == 0)
            {
                j++;
            }
        }

        /**
        //int numVertices = (map.width + 1) * (map.length + 1);
        //int numTriangles = map.width * map.length * 2;


        //Vector3[] vertices = new Vector3[numVertices];
        //int[] triangles = new int[numTriangles * 3];
        //for(int z = 0; z < map.length + 1; z++)
        //{
        //    for(int x = 0; x < map.width + 1; x++)
        //    {
        //        int index = (z * (map.length)) + x;
        //        vertices[index] = new Vector3(x, 0, z);
        //    }
        //}
        ////map.SampleHeight(new Vector2Int(x / 2, z / 2))

        //for (int z = 0; z < map.length; z++)
        //{
        //    for (int x = 0; x < map.width; x++)
        //    {
        //        int i = ((z * map.length) + x);

        //        int index = i * 3;
        //        //i = i + x;
        //        triangles[index + 0] = i;
        //        triangles[index + 1] = i + map.length + 1;
        //        triangles[index + 2] = i + 1;

        //        triangles[index + 3] = i + 1;
        //        triangles[index + 4] = i + map.length + 1;
        //        triangles[index + 5] = i + map.length + 2;

        //    }
        //}

        ////for (int i = 0; i < numTriangles/2; i++)
        ////{
        ////    if((i + 1) % (map.length + 1) != 0)
        ////    {
        ////        int index = i * 3;
        ////        triangles[index + 0] = i + map.length + 1;
        ////        triangles[index + 1] = i + 1;
        ////        triangles[index + 2] = i;

        ////        triangles[index + 3] = i + map.length + 1;
        ////        triangles[index + 4] = i + map.length + 2;
        ////        triangles[index + 5] = i + 1;
        ////    }
        ////}

        ////mesh.SetVertices(vertices);
        ///
        */
        meshFilter.sharedMesh = new Mesh();
        meshFilter.sharedMesh.SetVertices(vertices);
        meshFilter.sharedMesh.SetTriangles(triangles, 0);
        meshFilter.sharedMesh.RecalculateNormals();
    }
}
