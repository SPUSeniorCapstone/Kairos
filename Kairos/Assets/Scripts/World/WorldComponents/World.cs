using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TerrainUtils;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

/// <summary>
/// The 3D Voxel World
/// </summary>
public class World : MonoBehaviour
{
    /// <summary>
    /// The size of the world in chunks
    /// </summary>
    [Disable]
    public int widthInChunks = 16, lengthInChunks = 16;
    public int WidthInBlocks
    {
        get
        {
            return widthInChunks * Chunk.width;
        }
    }
    public int LengthInBlocks
    {
        get
        {
            return lengthInChunks * Chunk.length;
        }
    }

    public float BlockScale
    {
        get { return WorldController.Main.blockScale; }
    }

    /// <summary>
    /// The material to use for rendering the world (this material should have the texture atlas attached to it)
    /// </summary>
    public Material worldMaterial;

    public Bounds bounds;

    /// <summary>
    /// The world seed - this is stored for reading purposes only
    /// </summary>
    [Disable]
    public int seed;

    /// <summary>
    /// The 2D array of voxel chunks
    /// <para>
    /// If Chunks is deemed invalid, it will delete all chunks and recreate the array.
    /// </para>
    /// </summary>
    public Chunk[,] Chunks
    {
        get
        {
            if (chunks == null || chunks.GetLength(0) != widthInChunks || chunks.GetLength(1) != lengthInChunks)
            {
                Helpers.DeleteAllChildren(gameObject);
                chunks = new Chunk[widthInChunks, lengthInChunks];
            }
            return chunks;
        }
    }
    Chunk[,] chunks;


    /// <summary>
    /// Returns the top most block at the given (x,z) position
    /// </summary>
    public Block GetSurfaceBlock(int x, int z)
    {
        int chunkX = x / Chunk.width, chunkZ = z / Chunk.length;
        int voxelX = x % Chunk.width, voxelZ = z % Chunk.length;
        if (chunkX >= widthInChunks || chunkZ >= lengthInChunks || voxelX >= Chunk.width || voxelZ >= Chunk.length ||
           chunkX < 0 || chunkZ < 0 || voxelX < 0 || voxelZ < 0)
        {
            return new Block();
        }

        return Chunks[chunkX, chunkZ].GetSurfaceBlock(voxelX, voxelZ);
    }

    public int GetHeight(int x, int z)
    {
        int chunkX = x / Chunk.width, chunkZ = z / Chunk.length;
        int voxelX = x % Chunk.width, voxelZ = z % Chunk.length;
        if (chunkX >= widthInChunks || chunkZ >= lengthInChunks || voxelX >= Chunk.width || voxelZ >= Chunk.length ||
           chunkX < 0 || chunkZ < 0 || voxelX < 0 || voxelZ < 0)
        {
            return Chunk.height;
        }

        return Chunks[chunkX, chunkZ].GetHeight(voxelX, voxelZ);
    }

    public int GetHeight(Vector3Int v)
    {
        return GetHeight(v.x, v.z);
    }

    public int GetHeight(float x, float z)
    {
        var pos = GameController.Main.WorldController.WorldToBlockPosition(new Vector3(x, 0, z));
        return GetHeight(pos.x, pos.z);
    }

    public void SetHeight(int x, int z, int height)
    {
        int chunkX = x / Chunk.width, chunkZ = z / Chunk.length;
        int voxelX = x % Chunk.width, voxelZ = z % Chunk.length;
        if (chunkX >= widthInChunks || chunkZ >= lengthInChunks || voxelX >= Chunk.width || voxelZ >= Chunk.length ||
           chunkX < 0 || chunkZ < 0 || voxelX < 0 || voxelZ < 0)
        {
            return;
        }

        Chunks[chunkX, chunkZ].SetHeight(voxelX, voxelZ, height);
    }

    public bool IsPassable(int x, int z)
    {
        //int chunkX = x / Chunk.width, chunkZ = z / Chunk.length;
        //int voxelX = x % Chunk.width, voxelZ = z % Chunk.length;
        //if (chunkX >= width || chunkZ >= length || voxelX >= Chunk.width || voxelZ >= Chunk.length ||
        //   chunkX < 0 || chunkZ < 0 || voxelX < 0 || voxelZ < 0)
        //{
        //    return Chunk.height;
        //}

        //return Chunks[chunkX, chunkZ].GetHeight(voxelX, voxelZ);
        return true;
    }

    /// <summary>
    /// Returns the Block at the given (x,y,z) position
    /// </summary>
    public Block GetBlock(int x, int y, int z)
    {
        int chunkX = x / Chunk.width, chunkZ = z / Chunk.length;
        int voxelX = x % Chunk.width, voxelZ = z % Chunk.length;
        if (chunkX >= widthInChunks || chunkZ >= lengthInChunks ||
            voxelX >= Chunk.width || voxelZ >= Chunk.length ||
            chunkX < 0 || chunkZ < 0 || voxelX < 0 || voxelZ < 0 ||
            y < 0 || y > Chunk.height)
        {
            return new Block();
        }

        return Chunks[chunkX, chunkZ].blocks[voxelX, y, voxelZ];
    }

    /// <summary>
    /// Returns the Block at the given position
    /// </summary>
    public Block GetBlock(Vector3Int position)
    {
        return GetBlock(position.x, position.y, position.z);
    }

    public Texture2D GeneratedWorldTexture()
    {

        Color[] colors = new Color[WidthInBlocks * LengthInBlocks];

        for (int x = 0; x < WidthInBlocks; x++)
        {
            for (int z = 0; z < LengthInBlocks; z++)
            {
                var block = GetBlock(x, 0, z);
                colors[x + (z * LengthInBlocks)] = BlockManager.Main.GetBlockColor(block.blockID);
            }
        }

        Texture2D texture = new Texture2D(WidthInBlocks, LengthInBlocks);
        texture.SetPixels(colors);
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        return texture;
    }
}
