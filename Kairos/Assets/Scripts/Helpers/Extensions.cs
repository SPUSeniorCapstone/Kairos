using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Extensions 
{
    public static Vector2Int ToVector2Int(this Vector2 vec)
    {
        Vector2Int ret = new Vector2Int();
        ret.x = Mathf.FloorToInt(vec.x);
        ret.y = Mathf.FloorToInt(vec.y);
        return ret;
    }

    /// <summary>
    /// Converts Vector3 to Vector2 where y is assumed to be 0 
    /// <para>(x=x, y=z)</para>
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    public static Vector2 ToVector2(this Vector3 vec)
    {
        return new Vector2(vec.x, vec.z);
    }

    public static Vector2Int ToVector2Int(this Vector3Int vec)
    {
        Vector2Int ret = new Vector2Int();
        ret.x = vec.x;
        ret.y = vec.z;
        return ret;
    }

    public static Vector3 SetZ(this Vector3 vector, float z)
    {
        vector.z = z;
        return vector;
    }

    public static Vector3 Flat(this Vector3 vector)
    {
        return new Vector3(vector.x, 0, vector.z);
    }

    public static Vector3Int ToVector3Int(this Vector3 vector)
    {
        return new Vector3Int((int)vector.x, (int)vector.y, (int)vector.z);
    }

    public static Vector2Int ToVector2Int(this Vector3Int vector)
    {
        return new Vector2Int(vector.x, vector.z);
    }

    public static Vector3Int Flat(this Vector3Int vector)
    {
        return new Vector3Int(vector.x, 0, vector.z);
    }

    public static Vector3 Abs(this Vector3 vector)
    {
        return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
    }

    public static Vector3 ClosestPointOnBounds(this Bounds bounds, Vector3 position)
    {
        if (!bounds.Contains(position))
        {
            return bounds.ClosestPoint(position);
        }
        else
        {
            Vector3 dist = bounds.size - (bounds.center - position).Abs() ;
            if(dist.x < dist.y && dist.x < dist.z)
            {
                if(bounds.center.x - position.x < 0)
                {
                    return new Vector3(bounds.min.x, position.y, position.z);
                }
                else
                {
                    return new Vector3(bounds.max.x, position.y, position.z);
                }
            }
            else if (dist.y < dist.z)
            {
                if (bounds.center.y - position.y < 0)
                {
                    return new Vector3(position.x, bounds.min.y, position.z);
                }
                else
                {
                    return new Vector3(position.x, bounds.max.x, position.z);
                }
            }
            else
            {
                if (bounds.center.y - position.y < 0)
                {
                    return new Vector3(position.x, position.y, bounds.min.z);
                }
                else
                {
                    return new Vector3(position.x, position.y, bounds.max.z);
                }
            }
        }
    }
}
