using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

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

    public Vector3Int Position { get { return position; } }
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


    Task<bool> UpdateTask = null;
    ChunkMesh chunkMesh = null;
    int[,] heightsUpdate = null;

    //Methods

    private void Update()
    {
        
    }

    public void PushChunkMesh()
    {
        if (UpdateTask != null)
        {
            if (UpdateTask.IsCompleted)
            {
                if (!UpdateTask.Result)
                {
                    Debug.LogError("ERROR: Update Failed");
                }
                else
                {
                    var meshFilter = GetComponent<MeshFilter>();
                    Mesh mesh = new Mesh();

                    mesh.SetVertices(chunkMesh.vertices);
                    mesh.SetTriangles(chunkMesh.triangles, 0);
                    mesh.SetUVs(0, chunkMesh.uvs);
                    mesh.SetUVs(3, chunkMesh.corruption);
                    mesh.RecalculateNormals();
                    mesh.RecalculateBounds();

                    meshFilter.sharedMesh = mesh;
                    GetComponent<MeshCollider>().convex = false;
                    GetComponent<MeshCollider>().sharedMesh = mesh;

                    GetComponent<MeshCollider>().sharedMesh = mesh;

                    transform.position = new Vector3(position.x * width, 0, position.z * length);

                    heights = heightsUpdate;

                    ReloadDecorations();
                }
                UpdateTask = null;
            }

        }
    }

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
        ReloadDecorations();
    }

    public void BeginAsyncUpdate()
    {
        if (UpdateTask == null)
        {
            UpdateTask = UpdateChunkAsync();
        }
        else
            Debug.LogError("ERROR: Update in Progress");
    }

    async Task<bool> UpdateChunkAsync()
    {
        chunkMesh = await Task.Run(ReloadMeshAsync);
        heightsUpdate = await Task.Run(ReloadHeightsAsync);

        if (chunkMesh == null || heightsUpdate == null)
        {
            chunkMesh = null;
            heightsUpdate = null;
            return false;
        }
        return true;
    }

    public void ReloadDecorations()
    {
        foreach(var child in transform.GetComponentsInChildren<DecorationObject>())
        {
            var pos = child.position;
            if(pos.x < width && pos.x >= 0 && pos.y < length && pos.x >= 0)
            {
                bool corr = corruptionMap[pos.x, pos.y] > 0.5f;
                child.UpdateCorrupted(corr);
            }

        }
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

            for (int z = 0; z < length; z++)
            {
                for (int y = 0; y < height; y++)
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
                    else
                    {
                        break;
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
    public void ReloadHeights()
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

            for (int z = 0; z < length; z++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (blocks[x, y, z].blockID == 0)
                    {
                        break;
                    }
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

    public int[,] ReloadHeightsAsync()
    {
        int[,] h = new int[width, length];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                int y = 0;
                while (y < height && blocks[x, y, z].blockID != 0)
                {
                    y++;
                }
                h[x, z] = y - 1;
            }
        }
        return h;
    }

    public ChunkMesh ReloadMeshAsync()
    {

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
                    DrawVoxel(new Vector3Int(x, y, z), blocks[x, y, z].blockID, corruptionMap[x, z]);
                }
            }
        }

        return new ChunkMesh(vertices, triangles, uvs, corruption);

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
    public bool CheckVoxel(Vector3Int pos)
    {
        int x = pos.x;
        int y = pos.y;
        int z = pos.z;

        if (y < 0) return true;
        else if (y > height - 1) return false;

        if (x < 0 || x > width - 1 || y < 0 || z < 0 || z > length - 1)
        {
            var chunkOrigin = new Vector3Int(position.x * width, 0, position.z * length);
            return WorldController.Main.World.CheckVoxel(chunkOrigin + pos);
        }

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

public class ChunkMesh
{
    public List<Vector3> vertices;
    public List<int> triangles;
    public List<Vector2> uvs;
    public List<Vector2> corruption;

    public ChunkMesh(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, List<Vector2> corruption)
    {
        this.vertices = vertices;
        this.triangles = triangles;
        this.uvs = uvs;
        this.corruption = corruption;
    }
}