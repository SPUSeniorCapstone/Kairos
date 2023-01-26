using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class World : MonoBehaviour
{
    [DisableOnPlay]
    public int width = 16, length = 16;

    public Material worldMaterial;

    [Disable]
    public int seed;

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

    public Block GetSurfaceVoxel(int x, int z)
    {
        int chunkX = x / Chunk.width, chunkZ = z / Chunk.length;
        int voxelX = x % Chunk.width, voxelZ = z % Chunk.length;
        if (chunkX >= width || chunkZ >= length || voxelX >= Chunk.width || voxelZ >= Chunk.length ||
           chunkX < 0 || chunkZ < 0 || voxelX < 0 || voxelZ < 0)
        {
            return new Block();
        }

        return Chunks[chunkX, chunkZ].GetSurfaceVoxel(voxelX, voxelZ);
    }

    public Block GetVoxel(int x, int y, int z)
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

        return Chunks[chunkX, chunkZ].voxels[voxelX, y, voxelZ];
    }
}
