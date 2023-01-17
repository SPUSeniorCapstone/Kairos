using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class Tile : MonoBehaviour
{
    public static Tile CreateTile(Transform parent, TileInfo info, Vector3[] vertices)
    {
        if(vertices.Length != 4)
        {
            Debug.LogError("Invalid Vertex Count");
            return null;
        }

        int[] triangles = { 2, 1, 0, 3, 1, 2 };

        if(Map.main == null)
        {
            Map.main = FindObjectOfType<Map>();
        }

        Tile tile = Instantiate(Map.main.defaultTile, parent);

        tile.Mesh.sharedMesh = new Mesh();

        tile.Mesh.sharedMesh.SetVertices(vertices);
        tile.Mesh.sharedMesh.SetTriangles(triangles, 0);

        return tile;
    }

    public MeshFilter Mesh
    {
        get
        {
            if(mesh == null)
            {
                mesh = GetComponent<MeshFilter>();
            }
            return mesh;
        }
    }
    MeshFilter mesh;


    TileInfo tileInfo;
}
