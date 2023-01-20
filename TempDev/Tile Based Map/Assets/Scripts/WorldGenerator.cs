using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public float scale;
    public float terrainEccentricity;

    public int seed;

    [Range(0f, 1f)]
    public float seaLevel;

    Vector3Int terrainOffset;
    Vector3Int tempOffset;
    Vector3Int humidityOffset;

    public void InitWorldGen(int seed)
    {
        Random.InitState(seed);
        terrainOffset = new Vector3Int(Random.Range(-10000, 10000), Random.Range(-10000, 10000));
        tempOffset = new Vector3Int(Random.Range(-10000, 10000), Random.Range(-10000, 10000));
        humidityOffset = new Vector3Int(Random.Range(-10000, 10000), Random.Range(-10000, 10000));
    }

    public void GenerateWorld(World world)
    {
        world.seed = seed;
        InitWorldGen(seed);

        for (int x = 0; x < world.width; x++)
        {
            for (int z = 0; z < world.length; z++)
            {
                var pos = new Vector3Int(x, 0, z);
                if (world.Chunks[x,z] == null)
                {
                    world.Chunks[x, z] = Chunk.CreateChunk(pos, GenerateChunk(pos), world.transform, world.material);
                }
                else
                {
                    world.Chunks[x, z].SetChunk(pos, GenerateChunk(pos), world.material);
                }
            }
        }

        for (int x = 0; x < world.width; x++)
        {
            for (int z = 0; z < world.length; z++)
            {
                world.Chunks[x,z].ReloadMesh();
            }
        }
    }

    public Tile[,] GenerateChunk(Vector3Int position)
    {
        int terrainOffsetX = terrainOffset.x + (position.x * Chunk.width);
        int terrainOffsetZ = terrainOffset.z + (position.z * Chunk.length);

        int tempOffsetX = tempOffset.x + (position.x * Chunk.width);
        int tempOffsetZ = tempOffset.z + (position.z * Chunk.length);

        int humidityOffsetX = humidityOffset.x + (position.x * Chunk.width);
        int humidityOffsetZ = humidityOffset.z + (position.z * Chunk.length);

        Tile[,] tiles = new Tile[Chunk.width, Chunk.length];

        for(int x = 0; x < Chunk.width; x++)
        {
            for(int z = 0; z < Chunk.length; z++)
            {
                float height = Mathf.PerlinNoise((x + terrainOffsetX) * scale, (z + terrainOffsetZ) * scale);
                float temp = Mathf.PerlinNoise((x + tempOffsetX) * scale, (z + tempOffsetZ) * scale);
                float humidity = Mathf.PerlinNoise((x + humidityOffsetX) * scale, (z + humidityOffsetZ) * scale);
                int biome;
                if(height < seaLevel)
                {
                    biome = 4;
                    height = 0;
                }
                else
                {
                    biome = WorldController.main.BiomeManager.GetBiomeType(humidity, temp, height);
                    //height = 1;
                }
                tiles[x, z] = new Tile(new Vector3Int(x,0,z), height * terrainEccentricity, biome);
            }
        }

        return tiles;

    }



}
