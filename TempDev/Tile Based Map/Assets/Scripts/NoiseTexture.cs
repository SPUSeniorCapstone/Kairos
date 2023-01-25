using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class NoiseTexture : MonoBehaviour
{
    public string filePath = "filePAth";

    [Button("GenerateNoise")]
    public bool Generate;

    [DisableOnPlay]
    public int chunkWidth = 512, chunkHeight = 512;
    public int seed;
    [Range(0.001f,3.0f)]
    public float scale;
    [Range(1,10)]
    public int octaves;
    [Range(0.001f,5)]
    public float persistance;
    [Range(0.001f,1)]
    public float lacunarity;
    public Vector2 offset;

    public bool useFalloff = false;

    float[,] falloff;

    MeshRenderer Mesh
    {
        get
        {
            if(mesh == null)
            {
                mesh = GetComponent<MeshRenderer>();
            }
            return mesh;
        }
    }
    MeshRenderer mesh;

    private void Update()
    {
        GenerateNoise();
    }

    public void GenerateNoise()
    {
        Color[] colors = new Color[chunkWidth * chunkHeight];

        float[,] noise = NoiseGenerator.GenerateNoiseMap(chunkWidth, chunkHeight, seed, scale, octaves, persistance, lacunarity, offset);

        if(falloff == null)
            falloff = NoiseGenerator.GenerateFalloffMap(chunkWidth, chunkHeight);

        for (int x = 0; x < chunkWidth; x++)
        {
            for(int y = 0; y < chunkHeight; y++)
            {
                int i = x + (y * chunkHeight);
                var j = noise[x, y];
                if (useFalloff)
                {
                    j -= falloff[x, y];
                }
                colors[i] = new Color(j,j,j);
            }
        }

        Texture2D texture = new Texture2D(512, 512);
        texture.SetPixels(colors);
        texture.Apply();

        Mesh.sharedMaterial.SetTexture("_MainTex", texture);

        //System.IO.File.WriteAllBytes(Application.dataPath + "/Resources/" + filePath, texture.EncodeToPNG());
    }
}
