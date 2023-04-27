using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.UIElements;
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






    [Header("Noise Settings Objects")]
    public NoiseGenerator.NoiseSettings worldSettings;
    public NoiseGenerator.NoiseSettings forestSettings;

    [Header("General Generation Settings")]
    public int seed;
    public bool useLayerHeights = true;
    public bool useFalloff = false;

    [Range(1f, Chunk.height)]
    public float eccentricity;

    public Vector2Int worldSize = new Vector2Int(8, 8);

    //World Settings Maps
    float[,] falloff;
    float[,] heightMap;
    int[,] terrainMap;
    int[,] layerMap;
    int[,] blockMap;
    float[,] corruptionMap;

    [Header("Player Spawn Generation")]
    public Vector3Int strongholdPos;
    public float strongholdRadius = 10;
    public int strongholdSpawnLayer = 4;

    [Header("Corruption Generation")]
    public int numNodes = 10;
    public float corruptionNodeDensityRadius = 20;
    [Range(0,100)]
    public int veinsPerNode = 10;
    [Range(0,1000)]
    public int stepsPerVein = 100;
    [HideInInspector]
    public List<Vector3Int> corruptionNodePositions = new List<Vector3Int>();
    public int nodeSpawnLayer = 4;
    [Range(0,100000)]
    public int simulateCorruptionSteps = 10000;


    [Header("Tree Decorations")]
    public List<DecorationObject> treePrefabs;
    public float forestRadius;
    public int treeSpawnLayer = 4;
    [Range(0, 1)]
    public float forestMin;

    [Header("World Layers")]
    public WorldLayer[] layers;

    public void InitWorldGen(int seed)
    {
        UnityEngine.Random.InitState(seed);
        worldSettings.seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
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
        int max = 50;
        int count = 0;
        do
        {
            count++;
            strongholdPos = GetRandomPosition().ToVector3Int();
        } while (layerMap[strongholdPos.x, strongholdPos.z] != strongholdSpawnLayer && count < max);
        if(count == max)
        {
            Debug.LogError("Could Not Generate Stronghold");
        }
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

        for(int i = 0; i < simulateCorruptionSteps; i++)
        {
            var positions = GetRandomPositions(100);
            foreach (var pos in positions)
            {
                float corruptModifier = GetNeighborCorruptionStrength(pos.x, pos.y);
                float curr = corruptionMap[pos.x, pos.y];
                if (corruptModifier > 0.25f)
                {
                    float val = curr + 0.1f;
                    corruptionMap[pos.x, pos.y] =  val;
                }
            }
        }

        for(int i = (int) -(strongholdRadius + 1); i < strongholdRadius; i++)
        {
            for(int j = (int)-(strongholdRadius+1); j < strongholdRadius; j++)
            {
                var pos = strongholdPos.ToVector2Int() + new Vector2Int(i, j);
                if(pos.x >= 0 && pos.x < world.WidthInBlocks && pos.y >= 0 && pos.y < world.LengthInBlocks)
                {
                    if (Vector2Int.Distance(strongholdPos.ToVector2Int(), pos) < strongholdRadius)
                    {
                        corruptionMap[pos.x, pos.y] = 0;
                    }
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

        float GetNeighborCorruptionStrength(int x, int z)
        {
            float strength = 0;
            int c = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (j == 0 && i == 0)
                        continue;

                    var X = x + i;
                    var Z = z + j;
                    if (X >= world.WidthInBlocks || X < 0 || Z >= world.LengthInBlocks || Z < 0)
                        continue;

                    var corr = corruptionMap[x + i, z + j];
                    if (corr >= 0)
                    {
                        strength += corr;
                        c++;
                    }
                }
            }
            return strength / c;
        }
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
            var position = new Vector3(pos.x, 0, pos.y).ToVector3Int();
            if (Vector3Int.Distance(position, strongholdPos) > strongholdRadius * 1.5 && layerMap[position.x,position.z] == 4)
            {
                corruptionNodePositions.Add(new Vector3(pos.x, 0, pos.y).ToVector3Int());
            }
        }
        Debug.Log("Placing Corruption Nodes");
    }

    public void PlaceDecorations()
    {
        //Spawn Trees
        if (treePrefabs != null)
        {
            float[,] forestmap = NoiseGenerator.GenerateNoiseMap(world.WidthInBlocks, world.LengthInBlocks, forestSettings);
            var positions = PoissonDiscSampling.GeneratePoints(forestRadius, new Vector2(world.WidthInBlocks, world.LengthInBlocks));
            foreach (var pos in positions)
            {
                var position = pos.ToVector2Int();


                if (position.x < world.WidthInBlocks && position.x >= 0 && position.y < world.LengthInBlocks && position.y >= 0)
                {
                    if (layerMap[position.x, position.y] == treeSpawnLayer && forestmap[position.x, position.y] > forestMin &&
                        Vector3Int.Distance(position.ToVector3Int(), strongholdPos) > strongholdRadius * 0.75)
                    {
                        var cp = world.WorldToChunkPosition(position);
                        var wp = position.ToVector3(world.GetHeight(position.ToVector3Int()));
                        var obj = Instantiate<DecorationObject>(treePrefabs[Random.Range(0,treePrefabs.Count)], wp, Quaternion.identity, world.Chunks[cp.x, cp.y].transform);
                        obj.position = position - new Vector2Int(cp.x * Chunk.width, cp.y * Chunk.length);
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


        Color[] colors = new Color[world.WidthInBlocks * world.LengthInBlocks];

        for (int x = 0; x < world.WidthInBlocks; x++)
        {
            for (int z = 0; z < world.LengthInBlocks; z++)
            {
                var blockID = layers[layerMap[x, z]].BlockID;

                Color color = Color.Lerp(BlockManager.Main.GetBlockColor(blockID), new Color(1, 0, 1, 1), corruptionMap[x, z]);
                colors[x + (z * world.LengthInBlocks)] =  color;
            }
        }

        Texture2D texture = new Texture2D(world.WidthInBlocks, world.LengthInBlocks);
        texture.SetPixels(colors);
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        return texture;
    }

    public Vector2Int GetRandomPosition()
    {
        return new Vector2Int(Random.Range(0, world.WidthInBlocks), Random.Range(0, world.LengthInBlocks));
    }

    public List<Vector2Int> GetRandomPositions(int count)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        for(int i = 0; i < count; i++)
        {
            positions.Add(GetRandomPosition());
        }
        return positions;
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


