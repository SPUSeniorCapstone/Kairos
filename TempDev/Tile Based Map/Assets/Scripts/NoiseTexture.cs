using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class NoiseTexture : MonoBehaviour
{
    public string filePath = "filePAth";
    public float scale = 0.01f;

    [Button("GenerateNoise")]
    public bool Generate;

    public void GenerateNoise()
    {
        Color[] colors = new Color[512 * 512];

        for(int x = 0; x < 512; x++)
        {
            for(int y = 0; y < 512; y++)
            {
                int i = x + (y * 512);
                float j = Mathf.PerlinNoise(x * scale, y * scale);
                colors[i] = new Color(j,j,j);
            }
        }

        Texture2D texture = new Texture2D(512, 512);
        texture.SetPixels(colors);

        System.IO.File.WriteAllBytes(Application.dataPath + "/Resources/" + filePath, texture.EncodeToPNG());
    }
}
