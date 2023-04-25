using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

/// <summary>
/// Generate a 3D Voxel-based world
/// </summary>
public class WorldGenerator : MonoBehaviour
{
    static WorldGenerator instance;

    World world => WorldController.Main.World;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
            //seed = instance.seed;
            //scale = instance.scale;
            //Destroy(instance.gameObject);
            //instance = this;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public Vector3Int strongholdPos;
    public float strongholdRadius = 10;

    public List<Vector3Int> corruptionNodePositions = new List<Vector3Int>();
    public int numNodes = 10;
    public float corruptionNodeDensityRadius = 20;

    public int seed;
    public void SetSeed(string seed) { this.seed = int.Parse(seed); }

    public NoiseGenerator.NoiseSettings worldSettings;
    public NoiseGenerator.NoiseSettings flatMapSettings;
    public NoiseGenerator.NoiseSettings corruptionSettings;

    public List<Decoration> decorations;

    public bool useLayerHeights = true;
    public bool useFalloff = false;

    [Range(0, 100)]
    public int smoothStrength;

    public float corruptionStrength = 0.25f;

    float[,] falloff;
    float[,] heightMap;
    int[,] terrainMap;
    int[,] layerMap;
    int[,] blockMap;
    float[,] corruptionMap;

    public Vector2Int worldSize = new Vector2Int(8, 8);
    public void SetSize(float size) { worldSize = new Vector2Int((int)size, (int)size); }

    [Range(1f, Chunk.height)]
    public float eccentricity;

    public WorldLayer[] layers;

    [Range(0,100)]
    public int veinsPerNode = 10;
    [Range(0,1000)]
    public int stepsPerVein = 100;

    public void InitWorldGen(int seed)
    {
        UnityEngine.Random.InitState(seed);
        worldSettings.seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        corruptionSettings.seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        flatMapSettings.seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
    }

    public void GenerateWorld(bool loadMeshes = true)
    {

        world.seed = seed;
        world.widthInChunks = worldSize.x;
        world.lengthInChunks = worldSize.y;
        InitWorldGen(seed);

        world.Init(worldSize);

        GenerateHeights();


        GenerateVoxelMap();

        GenerateChunks();

        PlacePlayerStronghold();

        PlaceCorruptionNodes();


        GenerateCorruption();


        if (loadMeshes)
        {
            for (int x = 0; x < world.widthInChunks; x++)
            {
                for (int z = 0; z < world.lengthInChunks; z++)
                {
                    world.Chunks[x, z].UpdateChunk();
                }
            }
        }

        PlaceDecorations();

        //corruptionMap = new float[Chunk.width * world.widthInChunks, Chunk.length * world.lengthInChunks];
        //world.SetCorruptionMap(corruptionMap);

        float width = world.WidthInBlocks * world.BlockScale;
        float height = Chunk.height * world.BlockScale;
        float length = world.LengthInBlocks * world.BlockScale;

        Vector3 center = new Vector3(width / 2, width, length / 2);
        world.bounds = new Bounds(center.Flat(), center);
    }

    public void GenerateHeights()
    {
        Debug.Log("Generating Terrain Height Map...");
        heightMap = NoiseGenerator.GenerateNoiseMap(world.WidthInBlocks, world.LengthInBlocks, worldSettings);


        //Post Processing
        if (useFalloff) ApplyFalloffMap();
        Debug.Log("Finished Generating Terrain Height Map");
    }

    public void ApplyFalloffMap()
    {
        Debug.Log("Creating Falloff Map...");
        falloff = NoiseGenerator.GenerateFalloffMap(world.WidthInBlocks, world.LengthInBlocks);

        Debug.Log("Applying Falloff...");
        for (int x = 0; x < world.WidthInBlocks; x++)
        {
            for (int z = 0; z < world.LengthInBlocks; z++)
            {
                heightMap[x, z] = Mathf.Clamp(heightMap[x, z] - falloff[x, z], 0.0f, 1.0f);
            }
        }
        Debug.Log("Finished Applying Falloff");
    }


