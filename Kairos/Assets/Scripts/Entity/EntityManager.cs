using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public List<Entity> boids;

    public int speedLimit;
    public Vector3 max, min;
    int Xmin, Xmax, Ymin, Ymax, Zmin, Zmax;
    public Vector3 centerVector;
    public float followStr;
    public float followSpeed;
    public GameObject centerObj;
    public GameObject groupTargetObj;

    public bool flock;

    public bool twoD = false;
    // old max 0.25f
    [Range(0, 1f)]
    public float alignmentFactor;

    [Range(0, 50f)]
    public float effectiveDistance;

    [Range(0, 1f)]
    public float cohesionFactor;

    [Range(0, 1f)]
    public float boundFactor;

    [Range(0, 100f)]
    public float distanceFromTarget;

    // Start is called before the first frame update
    void Start()
    {
        var arry = GetComponentsInChildren<Entity>();
        foreach (var go in arry)
        {
            boids.Add(go);
            go.targetObject = groupTargetObj;
        }

    }

    // Update is called once per frame
    void Update()
    {
        CalculateCenter();
    }

    public void CalculateCenter()
    {
        float x = 0;
        float y = 0;
        float z = 0;
        foreach (Entity boid in boids)
        {
            boid.Perching();
            x += boid.transform.position.x;
            y += boid.transform.position.y;
            z += boid.transform.position.z;
            //avgAlignment += Alignment(tree);

        }
        //avgAlignment = avgAlignment.normalized;
        x /= boids.Count;
        //y /= boids.Count;
        y = 0f;
        z /= boids.Count;
        centerVector = new Vector3(x, y, z);
        centerObj.transform.position = centerVector;
        //centerObj.transform.LookAt(pos, transform.up);

    }

}
