using UnityEngine;

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
            return chunks;
        }
    }
    Chunk[,] chunks;

    Vector2Int currUpdate = Vector2Int.zero;

    public bool enable = true;

    bool meshUpdate = false;

    public void Init(Vector2Int size)
    {
        Helpers.DeleteAllChildren(gameObject);

        widthInChunks = size.x;
        lengthInChunks = size.y;
        chunks = new Chunk[widthInChunks, lengthInChunks];
    }

    private void Update()
    {
        if(enable && !meshUpdate)
            UpdateNext();
        else if (meshUpdate)
        {
            foreach(Chunk c in chunks)
            {
                c.PushChunkMesh();
            }
            meshUpdate = false;
        }
    }

    public void UpdateNext()
    {
        chunks[currUpdate.x, currUpdate.y].BeginAsyncUpdate();
        //chunks[currUpdate.x, currUpdate.y].UpdateCorruption();
        //chunks[currUpdate.x, currUpdate.y].ReloadDecorations();

        currUpdate.x += 1;
        if(currUpdate.x >= widthInChunks)
        {
            currUpdate.x = 0;
            currUpdate.y++;

            if(currUpdate.y >= lengthInChunks)
            {
                currUpdate.y = 0;
                meshUpdate = true;
            }
        }
    }

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

    public void SetCorruption(int x, int z, float val)
    {
        int chunkX = x / Chunk.width, chunkZ = z / Chunk.length;
        int voxelX = x % Chunk.width, voxelZ = z % Chunk.length;
        if (chunkX >= widthInChunks || chunkZ >= lengthInChunks ||
            voxelX >= Chunk.width || voxelZ >= Chunk.length ||
            chunkX < 0 || chunkZ < 0 || voxelX < 0 || voxelZ < 0)
        {
            return;
        }

        Chunks[chunkX, chunkZ].corruptionMap[voxelX, voxelZ] = Mathf.Clamp(val,0,1);
    }

    public float GetCorruption(int x, int z)
    {
        int chunkX = x / Chunk.width, chunkZ = z / Chunk.length;
        int voxelX = x % Chunk.width, voxelZ = z % Chunk.length;
        if (chunkX >= widthInChunks || chunkZ >= lengthInChunks ||
            voxelX >= Chunk.width || voxelZ >= Chunk.length ||
            chunkX < 0 || chunkZ < 0 || voxelX < 0 || voxelZ < 0)
        {
            return -1;
        }

        return Chunks[chunkX, chunkZ].corruptionMap[voxelX,voxelZ];
    }

    public void SetCorruptionMap(float[,] corruptionMap)
    {
        if(corruptionMap.GetLength(0) != WidthInBlocks || corruptionMap.GetLength(1) != LengthInBlocks)
        {
            return;
        }

        for(int x = 0; x < WidthInBlocks; x++)
        {
            for(int z = 0; z < LengthInBlocks; z++)
            {
                SetCorruption(x,z,corruptionMap[x, z]);
            }
        }
    }

    public bool CheckVoxel(Vector3Int pos)
    {
        var x = pos.x;
        var y = pos.y;
        var z = pos.z;

        int chunkX = x / Chunk.width, chunkZ = z / Chunk.length;
        int voxelX = x % Chunk.width, voxelZ = z % Chunk.length;
        if (chunkX >= widthInChunks || chunkZ >= lengthInChunks ||
            voxelX >= Chunk.width || voxelZ >= Chunk.length ||
            chunkX < 0 || chunkZ < 0 || voxelX < 0 || voxelZ < 0 ||
            y < 0 || y > Chunk.height)
        {
            return false;
        }

        return Chunks[chunkX, chunkZ].CheckVoxel(new Vector3Int(voxelX,y,voxelZ));
    }

    public Vector2Int WorldToChunkPosition(Vector2Int position)
    {
        int chunkX = position.x / Chunk.width, chunkZ = position.y / Chunk.length;
        return new Vector2Int(chunkX, chunkZ);
    }
}
