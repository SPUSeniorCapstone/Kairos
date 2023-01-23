using System.Collections.Generic;
using UnityEngine;

public class BolusManager : MonoBehaviour
{
    public List<Bolus> boids;
    public Vector3 v1;
    public Vector3 v2;
    public Vector3 v3;
    public int speedLimit;
    public Vector3 max, min;
    int Xmin, Xmax, Ymin, Ymax, Zmin, Zmax;
    public Vector3 trueCenter;
    public float followStr;
    public float followSpeed;
    public bool perch = false;
    public bool flock;

    public bool twoD = false;

    [Range(0,0.25f)]
    public float alignmentFactor;

    [Range(0, 50f)]
    public float effectiveDistance;

    [Range(0, 1f)]
    public float cohesionFactor;

    [Range (0, 1f)]
    public float boundFactor;

    // Start is called before the first frame update
    void Start()
    {
        var arry = GetComponentsInChildren<Bolus>();
        foreach (var go in arry)
        {
            boids.Add(go);
        }
   
    }

    // Update is called once per frame
    void Update()
    {
        // percieved center
        float x = 0;
        float y = 0;
        float z = 0;
        foreach (Bolus tree in boids)
        {
            //Perching(tree);
            x += tree.transform.position.x;
            y += tree.transform.position.y;
            z += tree.transform.position.z;

        }
        x /= boids.Count;
        y /= boids.Count;
        z /= boids.Count;
        trueCenter = new Vector3(x, y, z);
        if (!perch)
        {

            foreach (var go in boids)
            {
                
                v1 = Alignment(go);
                v2 = Seperation(go);
                v3 = Cohesion(go);
                Vector3 pc = (GameObject.Find("Center").transform.position - trueCenter).normalized * followStr;

                if (!flock)
                {
                    go.velocity = Vector3.zero;

                    go.velocity = go.velocity + v1 + v2 + v3 + pc;
                }
               

                if (flock)
                {

                    go.velocity = go.velocity + v1 + v2 + v3 + BoundPosition(go);
                }
                LimitVelocity(go);
                if (twoD)
                {
                    float X = go.transform.position.x;
                    float Z = go.transform.position.z;
                    go.transform.position = new Vector3(X, 0, Z);
                    go.velocity.y = 0;
                }

                go.gameObject.transform.position += (go.velocity.normalized * followSpeed * Time.deltaTime);
                //go.gameObject.transform.position += BoundPosition(go);



            }
        }
    }

    public Vector3 Alignment(Bolus boid)
    {
        //Vector3 pc = (GameObject.Find("Center").transform.position - trueCenter).normalized * followStr;
        Vector3 pc =  trueCenter.normalized * followStr;
        if (!flock)
        {
            pc = Vector3.zero;
        }
        foreach(Bolus boid1 in boids)
        {
            if (boid1 != boid)
            {
                pc += boid1.transform.position;
            }
        }
        pc = pc / (boids.Count - 1);
        return (pc - boid.transform.position) * alignmentFactor;
    }
    public Vector3 Seperation(Bolus boid)
    {
        Vector3 c = Vector3.zero;
        foreach(Bolus boid1 in boids)
        {
            if (boid1 != boid)
            {
                if (Vector3.Distance(boid1.transform.position, boid.transform.position) < effectiveDistance){
                    c = c - (boid1.transform.position - boid.transform.position);
                }
            }
        }
        return c;
    }
    public Vector3 Cohesion(Bolus boid)
    {
        //percieved velocity
        Vector3 pv = Vector3.zero;
        foreach (Bolus boid1 in boids)
        {
            if (boid1 != boid)
            {
                pv += boid.velocity;
            }
        }
        pv /= (boids.Count - 1);
        return (pv - boid.velocity) * cohesionFactor;
    }

    public void LimitVelocity(Bolus boid)
    {
        if (boid.velocity.magnitude > speedLimit)
        {
            boid.velocity = (boid.velocity / boid.velocity.magnitude) * speedLimit;
        }
    }

    public Vector3 BoundPosition(Bolus boid)
    {
        Vector3 adjustedPath = Vector3.zero;
        if (boid.gameObject.transform.position.x < min.x)
        {
            adjustedPath.x = (min.x - boid.transform.position.x);
        } 
        else if (boid.gameObject.transform.position.x > max.x)
        {
            adjustedPath.x = (max.x - boid.transform.position.x);
        }

        if (boid.gameObject.transform.position.y < min.y)
        {
            adjustedPath.y = ( min.y - boid.transform.position.y);
        }
        else if (boid.gameObject.transform.position.y > max.y)
        {
            adjustedPath.y = (max.y - boid.transform.position.y);
        }

        if (boid.gameObject.transform.position.z < min.z)
        {
            adjustedPath.z = ( min.z - boid.transform.position.z);
        }
        else if (boid.gameObject.transform.position.z > max.z)
        {
            adjustedPath.z = (max.z - boid.transform.position.z);
        }


        return adjustedPath * boundFactor;
    }

    public void Perching(Bolus boid)
    {
        if (Vector3.Distance(trueCenter, GameObject.Find("Center").transform.position) <= 4) 
        {
            boid.velocity = Vector3.zero;
            perch = true;
        }
        else
        {
            perch = false;
        }
    }
}
