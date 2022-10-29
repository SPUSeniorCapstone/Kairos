using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    public static Vector2Int ToVector2Int(Vector2 vec)
    {
        Vector2Int ret = new Vector2Int();
        ret.x = Mathf.FloorToInt(vec.x);
        ret.y = Mathf.FloorToInt(vec.y);
        return ret;
    }

    public static Vector3 Vector2IntToVector3(Vector2Int vec, float y = 0)
    {
        Vector3 ret = new Vector3();
        ret.x = vec.x;
        ret.y = y;
        ret.z = vec.y;
        return ret;
    }
}
