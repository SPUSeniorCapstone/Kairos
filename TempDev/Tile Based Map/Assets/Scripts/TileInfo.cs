using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TileInfo
{
    public Vector3Int position;
    public float height;
    ///Biome...
    ///speedBuff/debuff?
    ///water?
    ///angle?
    ///

    public TileInfo(Vector3Int pos, float h)
    {
        height = h;
        position = pos;
    }
}
