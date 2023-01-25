using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BiomeType 
{
    public int ID;
    public string Name;
    public int textureID;
    public Color mapColor;

    [Range(0,1)]
    public float temperatureMin = 0;

    [Range(0, 1)]
    public float humidityMin = 0;

    [Range(0, 1)]
    public float altitude;
}
