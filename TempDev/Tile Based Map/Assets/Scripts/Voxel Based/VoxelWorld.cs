using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class VoxelWorld : MonoBehaviour
{
    [DisableOnPlay]
    public int width = 16, length = 16;

    public Material worldMaterial;

    [Disable]
    public int seed;

    public VoxelChunk[,] Chunks
    {
        get
        {
            if (chunks == null || chunks.GetLength(0) != width || chunks.GetLength(1) != length)
            {
                Helpers.DeleteAllChildren(gameObject);
                chunks = new VoxelChunk[width, length];
            }
            return chunks;
        }
    }
    VoxelChunk[,] chunks;

    public Voxel GetSurfaceVoxel(int x, int z)
    {
        int chunkX = x / VoxelChunk.width, chunkZ = z / VoxelChunk.length;
        int voxelX = x % VoxelChunk.width, voxelZ = z % VoxelChunk.length;
        if (chunkX >= width || chunkZ >= length || voxelX >= VoxelChunk.width || voxelZ >= VoxelChunk.length ||
           chunkX < 0 || chunkZ < 0 || voxelX < 0 || voxelZ < 0)
        {
            return new Voxel();
        }

        return Chunks[chunkX, chunkZ].GetSurfaceVoxel(voxelX, voxelZ);
    }

    public Voxel GetVoxel(int x, int y, int z)
    {
        int chunkX = x / VoxelChunk.width, chunkZ = z / VoxelChunk.length;
        int voxelX = x % VoxelChunk.width, voxelZ = z % VoxelChunk.length;
        if (chunkX >= width || chunkZ >= length ||
            voxelX >= VoxelChunk.width || voxelZ >= VoxelChunk.length ||
            chunkX < 0 || chunkZ < 0 || voxelX < 0 || voxelZ < 0 ||
            y < 0 || y > VoxelChunk.height)
        {
            return new Voxel();
        }

        return Chunks[chunkX, chunkZ].voxels[voxelX, y, voxelZ];
    }
}
