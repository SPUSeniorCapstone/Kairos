using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeManager : MonoBehaviour
{
    public List<BiomeType> biomes;



    public int GetBiomeType(float humidity, float temperature, float height)
    {
        int biome = -1;

        for(int i = 0; i < biomes.Count; i++)
        {
            if (biomes[i].humidityMin < humidity && 
                biomes[i].temperatureMin < temperature && 
                biomes[i].altitude < height)
            {
                if (biome == -1)
                {
                    biome = i;
                }
                else if (biomes[i].humidityMin > biomes[biome].humidityMin || 
                    biomes[i].temperatureMin > biomes[biome].temperatureMin ||
                    biomes[i].altitude > biomes[biome].altitude)
                {
                    biome = i;
                }
            }
        }

        if(biome == -1)
        {
            biome = 0;
        }
        return biome;
    }
}
