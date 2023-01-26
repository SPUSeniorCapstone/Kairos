using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Block
{
    public int blockID;
    public Vector3Int position;

    public Block(int blockID, Vector3Int position)
    {
        this.blockID = blockID;
        this.position = position;
    }
}
