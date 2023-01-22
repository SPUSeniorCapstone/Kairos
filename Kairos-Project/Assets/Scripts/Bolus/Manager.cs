using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Manager : Entity
{
    public List<Boid> boids;
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
    public float repulseValue;
    public GameObject targetObj;
    public Vector3 pos;
    public bool goeth;
    //public Vector2[] path;
    public bool selected;
    public Queue<Vector3> path;
    public bool drawPath;

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
        path = new Queue<Vector3>();
        var arry = GetComponentsInChildren<Boid>();
        foreach (var go in arry)
        {
            boids.Add(go);
        }
   
    }

    // Update is called once per frame
    void Update()
    {
        if (targetObj != null &&  targetObj.GetComponent<Battalion>())
        {
            pos = targetObj.GetComponent<Battalion>().GetCenter();
        }
        else if (targetObj != null)
        {
            pos = targetObj.transform.position;
        }

        // percieved center
        float x = 0;
        float y = 0;
        float z = 0;
        foreach (Boid tree in boids)
        {
            Perching(tree);
            x += tree.transform.position.x;
            y += tree.transform.position.y;
            z += tree.transform.position.z;

        }
        x /= boids.Count;
        y /= boids.Count;
        z /= boids.Count;
        trueCenter = new Vector3(x, y, z);
        if (!perch && goeth)
        {

            foreach (var go in boids)
            {
                
                v1 = Alignment(go);
                v2 = Seperation(go);
                v3 = Cohesion(go);
                Vector3 pc = (pos - trueCenter).normalized * followStr;

                if (!flock)
                {
                    go.velocity = Vector3.zero;

                    go.velocity = go.velocity + v1 + v2 + v3 + pc;
                }
               

                if (flock)
                {

                    go.velocity = go.velocity + v1 + v2 + v3;
                }
                LimitVelocity(go);
                if (twoD)
                {
                    float X = go.transform.position.x;
                    float Z = go.transform.position.z;
                    go.transform.position = new Vector3(X, 0, Z);
                    go.velocity.y = 0;
                }
                go.velocity += avoidWall(go) * repulseValue;
                go.gameObject.transform.position += (go.velocity.normalized * followSpeed * Time.deltaTime);
                //go.gameObject.transform.position += BoundPosition(go);



            }
        }
    }

    public Vector3 Alignment(Boid boid)
    {
        Vector3 pc = (pos - trueCenter).normalized * followStr;
        if (!flock)
        {
            pc = Vector3.zero;
        }
        foreach(Boid boid1 in boids)
        {
            if (boid1 != boid)
            {
                pc += boid1.transform.position;
            }
        }
        pc = pc / (boids.Count - 1);
        return (pc - boid.transform.position) * alignmentFactor;
    }
    public Vector3 Seperation(Boid boid)
    {
        Vector3 c = Vector3.zero;
        foreach(Boid boid1 in boids)
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
    public Vector3 Cohesion(Boid boid)
    {
        //percieved velocity
        Vector3 pv = Vector3.zero;
        foreach (Boid boid1 in boids)
        {
            if (boid1 != boid)
            {
                pv += boid.velocity;
            }
        }
        pv /= (boids.Count - 1);
        return (pv - boid.velocity) * cohesionFactor;
    }

    public void LimitVelocity(Boid boid)
    {
        if (boid.velocity.magnitude > speedLimit)
        {
            boid.velocity = (boid.velocity / boid.velocity.magnitude) * speedLimit;
        }
    }

    public Vector3 BoundPosition(Boid boid)
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

    public void Perching(Boid boid)
    {
        if (Vector3.Distance(trueCenter, pos) <= 10) 
        {
            boid.velocity = Vector3.zero;
            perch = true;
            if (path.Count != 0)
            {
                pos = path.Dequeue();
            }
        }
        else
        {
            perch = false;
        }
    }
    // calc velcoty, then call this function and then adsd to velcoity
    public Vector3 avoidWall(Boid boid)
    {
        RaycastHit hit;
        Vector3 dir = boid.velocity.normalized * followSpeed * Time.deltaTime * 100;
        if (Physics.Raycast(boid.transform.position, dir.normalized, out hit, dir.magnitude, LayerMask.GetMask("Terrain", "Structure"))){
            Debug.Log("NORMAL DETECTED" + hit.collider.name);

            return hit.normal * (1/Vector3.Distance(boid.transform.position, hit.point) + 10);
        }
        else
        {
            return Vector3.zero;
        }
    }

    public void Select()
    {
        selected = true;
        foreach (Boid boid in boids)
        {
            if (!boid.select)
                boid.SetSelectedVisible(true);
            //playerController.selectedEntityList.Add(unit);
        }
    }
    // worth making a function for? versus direct access to public bool?
    public void Deselect()
    {
        selected = false;
        foreach (Boid boid in boids)
        {
            boid.SetSelectedVisible(false);
            //playerController.selectedEntityList.Add(unit);
        }
    }
}
