using Unity.VisualScripting;
using UnityEngine;

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

    public static void DeleteAllChildren(this GameObject obj)
    {
        foreach (var c in obj.GetComponentsInChildren<Transform>())
        {
            if (c != null && c.gameObject != obj)
                GameObject.DestroyImmediate(c.gameObject);
        }
    }

    public static bool InSquareRadius(float radius, Vector2 center, Vector2 target)
    {
        //Debug.Log(center.x+ " - "+ target.x+ " <= "+ radius + " and " + center.y + " - " + target.y + " <= " + radius);
        return Mathf.Abs(center.x - target.x) <= radius && Mathf.Abs(center.y - target.y) <= radius;
    }

    public static string FloatToString(float val, int round = 1)
    {
        string ret = ((int)val).ToString();
        ret += '.';
        ret += (int)(val * Mathf.Pow(10, round));
        return ret;
    }
}
