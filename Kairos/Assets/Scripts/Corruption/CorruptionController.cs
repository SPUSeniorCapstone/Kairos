using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionController : MonoBehaviour
{
    [Range(0,1)]
    public float corruptionStrength = 0.1f;

    [Range(0, 1)]
    public float corruptionSpread = 0.3f;

    [Range(0, 10000)]
    public int RandomTick = 1000;

    private void Start()
    {
        var positions = GetRandomPositions(10);
        foreach(var pos in positions)
        {
            WorldController.Main.World.SetCorruption(pos.x, pos.y, 1.0f);
        }
    }

    private void Update()
    {
        var positions = GetRandomPositions(RandomTick);
        foreach(var pos in positions){ 
            float corruptModifier = GetNeighborCorruptionStrength(pos.x,pos.y);
            float curr = WorldController.Main.World.GetCorruption(pos.x, pos.y);
            if(corruptModifier > corruptionSpread)
            {
                float val = curr + corruptionStrength * Time.deltaTime;
                WorldController.Main.World.SetCorruption(pos.x, pos.y, val);
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

    List<Vector2Int> GetRandomPositions(int count)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        for(int i = 0; i < count; i++)
        {
            positions.Add(new Vector2Int(Random.Range(0, WorldController.Main.World.WidthInBlocks), Random.Range(0, WorldController.Main.World.LengthInBlocks)));
        }
        return positions;
    }
}
