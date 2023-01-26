using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct BlockType
{
    public string name;
    [Disable]
    public int id;
    public int textureID;
    public Color color;
}
