using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cuve : MonoBehaviour
{
    void Update()
    {
        var t = Terrain.activeTerrain.SampleHeight(transform.position);
        var n = transform.position;
        n.y = t + 0.5f;
        transform.position = n;
    }
}
