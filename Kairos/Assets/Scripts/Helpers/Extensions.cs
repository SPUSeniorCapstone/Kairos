using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Extensions 
{
    public static Vector3 Flat(this Vector3 vector)
    {
        return new Vector3(vector.x, 0, vector.z);
    }

    public static Vector3Int ToVector3Int(this Vector3 vector)
    {
        return new Vector3Int((int)vector.x, (int)vector.y, (int)vector.z);
    }

    public static Vector3Int Flat(this Vector3Int vector)
    {
        return new Vector3Int(vector.x, 0, vector.z);
    }
}
