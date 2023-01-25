using System.Collections;
using System.Collections.Generic;
using Unity.Rendering.HybridV2;
using UnityEngine;

public class World : MonoBehaviour
{
    [DisableOnPlay]
    public int width = 16, length = 16;

    public Material material;

    [Disable]
    public int seed;

    public Chunk[,] Chunks
    {
        get
        {
            if(chunks == null || chunks.GetLength(0) != width || chunks.GetLength(1) != length)
            {
                Helpers.DeleteAllChildren(gameObject);
                chunks = new Chunk[width, length];
            }
            return chunks;
        }
    } 
    Chunk[,] chunks;

    public Tile GetTile(int x, int z)
    {
        int chunkX = x / Chunk.width, chunkZ = z / Chunk.length;
        int tileX = x % Chunk.width, tileZ = z % Chunk.length;
        if(chunkX >= width || chunkZ >= length || tileX >= Chunk.width || tileZ >= Chunk.length ||
           chunkX < 0 || chunkZ < 0 || tileX < 0 || tileZ < 0)
        {
            return null;
        }

        return Chunks[chunkX, chunkZ].tiles[tileX, tileZ];
    }
}
