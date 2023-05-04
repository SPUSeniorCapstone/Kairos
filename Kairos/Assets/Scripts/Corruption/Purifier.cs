using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Purifier : MonoBehaviour, IComparable<Purifier>
{
    public bool cleans = false;

    [Range(1,100)]
    public int radius = 10;

    [Range(0,1)]
    public float strength = 0.2f;

    public bool InRange(Vector2Int pos)
    {
        Vector2Int position = WorldController.Main.WorldToBlockPosition(transform.position).ToVector2Int();
        var dist = Vector2Int.Distance(pos, position);
        return dist < radius;
    }

    private void Start()
    {
        GameController.Main.CorruptionController.RegisterPurifier(this);
    }

    public int CompareTo(Purifier other)
    {
        if(strength < other.strength)
        {
            return -1;
        }
        else if (strength > other.strength)
        {
            return 1;
        }
        return 0;
    }
}
