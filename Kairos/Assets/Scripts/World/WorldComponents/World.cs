using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// The 3D Voxel World
/// </summary>
public class World : MonoBehaviour
{
    /// <summary>
    /// The size of the world in chunks
    /// </summary>
    [DisableOnPlay]
    public int width = 16, length = 16;

    /// <summary>
    /// The material to use for rendering the world (this material should have the texture atlas attached to it)
    /// </summary>
    public Material worldMaterial;

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
            if (chunks == null || chunks.GetLength(0) != width || chunks.GetLength(1) != length)
            {
                Helpers.DeleteAllChildren(gameObject);
                chunks = new Chunk[width, length];
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
        if (chunkX >= width || chunkZ >= length || voxelX >= Chunk.width || voxelZ >= Chunk.length ||
           chunkX < 0 || chunkZ < 0 || voxelX < 0 || voxelZ < 0)
        {
            return new Block();
        }

        return Chunks[chunkX, chunkZ].GetSurfaceBlock(voxelX, voxelZ);
    }

    /// <summary>
    /// Returns the Block at the given (x,y,z) position
    /// </summary>
    public Block GetBlock(int x, int y, int z)
    {
        int chunkX = x / Chunk.width, chunkZ = z / Chunk.length;
        int voxelX = x % Chunk.width, voxelZ = z % Chunk.length;
        if (chunkX >= width || chunkZ >= length ||
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
}