    public void GenerateVoxelMap()
    {
        Debug.Log("Generating Voxel Map...");
        terrainMap = new int[world.WidthInBlocks, world.LengthInBlocks];
        blockMap = new int[world.WidthInBlocks, world.LengthInBlocks];
        layerMap = new int[world.WidthInBlocks, world.LengthInBlocks];

        for (int x = 0; x < world.WidthInBlocks; x++)
        {
            for (int z = 0; z < world.LengthInBlocks; z++)
            {
                float height = heightMap[x, z];
                int blockID = 0;
                int newHeight = 1;
                int layer = 0;

                for(int i = 0; i < layers.Length; i++)
                {
                    blockID = layers[i].BlockID;
                    newHeight += layers[i].thickness;
                    layer = i;

                    if(i+1 >= layers.Length || layers[i+1].height > height)
                    {
                        break;
                    }
                }

                if (!useLayerHeights)
                {
                    newHeight = (int)(height * eccentricity);
                }

                layerMap[x, z] = layer;
                terrainMap[x, z] = newHeight;
                blockMap[x, z] = blockID;

            }
        }
    }




    public void PlacePlayerStronghold()
    {
        strongholdPos = GetRandomPosition().ToVector3Int();
    }

    public void GenerateCorruption()
    {
        corruptionMap = new float[world.WidthInBlocks, world.LengthInBlocks];
        foreach(var pos in corruptionNodePositions)
        {
            corruptionMap[pos.x,pos.z] = 1.0f;
            for(int i = 0; i < veinsPerNode; i++)
            {
                Vector2Int prev = pos.ToVector2Int();
                Vector2Int dir = Vector2Int.zero;
                float curr = 1.0f;
                int c = 0;
                while(curr > 0 && c < stepsPerVein)
                {
                    curr = corruptionMap[prev.x, prev.y] - Random.Range(0,0.01f);
                    Vector2Int next = prev + dir + new Vector2Int(Random.Range(-1, 2), Random.Range(-1, 2));
                    if (prev == next || next.x >= world.WidthInBlocks || next.x < 0 || next.y < 0 || next.y >= world.LengthInBlocks)
                    {
                        c++;
                        continue;
                    }

                    corruptionMap[next.x, next.y] = Mathf.Clamp(curr + corruptionMap[next.x, next.y], 0,1);
                    
                    dir = next - prev;
                    dir /= (int)dir.magnitude;

                    prev = next;
                    c++;
                }

            }

        }

        world.SetCorruptionMap(corruptionMap);

        /**
        //Debug.Log("Generating Corruption Map...");
        //corruptionMap = NoiseGenerator.GenerateNoiseMap(world.WidthInBlocks, world.LengthInBlocks, corruptionSettings);
        //for (int x = 0; x < world.WidthInBlocks; x++)
        //{
        //    for (int z = 0; z < world.LengthInBlocks; z++)
        //    {
        //        var c = corruptionMap[x, z];

        //        if(Vector3Int.Distance(strongholdPos, new Vector3Int(x,0,z)) < strongholdRadius)
        //        {
        //            c = 0;
        //        }

        //        if (c < 0.25) corruptionMap[x, z] = 0;
        //        if (c > 0.75) corruptionMap[x, z] = 1;
        //    }
        //}

        //float[,] temp = new float[world.WidthInBlocks, world.LengthInBlocks];
        //for (int x = 0; x < world.WidthInBlocks; x++)
        //{
        //    for (int z = 0; z < world.LengthInBlocks; z++)
        //    {
        //        float avg = 0;
        //        int c = 0;
        //        for (int i = -2; i <= 2; i++)
        //        {
        //            for (int j = -2; j <= 2; j++)
        //            {
        //                if (x + i < 0 || z + j < 0 || x + i >= world.WidthInBlocks || z + j >= world.LengthInBlocks)
        //                    continue;
        //                avg += corruptionMap[x + i, z + j];
        //                c++;
        //            }
        //        }
        //        if (c != 0)
        //            temp[x, z] = avg / c;
        //    }
        //}
        //corruptionMap = temp;

        //world.SetCorruptionMap(corruptionMap);
        //Debug.Log("Finished Generating Corruption Map");
        */

    }

    public void PlaceCorruptionNodes()
    {
        var positions = PoissonDiscSampling.GeneratePoints(corruptionNodeDensityRadius, new Vector2(world.WidthInBlocks, world.LengthInBlocks));


        corruptionNodePositions = new List<Vector3Int>();
        while (corruptionNodePositions.Count < numNodes && positions.Count > 0)
        {
            int i = Random.Range(0, positions.Count);
            Vector2 pos = positions[i];
            positions.RemoveAt(i);
            if(Vector3Int.Distance(new Vector3(pos.x,0,pos.y).ToVector3Int(), strongholdPos) > strongholdRadius)
            {
                corruptionNodePositions.Add(new Vector3(pos.x, 0, pos.y).ToVector3Int());
            }
        }
        Debug.Log("Placing Corruption Nodes");
    }

