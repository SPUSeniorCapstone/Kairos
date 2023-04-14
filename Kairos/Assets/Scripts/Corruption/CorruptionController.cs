using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionController : MonoBehaviour
{
    [Range(0,1)]
    public float corruptionStrength = 0.2f;

    private void Update()
    {
        for(int x = 0; x < WorldController.Main.World.WidthInBlocks, x++){
            for (int z = 0; z < WorldController.Main.World.LengthInBlocks)
            {
                float corruptModifier = 0;
                foreach(Block b in GetBlockNeighbors(x, z))
                {
                    if (b.corruption > 0.5f)
                    {
                        corruptModifier += 1;
                    }
                }

                corruptModifier /= 100;

                WorldController.Main.World
            }
        }
    }

    float GetNeighborCorruptionStrength(int x, int z)
    {
        float strength = 0;
        for(int i = -1; i <= 1; i++)
        {
            for(int j = -1; j <= 1; j++)
            {
                if (j == 0 && i == 0)
                    continue;

                blocks.Add(WorldController.Main.World.GetSurfaceBlock(x + i, z + i));
            }
        }
        return 0;
    }
}
