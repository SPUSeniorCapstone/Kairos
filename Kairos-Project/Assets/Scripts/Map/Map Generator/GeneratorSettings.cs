using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "generatorsettings", menuName = "ScriptableObjects/Map Generator Settings", order = 1)]
public class GeneratorSettings : ScriptableObject
{
    public int width, length;
    [Range(0, 0.3f)]
    public float scale, eccentricity;
    [Range(0, 1)]
    public float layerDivider = 0.5f;
    public float offsetX, offsetZ;

}
