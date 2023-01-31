using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextureAtlas))]
public class BlockManager : MonoBehaviour
{
    static BlockManager main;
    public static BlockManager Main
    {
        get { if (main == null) main = FindObjectOfType<BlockManager>(); return main; }
    }

    public TextureAtlas TextureAtlas
    {
        get
        {
            if(textureAtlas == null)
            {
                textureAtlas = GetComponent<TextureAtlas>();
            }
            return textureAtlas;
        }
    }
    TextureAtlas textureAtlas;

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
        return GetOffsetsByID(blockTypes[BlockID - 1].textures.GetTextureID((FaceID)face));
    }

    public Vector2 GetOffsetsByID(int TextureID)
    {
        int x = TextureID % TextureAtlas.TextureAtlasSizeInBlocks;
        int y = TextureID / TextureAtlas.TextureAtlasSizeInBlocks;

        return new Vector2(x, y) * TextureAtlas.NormalizedBlockTextureSize;
    }

    public void ReloadTextureAtlasIDs()
    {
        TextureAtlas.ReloadTextureAtlasIDs();
    }

    public void GenerateTextureAtlas()
    {
        TextureAtlas.GenerateTextureAtlas();
    }
    
    public void SaveTextureAtlas()
    {
        TextureAtlas.SaveTextureAtlas();
    }
}
