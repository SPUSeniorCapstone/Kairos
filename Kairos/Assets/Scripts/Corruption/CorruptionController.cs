using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class CorruptionController : MonoBehaviour
{
    public enum CorruptionMode { AVERAGE_SPREAD, POINT_SPREAD }

    public CorruptionMode corruptionMode = CorruptionMode.AVERAGE_SPREAD;

    [Range(0,1)]
    public float corruptionStrength = 0.1f;

    [Range(0, 1)]
    public float purificationStrength = 0.1f;

    [Range(0, 1)]
    public float minimumCorruptionSpread = 0.3f;

    [Range(0, 1000)]
    public int RandomTick = 10;

    [Range(0, 1)]
    public float startingCorruption;

    public bool SpawnUnits = true;


    public bool DoCorruptionVeins = true;
    [Range (0, 1000)]
    public int veinTick = 1;
    public int maxVeins = 10;
    public int maxStepsPerVein = 10;

    [Range(0, 100)]
    public float veinChance = 1;

    public List<Purifier> purifiers = new List<Purifier>();

    public bool doCorruptionDamage = true;
    public float damageTick = 2;
    public float corruptionDamage = 20;

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
        if(corruptionMode == CorruptionMode.AVERAGE_SPREAD)
        {
            AverageSpreadUpdate();
        }
        else
        {
            PointSpreadUpdate();
        }

        if (DoCorruptionVeins)
        {
            CorruptionVeins();
        }
    }

    public void PointSpreadUpdate()
    {
        foreach (Chunk c in WorldController.Main.World.Chunks)
        {
            var positions = GetRandomPositions(RandomTick, Vector2Int.zero, new Vector2Int(Chunk.width, Chunk.length));
            foreach (var position in positions)
            {
                var pos = new Vector2Int(c.Position.x * Chunk.width, c.Position.z * Chunk.length) + position;

                Purifier purifier = null;
                foreach(Purifier pure in purifiers)
                {
                    if ((purifier == null || pure.strength > purifier.strength) && pure.InRange(pos))
                    {
                        purifier = pure;
                    }
                }


                if (purifier != null)
                {
                    var currCorr = WorldController.Main.World.GetCorruption(pos.x, pos.y);
                    WorldController.Main.World.SetCorruption(pos.x, pos.y, currCorr - purifier.strength * purificationStrength);
                    continue;
                }


                var corr = WorldController.Main.World.GetCorruption(pos.x, pos.y);

                if (corr < minimumCorruptionSpread)
                {
                    continue;
                }

                foreach (var point in GetNeighbors(pos.x, pos.y))
                {
                    var currCorr = WorldController.Main.World.GetCorruption(point.x, point.y);
                    WorldController.Main.World.SetCorruption(point.x, point.y, currCorr + corr * corruptionStrength);
                }

            }



        }
    }

    public void RegisterPurifier(Purifier purifier)
    {
        purifiers.Add(purifier);
    }

    public void AverageSpreadUpdate()
    {
        foreach (Chunk c in WorldController.Main.World.Chunks)
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
                if(corr >= 0)
                {
                    strength += corr;
                    c++;
                }
            }
        }
        return strength/c;
    }

    List<Vector2Int> GetNeighbors(int x, int z)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (j == 0 && i == 0)
                    continue;

                if(IsValidPosition(x + i, z + j))
                {
                    neighbors.Add(new Vector2Int(x + i, z + j));
                }
            }
        }
        return neighbors;
    }

    bool IsValidPosition(int x, int z)
    {
        if(x < 0 || x > WorldController.Main.World.WidthInBlocks || z < 0 || z > WorldController.Main.World.LengthInBlocks)
        {
            return false;
        }
        return true;
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

    void CorruptionVeins()
    {
        var positions = GetRandomPositions(veinTick, Vector2Int.zero, new Vector2Int(WorldController.Main.World.WidthInBlocks, WorldController.Main.World.LengthInBlocks));

        foreach(var pos in positions)
        {
            if(Random.Range(0.0f,100) < veinChance || WorldController.Main.World.GetCorruption(pos.x,pos.y) < 1.0f)
            {
                continue;
            }

            int veinsPerNode = Random.Range(1, maxVeins);
            int stepsPerVein = Random.Range(1, maxStepsPerVein);


            for (int i = 0; i < veinsPerNode; i++)
            {
                Vector2Int prev = pos;
                Vector2Int dir = Vector2Int.zero;
                float curr = 1.0f;
                int c = 0;
                while (curr > 0 && c < stepsPerVein)
                {
                    curr -= 1 / stepsPerVein;
                    Vector2Int next = prev + dir + new Vector2Int(Random.Range(-1, 2), Random.Range(-1, 2));
                    if (prev == next || !IsValidPosition(next.x, next.y))
                    {
                        c++;
                        continue;
                    }

                    var nextCorr = WorldController.Main.World.GetCorruption(next.x, next.y);
                    WorldController.Main.World.SetCorruption(next.x, next.y, curr + nextCorr);

                    dir = next - prev;
                    dir /= (int)dir.magnitude;

                    prev = next;
                    c++;
                }

            }
        }
    }
}
