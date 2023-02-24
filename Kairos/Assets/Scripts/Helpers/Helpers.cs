using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Helpers
{


    public static Vector3 Vector2IntToVector3(Vector2Int vec, float y = 0)
    {
        Vector3 ret = new Vector3();
        ret.x = vec.x;
        ret.y = y;
        ret.z = vec.y;
        return ret;
    }

    public static void DeleteAllChildren(GameObject obj)
    {
        foreach(var c in obj.GetComponentsInChildren<Transform>())
        {
            if(c.gameObject != obj)
                GameObject.DestroyImmediate(c.gameObject);
        }
    }

    public static bool InSquareRadius(float radius, Vector2 center, Vector2 target)
    {
        return Mathf.Abs(center.x - target.x) <= radius && Mathf.Abs(center.y - target.y) <= radius;
    }
}