    public void PlaceDecorations()
    {
        foreach(var dec in decorations)
        {
            if(dec.decorationObject != null)
            {
                var positions = PoissonDiscSampling.GeneratePoints(dec.radius, new Vector2(world.WidthInBlocks, world.LengthInBlocks));
                foreach(var pos in positions)
                {
                    var position = pos.ToVector2Int();
                    if(position.x < world.WidthInBlocks && position.x >= 0 && position.y < world.LengthInBlocks && position.y >= 0)
                    {
                        if (layerMap[position.x, position.y] == dec.LayerID)
                        {
                            var cp = world.WorldToChunkPosition(position);
                            var wp = position.ToVector3(world.GetHeight(position.ToVector3Int()));
                            Instantiate(dec.decorationObject, wp, Quaternion.identity, world.Chunks[cp.x, cp.y].transform);
                        }
                    }
                }
            }
        }
    }

    public void PlaceResources()
    {

    }

    public void GenerateChunks()
    {
        Debug.Log("Generating World Chunks...");
        for (int x = 0; x < world.widthInChunks; x++)
        {
            for (int z = 0; z < world.lengthInChunks; z++)
            {
                var pos = new Vector3Int(x, 0, z);
                world.Chunks[x, z] = Chunk.CreateChunk(pos, GenerateChunk(pos), world.transform, world.worldMaterial);
            }
        }
        Debug.Log("Finished Generating Chunks");
    }

    public Block[,,] GenerateChunk(Vector3Int position)
    {
        //Debug.Log("Generating Chunk: " + position);

        Vector2Int offset = new Vector2Int(position.x * Chunk.width, position.z * Chunk.length);
        Block[,,] blocks = new Block[Chunk.width, Chunk.height, Chunk.length];

        for (int x = 0; x < Chunk.width; x++)
        {
            for (int z = 0; z < Chunk.length; z++)
            {
                var h = terrainMap[offset.x + x, offset.y + z];
                if (h < 0) h = 0;
                else if (h > Chunk.height-1) h = Chunk.height-1;

                int blockID = blockMap[position.x * Chunk.width + x, position.z * Chunk.length + z];

                //for (int i = layers.Length-1; i >= 0; i--)
                //{
                //    var layer = layers[i];
                //    if (h >= layer.height)
                //    {
                //        blockID = layer.BlockID;
                //        break;
                //    }
                //}

                for (int y = 0; y <= h; y++)
                {
                    blocks[x, y, z] = new Block(blockID, new Vector3Int(x, y, z));
                }
            }
        }

        return blocks;
    }

    public Texture2D GenerateWorldTexture()
    {
        InitWorldGen(seed);

        int width = Chunk.width * worldSize.x;
        int length = Chunk.length * worldSize.y;
        heightMap = NoiseGenerator.GenerateNoiseMap(width, length, worldSettings);

        if (useFalloff)
        {
            falloff = NoiseGenerator.GenerateFalloffMap(width, length);
        }

        Color[] colors = new Color[width * length];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                float height = heightMap[x, z];

                if (useFalloff)
                {
                    height -= falloff[x, z];
                }

                int blockID = layers[0].BlockID;

                for (int i = 1; i < layers.Length; i++)
                {
                    var layer = layers[i];
                    if (layer.height <= height * eccentricity)
                    {
                        blockID = layer.BlockID;
                    }
                    else
                    {
                        break;
                    }
                }

                colors[x + (z * length)] = BlockManager.Main.GetBlockColor(blockID);
            }
        }

        Texture2D texture = new Texture2D(width, length);
        texture.SetPixels(colors);
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        return texture;
    }

    public Vector2Int GetRandomPosition()
    {
        return new Vector2Int(Random.Range(0, world.WidthInBlocks), Random.Range(0, world.LengthInBlocks));
    }
}

[Serializable]
public struct WorldLayer
{
    public string name;
    public int BlockID;
    [Range(0, 1)]
    public float height;
    [Range(1, Chunk.height)]
    public int thickness;
}

[Serializable]
public struct Decoration
{
    public GameObject decorationObject;
    public float radius;
    public int LayerID;
}

