using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public EntityManager entityManager;
    public Vector3 velocity = Vector3.zero;
    public Vector3 v1;
    public Vector3 v2;
    public Vector3 v3;
    public GameObject targetObject;
    public bool perch = false;
    //public List<GameObject> boids;
    // Start is called before the first frame update
    void Start()
    {
        //bolusManager = GetComponentInParent<BolusManager>();
        //var arry = FindObjectsOfType<Boid>();
        // foreach (var go in arry)
        // {
        //     boids.Add(go.gameObject);
        // }
        // boids.Remove(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (!perch)
        {
            v1 = Alignment();
            v2 = Seperation();
            v3 = Cohesion();
            Vector3 pc = (targetObject.transform.position - entityManager.centerVector).normalized * entityManager.followStr;

            if (!entityManager.flock)
            {
                velocity = Vector3.zero;

                velocity = velocity + v1 + v2 + v3 + pc;
            }

            if (entityManager.flock)
            {

                velocity = velocity + v1 + v2 + v3 + BoundPosition();
            }

            LimitVelocity();
            if (entityManager.twoD)
            {
                float X = transform.position.x;
                float Z = transform.position.z;
                transform.position = new Vector3(X, 0, Z);
                velocity.y = 0;
            }

            gameObject.transform.position += (velocity.normalized * entityManager.followSpeed * Time.deltaTime);
            //go.gameObject.transform.position += BoundPosition(go);
        }
    }
    public Vector3 Alignment()
    {
        //Vector3 perceivedCenter = (GameObject.Find("Center").transform.position - trueCenter).normalized * followStr;
        Vector3 perceivedCenter = entityManager.centerVector.normalized * entityManager.followStr;

        if (!entityManager.flock)
        {
            perceivedCenter = Vector3.zero;
        }
        foreach (Entity boid in entityManager.boids)
        {
            if (boid != this)
            {
                perceivedCenter += boid.transform.position;
            }
        }
        perceivedCenter = perceivedCenter / (entityManager.boids.Count - 1);
        return (perceivedCenter - transform.position) * entityManager.alignmentFactor;
    }
    public Vector3 Seperation()
    {
        // vector c is the displacement of each boid which is near by
        Vector3 c = Vector3.zero;
        foreach (Entity boid in entityManager.boids)
        {
            if (boid != this)
            {
                if (Vector3.Distance(boid.transform.position, transform.position) < entityManager.effectiveDistance)
                {
                    c = c - (boid.transform.position - transform.position);
                }
            }
        }
        return c;
    }
    public Vector3 Cohesion()
    {
        //percieved velocity
        Vector3 pv = Vector3.zero;
        foreach (Entity boid in entityManager.boids)
        {
            if (boid != this)
            {
                pv += boid.velocity;
            }
        }
        pv /= (entityManager.boids.Count - 1);
        return (pv - velocity) * entityManager.cohesionFactor;
    }

    public void LimitVelocity()
    {
        if (velocity.magnitude > entityManager.speedLimit)
        {
            velocity = (velocity / velocity.magnitude) * entityManager.speedLimit;
        }
    }

    public Vector3 BoundPosition()
    {
        Vector3 adjustedPath = Vector3.zero;
        if (transform.position.x < entityManager.min.x)
        {
            adjustedPath.x = (entityManager.min.x - transform.position.x);
        }
        else if (gameObject.transform.position.x > entityManager.max.x)
        {
            adjustedPath.x = (entityManager.max.x - transform.position.x);
        }

        if (gameObject.transform.position.y < entityManager.min.y)
        {
            adjustedPath.y = (entityManager.min.y - transform.position.y);
        }
        else if (gameObject.transform.position.y > entityManager.max.y)
        {
            adjustedPath.y = (entityManager.max.y - transform.position.y);
        }

        if (gameObject.transform.position.z < entityManager.min.z)
        {
            adjustedPath.z = (entityManager.min.z - transform.position.z);
        }
        else if (gameObject.transform.position.z > entityManager.max.z)
        {
            adjustedPath.z = (entityManager.max.z - transform.position.z);
        }
        return adjustedPath * entityManager.boundFactor;
    }

    public void Perching()
    {
        if (Vector3.Distance(entityManager.centerVector, targetObject.transform.position) <= entityManager.distanceFromTarget)
        {
            velocity = Vector3.zero;
            perch = true;
        }
        else
        {
            perch = false;
        }
    }
}
