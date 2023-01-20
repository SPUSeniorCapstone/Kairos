using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Tile
{
    //public int tileID;
    public Vector3Int position;
    public float height;
    public int biomeID;
    

    public Tile(Vector3Int position, float height, int biomeID)
    {
        this.position = position;
        this.height = height;
        this.biomeID = biomeID;
    }
}
