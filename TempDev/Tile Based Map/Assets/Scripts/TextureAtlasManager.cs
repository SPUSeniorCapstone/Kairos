using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;

public class TextureAtlasManager : MonoBehaviour
{
    static TextureAtlasManager main;
    public static TextureAtlasManager Main
    {
        get { if (main == null) main = FindObjectOfType<TextureAtlasManager>(); return main; }
    }
    public static int textureHeight = 32;
    public static int textureWidth = 32;
    public static int TextureAtlasSizeInBlocks = 4;
    public static float NormalizedBlockTextureSize { get { return 1f / (float)TextureAtlasSizeInBlocks; } }


    public Texture2D defaultTexture;

    [Disable]
    public Texture2D textureAtlas;

    [Header("Texture Atlas Editor")]
    [Disable]
    public int textureCount;

    public TextureAtlasItem[] textures;

    private void Awake()
    {
        main = this;
    }

    [SerializeField]
    [Button("ReloadIDs")]
    bool Button_ReloadIDs = false;
    public void ReloadIDs()
    {
        for(int i = 0; i < textures.Length; i++)
        {
            textures[i].ID = i;
        }
    }

    [Header("Texture Atlas Generator")]
    [SerializeField]
    [Button("GenerateTextureAtlas")]
    bool Button_GenerateTextureAtlas = false;
    public Texture2D GenerateTextureAtlas()
    {
        ReloadIDs();

        textureAtlas = new Texture2D(textureWidth * TextureAtlasSizeInBlocks, textureHeight * TextureAtlasSizeInBlocks);

        for (int y = 0; y < TextureAtlasSizeInBlocks; y++)
        {
            for (int x = 0; x < TextureAtlasSizeInBlocks; x++)
            {
                int index = (y * TextureAtlasSizeInBlocks) + x;
                if (index < textures.Length)
                {
                    textureAtlas.SetPixels(x * 32, y * 32, 32, 32, textures[index].texture.GetPixels());
                }
                else
                {
                    textureAtlas.SetPixels(x * 32, y * 32, 32, 32, textures[0].texture.GetPixels());
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

[Serializable]
public struct TextureAtlasItem
{
    [Disable]
    public int ID;
    public string name;
    public Texture2D texture;
}
