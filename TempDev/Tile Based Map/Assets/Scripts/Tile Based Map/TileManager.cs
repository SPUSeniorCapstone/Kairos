using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    static TileManager main;
    public static TileManager Main
    {
        get { if (main == null) main = FindObjectOfType<TileManager>(); return main; }
    }

    public static int textureHeight = 32;
    public static int textureWidth = 32;
    public static int TextureAtlasSizeInBlocks = 4;
    public static float NormalizedBlockTextureSize { get { return 1f/(float)TextureAtlasSizeInBlocks; } }


    //[DisableOnPlay]
    public TileType[] tileTypes;


    Texture2D textureAtlas;

    [SerializeField]
    [Button("GenerateTextureAtlas")]
    bool Button_GenerateTextureAtlas = false;
    public Texture2D GenerateTextureAtlas()
    {
        textureAtlas = new Texture2D(textureWidth * TextureAtlasSizeInBlocks, textureHeight * TextureAtlasSizeInBlocks);

        for(int y = 0; y < TextureAtlasSizeInBlocks; y++)
        {
            for(int x = 0; x < TextureAtlasSizeInBlocks; x++)
            {
                int index = (y * TextureAtlasSizeInBlocks) + x;
                if(index < tileTypes.Length )
                {
                    textureAtlas.SetPixels(x * 32, y * 32, 32, 32, tileTypes[index].texture.GetPixels());
                }
                else
                {
                    textureAtlas.SetPixels(x * 32, y * 32, 32, 32, tileTypes[0].texture.GetPixels());
                }
            }
        }
        textureAtlas.filterMode = FilterMode.Point;
        textureAtlas.wrapMode = TextureWrapMode.Clamp;
        textureAtlas.Apply();
        return textureAtlas;
    }

    [SerializeField]
    [Button("SaveTextureAtlas")]
    bool Button_SaveTextureAtlasToFile = false;
    public void SaveTextureAtlas()
    {
        if (textureAtlas == null)
        {
            Debug.LogError("Cannot save null texture atlas");
            return;
        }

        System.IO.File.WriteAllBytes(Application.dataPath + "/Resources/" + "TextureAtlas.png", textureAtlas.EncodeToPNG());
    }

    /// <summary>
    /// Saves the texture atlas to the Resources foler
    /// </summary>
    /// <param name="filePath">Relative path from the Resources folder</param>
    public void SaveTextureAtlasToFile(string filePath = "TextureAtlas.png")
    {
        if (textureAtlas == null)
        {
            Debug.LogError("Cannot save null texture atlas");
            return;
        }

        System.IO.File.WriteAllBytes(Application.dataPath + "/Resources/" + filePath, textureAtlas.EncodeToPNG());
    }
}
