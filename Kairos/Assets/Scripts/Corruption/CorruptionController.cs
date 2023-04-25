using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionController : MonoBehaviour
{
    [Range(0,1)]
    public float corruptionStrength = 0.1f;

    [Range(0, 1)]
    public float minimumCorruptionSpread = 0.3f;

    [Range(0, 1000)]
    public int RandomTick = 10;

    [Range(0, 1)]
    public float startingCorruption;

    public bool SpawnUnits = true;

    private void Start()
    {
        //var max = new Vector2Int(WorldController.Main.World.WidthInBlocks, WorldController.Main.World.LengthInBlocks);
        //var positions = GetRandomPositions((int)(startingCorruption * 100), Vector2Int.zero, max);
        //foreach(var pos in positions)
        //{
        //    WorldController.Main.World.SetCorruption(pos.x, pos.y, 1.0f);
        //}
    }

    private void Update()
    {
        foreach(Chunk c in WorldController.Main.World.Chunks)
        {
            var positions = GetRandomPositions(RandomTick, Vector2Int.zero, new Vector2Int(Chunk.width, Chunk.length));
            foreach (var position in positions)
            {
                var pos = new Vector2Int(c.Position.x * Chunk.width, c.Position.z * Chunk.length) + position;

                float corruptModifier = GetNeighborCorruptionStrength(pos.x, pos.y);
                float curr = WorldController.Main.World.GetCorruption(pos.x, pos.y);
                if (corruptModifier > minimumCorruptionSpread)
                {
                    float val = curr + corruptionStrength * Time.deltaTime;
                    WorldController.Main.World.SetCorruption(pos.x, pos.y, val);
                }
            }
        }

    }

    float GetNeighborCorruptionStrength(int x, int z)
    {
        float strength = 0;
        int c = 0;
        for(int i = -1; i <= 1; i++)
        {
            for(int j = -1; j <= 1; j++)
            {
                if (j == 0 && i == 0)
                    continue;

                var corr = WorldController.Main.World.GetCorruption(x + i, z + j);
                if(corr > 0)
                {
                    strength += corr;
                    c++;
                }
            }
        }
        return strength/c;
    }

    List<Vector2Int> GetRandomPositions(int count, Vector2Int min, Vector2Int max)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        for(int i = 0; i < count; i++)
        {
            var pos = new Vector2Int(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
            if (!positions.Contains(pos))
                positions.Add(pos);
            //else
            //    i--;
        }
        return positions;
    }

}
