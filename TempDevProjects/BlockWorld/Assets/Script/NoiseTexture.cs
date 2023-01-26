using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class NoiseTexture : MonoBehaviour
{
    [Button("GenerateNoise")]
    public bool Generate;

    [Button("GenerateMapTexture")]
    public bool GenerateMap;

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

    int terrainSeed;

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
    public void InitWorldGen(int seed)
    {
        Random.InitState(seed);
        terrainSeed = Random.Range(int.MinValue, int.MaxValue);
    }

    public void GenerateMapTexture()
    {

        InitWorldGen(seed);
        float[,] noise = GenerateWorldHeightmap();
        Color[] colors = new Color[noise.GetLength(0) * noise.GetLength(1)];

        for(int i = 0; i < noise.GetLength(0); i++)
        {
            for(int j = 0; j < noise.GetLength(1); j++)
            {
                float h = noise[i, j];
                colors[(i * noise.GetLength(0)) + j] = new Color(h, h, h);
            }
        }

        Texture2D texture = new Texture2D(noise.GetLength(0), noise.GetLength(1));
        texture.SetPixels(colors);
        texture.Apply();

        Mesh.sharedMaterial.SetTexture("_MainTex", texture);
    }

    public float[,] GenerateWorldHeightmap()
    {
        float[,] heights = new float[WorldController.main.World.width * Chunk.width, WorldController.main.World.length * Chunk.length];
        for (int x = 0; x < WorldController.main.World.width; x++)
        {
            for (int z = 0; z < WorldController.main.World.length; z++)
            {
                float[,] chunk = GenerateChunkHeightmap(new Vector3Int(x, 0, z));

                for (int i = 0; i < Chunk.width; i++)
                {
                    for (int j = 0; j < Chunk.length; j++)
                    {
                        heights[x * Chunk.width + i, z * Chunk.length + j] = chunk[i, j];
                    }
                }
            }
        }

        return heights;
    }

    public float[,] GenerateChunkHeightmap(Vector3Int position)
    {
        Vector2Int offset = new Vector2Int(position.x * Chunk.width, position.z * Chunk.length);
        return NoiseGenerator.GenerateNoiseMap(Chunk.width, Chunk.length, terrainSeed, scale, octaves, persistance, lacunarity, offset);

    }
}
