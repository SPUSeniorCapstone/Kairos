using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Chunk : MonoBehaviour
{
    public const int width = 16, height = 32, length = 16;

    public static Chunk CreateChunk(Vector3Int position, Block[,,] voxels, Transform parent, Material material)
    {
        if(voxels.GetLength(0) != width || voxels.GetLength(1) != height || voxels.GetLength(2) != length)
        {
            Debug.LogError("Cannot create chunk that doesn't conform to chunk width of " + width + " height of " + height + " and length of " + length);
            return null;
        }

        Chunk chunk = Instantiate(WorldController.main.defaultChunk, parent);
        chunk.SetChunk(position, voxels, material);

        return chunk;
    }

    //Fields
    Vector3Int position;
    public Block[,,] voxels = new Block[width, height, length];

    

    //Methods

    /// <summary>
    /// Gets the top level (non-air) voxel at the given coordinate
    /// </summary>
    /// <returns></returns>
    public Block GetSurfaceVoxel(int x, int z)
    {
        Block v = voxels[x,0,z];
        for(int i = 1; i < height; i++)
        {
            if(v.blockID == 0)
            {
                break;
            }
            v = voxels[x, i, z];            
        }
        return v;
    }

    public void SetChunk(Vector3Int position, Block[,,] voxels, Material material)
    {
        if (voxels.GetLength(0) != width || voxels.GetLength(1) != height || voxels.GetLength(2) != length)
        {
            Debug.LogError("Cannot create chunk that doesn't conform to chunk width of " + width + " height of " + height + " and length of " + length);
            return;
        }

        this.position = position;
        this.voxels = voxels;
        GetComponent<MeshRenderer>().material = material;
    }

    public void ReloadMesh()
    {
        var meshFilter = GetComponent<MeshFilter>();
        if (meshFilter.sharedMesh == null)
        {
            meshFilter.sharedMesh = new Mesh();
        }

        int index = 0;
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for(int z = 0; z < length; z++)
                {
                    DrawVoxel(new Vector3Int(x, y, z), voxels[x, y, z].blockID);
                }
            }
        }

        Mesh mesh = new Mesh();

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();

        meshFilter.sharedMesh = mesh;

        transform.position = new Vector3(position.x * width, 0, position.z * length);

        void DrawVoxel(Vector3Int pos, int blockID)
        {
            if(blockID == 0)
            {
                return;
            }
            ///
            /// 
            /// Help from https://github.com/b3agz/Code-A-Game-Like-Minecraft-In-Unity/blob/master/01-the-first-voxel/Assets/Scripts/Chunk.cs
            for(int i = 0; i < 6; i++)
            {
                if (!CheckVoxel(pos + VoxelData.faceChecks[i]))
                {
                    Vector2 UVOffsett = BlockManager.Main.GetBlockUVOffset(blockID, i);

                    vertices.Add(pos + VoxelData.Vertices[VoxelData.Triangles[i, 0]]);
                    vertices.Add(pos + VoxelData.Vertices[VoxelData.Triangles[i, 1]]);
                    vertices.Add(pos + VoxelData.Vertices[VoxelData.Triangles[i, 2]]);
                    vertices.Add(pos + VoxelData.Vertices[VoxelData.Triangles[i, 3]]);
                    uvs.Add((VoxelData.UVs[0] * TextureAtlasManager.NormalizedBlockTextureSize) + UVOffsett);
                    uvs.Add((VoxelData.UVs[1] * TextureAtlasManager.NormalizedBlockTextureSize) + UVOffsett);
                    uvs.Add((VoxelData.UVs[2] * TextureAtlasManager.NormalizedBlockTextureSize) + UVOffsett);
                    uvs.Add((VoxelData.UVs[3] * TextureAtlasManager.NormalizedBlockTextureSize) + UVOffsett);
                    triangles.Add(index);
                    triangles.Add(index + 1);
                    triangles.Add(index + 2);
                    triangles.Add(index + 2);
                    triangles.Add(index + 1);
                    triangles.Add(index + 3);
                    index += 4;
                }
            }
        }
    }

    public Block GetVoxel(Vector3Int pos)
    {
        if(pos.x > width || pos.y > height || pos.z > length)
        {
            Debug.LogError("Invalid Chunk Position");
            return new Block(-1, Vector3Int.zero);
        }
        return voxels[pos.x, pos.y, pos.z];
    }

    bool CheckVoxel(Vector3Int pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (x < 0 || x > width - 1 || y < 0 || y > height - 1 || z < 0 || z > length - 1)
            return false;

        if (voxels[x,y,z].blockID == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


}
