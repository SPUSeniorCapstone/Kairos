using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    static BlockManager main;
    public static BlockManager Main
    {
        get { if (main == null) main = FindObjectOfType<BlockManager>(); return main; }
    }

    public BlockType[] blockTypes;

    [SerializeField]
    [Button("ReloadIDs")]
    bool Button_ReloadIDs = false;
    public void ReloadIDs()
    {
        for (int i = 0; i < blockTypes.Length; i++)
        {
            blockTypes[i].id = i + 1;
        }
    }

    public Vector2 GetBlockUVOffset(int BlockID, int face)
    {
        return GetOffsetsByID(blockTypes[BlockID - 1].textureID);
    }

    public Vector2 GetOffsetsByID(int TextureID)
    {
        int x = TextureID % TextureAtlasManager.TextureAtlasSizeInBlocks;
        int y = TextureID / TextureAtlasManager.TextureAtlasSizeInBlocks;

        return new Vector2(x, y) * TextureAtlasManager.NormalizedBlockTextureSize; 
    }
}
