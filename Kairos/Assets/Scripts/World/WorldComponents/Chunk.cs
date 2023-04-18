using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

/// <summary>
/// A 3D World Voxel Chunk
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Chunk : MonoBehaviour
{
    /// <summary>
    /// The width, height, and length of the chunk in blocks
    /// </summary>
    public const int width = 32, height = 32, length = 32;

    /// <summary>
    /// Instantiates and Initializes a new Chunk Object
    /// </summary>
    /// <param name="position"></param>
    /// <param name="voxels"></param>
    /// <param name="parent"></param>
    /// <param name="material"></param>
    /// <returns></returns>
    public static Chunk CreateChunk(Vector3Int position, Block[,,] voxels, Transform parent, Material material)
    {
        if (voxels.GetLength(0) != width || voxels.GetLength(1) != height || voxels.GetLength(2) != length)
        {
            Debug.LogError("Cannot create chunk that doesn't conform to chunk width of " + width + " height of " + height + " and length of " + length);
            return null;
        }

        Chunk chunk = Instantiate(WorldController.Main.defaultChunk, parent);
        chunk.SetChunk(position, voxels, material);

        return chunk;
    }

    //Fields
    /// <summary>
    /// The Chunk Position
    /// </summary>
    Vector3Int position;

    /// <summary>
    /// The List of blocks in the Chunk
    /// </summary>
    public Block[,,] blocks = new Block[width, height, length];

    public int[,] heights = new int[width, length];

    public float[,] corruptionMap = new float[width, length];

    //Methods

    /// <summary>
    /// Gets the top level (non-air) Block at the given local coordinate
    /// </summary>
    /// <returns></returns>
    public Block GetSurfaceBlock(int x, int z)
    {
        Block block = blocks[x, 0, z];
        for (int i = 1; i < height; i++)
        {
            if (block.blockID == 0)
            {
                break;
            }
            block = blocks[x, i, z];
        }
        return block;
    }

    public int GetHeight(int x, int z)
    {
        return heights[x, z] + 1;
    }

    public void SetHeight(int x, int z, int h)
    {
        heights[x, z] = height - 1;
    }

    /// <summary>
    /// Initializes the chunk with the given data
    /// </summary>
    public void SetChunk(Vector3Int position, Block[,,] voxels, Material material)
    {
        if (voxels.GetLength(0) != width || voxels.GetLength(1) != height || voxels.GetLength(2) != length)
        {
            Debug.LogError("Cannot create chunk that doesn't conform to chunk width of " + width + " height of " + height + " and length of " + length);
            return;
        }

        this.position = position;
        this.blocks = voxels;
        GetComponent<MeshRenderer>().material = material;
    }

    /// <summary>
    /// Call this anytime a chunk is updated
    /// </summary>
    public void UpdateChunk()
    {
        ReloadMesh();
        ReloadHeights();
    }

    public void UpdateCorruption()
    {
        var meshFilter = GetComponent<MeshFilter>();

        if (meshFilter == null || meshFilter.sharedMesh == null || meshFilter.sharedMesh.vertexCount == 0)
        {
            return;
        }

        List<Vector2> corruption = new List<Vector2>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < length; z++)
                {
                    if (CheckVoxel(new Vector3Int(x, y, z)))
                    {
                        var corruptStrength = corruptionMap[x, z];
                        for (int i = 0; i < 6; i++)
                        {
                            if (!CheckVoxel(new Vector3Int(x, y, z) + VoxelData.faceChecks[i]))
                            {
                                corruption.Add(new Vector2(corruptStrength, 0));
                                corruption.Add(new Vector2(corruptStrength, 0));
                                corruption.Add(new Vector2(corruptStrength, 0));
                                corruption.Add(new Vector2(corruptStrength, 0));
                            }
                        }
                    }

                }
            }
        }

        if (corruption.Count == meshFilter.sharedMesh.vertexCount)
        {
            meshFilter.sharedMesh.SetUVs(3, corruption);
        }
        else
        {
            Debug.LogError("INVALID CORRUPTION VALUES");
        }
    }

    /// <summary>
    /// Reloads chunks heightmap
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void ReloadHeights()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                int y = 0;
                while (y < height && blocks[x, y, z].blockID != 0)
                {
                    y++;
                }
                heights[x, z] = y - 1;
            }
        }
    }

    /// <summary>
    /// Reloads the Chunk Mesh
    /// </summary>
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
        List<Vector2> corruption = new List<Vector2>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < length; z++)
                {
                    DrawVoxel(new Vector3Int(x, y, z), blocks[x, y, z].blockID, corruptionMap[x,z]);
                }
            }
        }

        Mesh mesh = new Mesh();

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);
        mesh.SetUVs(3,corruption);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.sharedMesh = mesh;
        GetComponent<MeshCollider>().convex = false;
        GetComponent<MeshCollider>().sharedMesh = mesh;

        GetComponent<MeshCollider>().sharedMesh = mesh;

        transform.position = new Vector3(position.x * width, 0, position.z * length);

        void DrawVoxel(Vector3Int pos, int blockID, float corruptStrength = 0f)
        {
            if (blockID == 0)
            {
                return;
            }
            ///
            /// 
            /// Help from https://github.com/b3agz/Code-A-Game-Like-Minecraft-In-Unity/blob/master/01-the-first-voxel/Assets/Scripts/Chunk.cs
            for (int i = 0; i < 6; i++)
            {
                if (!CheckVoxel(pos + VoxelData.faceChecks[i]))
                {
                    Vector2 UVOffsett = BlockManager.Main.GetBlockUVOffset(blockID, i);

                    vertices.Add(pos + VoxelData.Vertices[VoxelData.Triangles[i, 0]]);
                    vertices.Add(pos + VoxelData.Vertices[VoxelData.Triangles[i, 1]]);
                    vertices.Add(pos + VoxelData.Vertices[VoxelData.Triangles[i, 2]]);
                    vertices.Add(pos + VoxelData.Vertices[VoxelData.Triangles[i, 3]]);
                    uvs.Add((VoxelData.UVs[0] * BlockManager.Main.TextureAtlas.NormalizedBlockTextureSize) + UVOffsett);
                    uvs.Add((VoxelData.UVs[1] * BlockManager.Main.TextureAtlas.NormalizedBlockTextureSize) + UVOffsett);
                    uvs.Add((VoxelData.UVs[2] * BlockManager.Main.TextureAtlas.NormalizedBlockTextureSize) + UVOffsett);
                    uvs.Add((VoxelData.UVs[3] * BlockManager.Main.TextureAtlas.NormalizedBlockTextureSize) + UVOffsett);
                    corruption.Add(new Vector2(corruptStrength, 0));
                    corruption.Add(new Vector2(corruptStrength, 0));
                    corruption.Add(new Vector2(corruptStrength, 0));
                    corruption.Add(new Vector2(corruptStrength, 0));
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

    //public void UpdateCorruptionUV()
    //{
    //    List<Vector2> corruption = new List<Vector2>();
    //    for (int x = 0; x < width; x++)
    //    {
    //        for (int y = 0; y < height; y++)
    //        {
    //            for (int z = 0; z < length; z++)
    //            {
    //                if(blocks[x, y, z].blockID == 0)
    //                {
    //                    continue;
    //                }

    //                float corruptStrength = corruptionMap[x, z];
    //                corruption.Add(new Vector2(corruptStrength, 0));
    //                corruption.Add(new Vector2(corruptStrength, 0));
    //                corruption.Add(new Vector2(corruptStrength, 0));
    //                corruption.Add(new Vector2(corruptStrength, 0));
    //            }
    //        }
    //    }
    //    GetComponent<MeshFilter>().sharedMesh.SetUVs(3, corruption);
    //}

    /// <summary>
    /// Returns the Blok at the given local position
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Block GetBlock(Vector3Int pos)
    {
        if (pos.x > width || pos.y > height || pos.z > length)
        {
            Debug.LogError("Invalid Chunk Position");
            return new Block(-1, Vector3Int.zero);
        }
        return blocks[pos.x, pos.y, pos.z];
    }

    /// <summary>
    /// Checks if there is a (non-air) Block at the given position 
    /// </summary>
    bool CheckVoxel(Vector3Int pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (x < 0 || x > width - 1 || y < 0 || y > height - 1 || z < 0 || z > length - 1)
            return false;

        if (blocks[x, y, z].blockID == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


}
