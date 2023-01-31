using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;

/// <summary>
/// TODO
/// **Create Custom Editor**
/// </summary>
public class TextureAtlas : MonoBehaviour
{
    /// <summary>
    /// The height of each texture
    /// </summary>
    [DisableOnPlay]
    public int textureHeight = 32;

    /// <summary>
    /// The width of each texture
    /// </summary>
    [DisableOnPlay]
    public int textureWidth = 32;

    /// <summary>
    /// How many textures to put on each row/column
    /// </summary>
    [DisableOnPlay]
    public int TextureAtlasSizeInBlocks = 4;

    /// <summary>
    /// The size of each texture in normalized metrics
    /// </summary>
    public float NormalizedBlockTextureSize { get { return 1f / (float)TextureAtlasSizeInBlocks; } }

    /// <summary>
    /// The Generated Texture Atlas Texture
    /// </summary>
    public Texture2D Texture
    {
        get
        {
            if(textureAtlas == null)
            {
                textureAtlas = GenerateTextureAtlas();
            }
            return textureAtlas;
        }
    }
    [Disable][SerializeField] Texture2D textureAtlas;

    [Header("Texture Atlas Editor")]

    public TextureAtlasItem[] textures;


    [SerializeField]
    [Button("ReloadTextureAtlasIDs")]
    bool Button_ReloadIDs = false;
    public void ReloadTextureAtlasIDs()
    {
        for (int i = 0; i < textures.Length; i++)
        {
            textures[i].ID = i;
        }
    }


    [Header("Texture Atlas Generator")]
    [SerializeField]
    [Button("GenerateTextureAtlas")]
    bool Button_GenerateTextureAtlas = false;
    /// <summary>
    /// Generates the texture atlas from the list of TextureAtlasItems
    /// </summary>
    /// <returns>The Texture Atlas Texture</returns>
    public Texture2D GenerateTextureAtlas()
    {
        ReloadTextureAtlasIDs();

        textureAtlas = new Texture2D(textureWidth * TextureAtlasSizeInBlocks, textureHeight * TextureAtlasSizeInBlocks);

        for (int y = 0; y < TextureAtlasSizeInBlocks; y++)
        {
            for (int x = 0; x < TextureAtlasSizeInBlocks; x++)
            {
                int index = (y * TextureAtlasSizeInBlocks) + x;
                if (index < textures.Length)
                {
                    textureAtlas.SetPixels(x * textureWidth, y * textureHeight, textureWidth, textureHeight, textures[index].texture.GetPixels());
                }
                else
                {
                    textureAtlas.SetPixels(x * textureWidth, y * textureHeight, textureWidth, textureHeight, textures[0].texture.GetPixels());
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
    /// <summary>
    /// Saves the texture atlas to the default file location
    /// </summary>
    public void SaveTextureAtlas()
    {
        if (textureAtlas == null)
        {
            Debug.LogError("Cannot save null texture atlas");
            return;
        }

        System.IO.File.WriteAllBytes(Application.dataPath + "/Resources/Textures/" + "MapTextureAtlas.png", textureAtlas.EncodeToPNG());
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

/// <summary>
/// Stores a single TextureAtlas Texture Item
/// </summary>
[Serializable]
public struct TextureAtlasItem
{
    public string name;
    [Disable]
    public int ID;
    public Texture2D texture;
}
