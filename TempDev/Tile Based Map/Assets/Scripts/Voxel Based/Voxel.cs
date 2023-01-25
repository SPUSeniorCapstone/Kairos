using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Voxel
{
    public int blockID;
    public Vector3Int position;

    public Voxel(int blockID, Vector3Int position)
    {
        this.blockID = blockID;
        this.position = position;
    }
}
