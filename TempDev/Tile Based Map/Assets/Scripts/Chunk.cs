using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Chunk : MonoBehaviour
{
    public static readonly int width = 64, length = 64;
    public static Chunk CreateChunk(Vector3Int position, Tile[,] tiles, Transform parent, Material material)
    {
        Chunk chunk = Instantiate(WorldController.main.defaultChunk, parent);
        chunk.SetChunk(position, tiles, material);

        return chunk;
    }

    public Vector3Int position;
    public Tile[,] tiles = new Tile[width, length];

    public void SetChunk(Vector3Int position, Tile[,] tiles, Material material)
    {
        this.position = position;
        this.tiles = tiles;
        GetComponent<MeshRenderer>().material = material;
    }

    public void ReloadMesh()
    {
        var meshFilter = GetComponent<MeshFilter>();
        if(meshFilter.sharedMesh == null)
        {
            meshFilter.sharedMesh = new Mesh();
        }

        Vector3[] vertices = new Vector3[width * length * 4];
        int[] triangles = new int[width * length * 6];
        Vector2[] uvs = new Vector2[width * length * 4];

        int i = 0;
        int j = 0;
        for(int x = 0; x < width; x++)
        {
            for(int z = 0; z < length; z++)
            {
                //vertices[i] = new Vector3(x, tiles[x,z].height, z);
                //vertices[i + 1] = new Vector3(x + 1, tiles[x, z].height, z);
                //vertices[i + 2] = new Vector3(x, tiles[x, z].height, z + 1);
                //vertices[i + 3] = new Vector3(x + 1, tiles[x, z].height, z + 1);

                vertices[i] = new Vector3(x, GetAvgHeight(x,z,new Vector2Int(-1, -1)), z);
                vertices[i + 1] = new Vector3(x + 1, GetAvgHeight(x, z, new Vector2Int(1, -1)), z);
                vertices[i + 2] = new Vector3(x, GetAvgHeight(x, z, new Vector2Int(-1, 1)), z + 1);
                vertices[i + 3] = new Vector3(x + 1, GetAvgHeight(x, z, new Vector2Int(1, 1)), z + 1);

                int biomeID = tiles[x, z].biomeID;
                int textureID = WorldController.main.BiomeManager.biomes[biomeID].textureID;
                //TileType tileType = WorldController.main.TileManager.tileTypes[textureID];

                var offset = TileManager.NormalizedBlockTextureSize;
                var tX = textureID % TileManager.TextureAtlasSizeInBlocks;
                var tZ = textureID / TileManager.TextureAtlasSizeInBlocks;
                Vector2 start = new Vector2(tX * offset, tZ * offset);

                uvs[i] = start;
                uvs[i + 1] = new Vector2(start.x + offset, start.y );
                uvs[i + 2] = new Vector2(start.x , start.y + offset);
                uvs[i + 3] = new Vector2(start.x + offset, start.y + offset);

                triangles[j] = i;
                triangles[j + 1] = i + 2;
                triangles[j + 2] = i + 1;
                triangles[j + 3] = i + 1;
                triangles[j + 4] = i + 2;
                triangles[j + 5] = i + 3;

                i += 4;
                j += 6;
            }
        }

        meshFilter.sharedMesh.SetVertices(vertices);
        meshFilter.sharedMesh.SetTriangles(triangles, 0);
        meshFilter.sharedMesh.SetUVs(0, uvs);
        meshFilter.sharedMesh.RecalculateNormals();
        transform.position = new Vector3(position.x * width, 0, position.z * length);
    }

    float GetAvgHeight(int x, int z, Vector2Int dir)
    {
        if(x == width - 1 || x == 0 || z == length - 1 || z == 0)
        {
            //return tiles[x, z].height;
        }

        int X = position.x * width + x;
        int Z = position.z * length + z;

        Tile[] t = new Tile[4];
        t[0] = tiles[x, z];
        t[1] = WorldController.main.World.GetTile(X + dir.x, Z + dir.y);
        t[2] = WorldController.main.World.GetTile(X, Z + dir.y);
        t[3] = WorldController.main.World.GetTile(X + dir.x, Z);

        List<float> heights = new List<float>(4);
        heights.Add(t[0].height);
        if (t[1] != null) heights.Add(t[1].height);
        if (t[2] != null) heights.Add(t[2].height);
        if (t[3] != null) heights.Add(t[3].height);

        return heights.Average();
    }
    
}
