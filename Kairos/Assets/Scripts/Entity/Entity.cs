using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public CommandGroup CommandGroup;
    public Vector3 velocity = Vector3.zero;

    public GameObject targetObject;
    public Vector3 targetPos;
    public float distance;
    public bool perch = false;
    public bool idle = true;

    [Range(0, 50f)]
    public float effectiveDistance;

    //[Range(0, 1f)]
    //public float cohesionFactor;

    //[Range(0, 1f)]
    //public float boundFactor;

    //[Range(0, 100f)]
    //public float distanceFromTarget;
    //==






















    // Start is called before the first frame update
    void Start()
    {
        GameController.Main.EntityController.AddEntity(this);

        // delete this later
        //CommandGroup = GetComponentInParent<CommandGroup>();
        //if (CommandGroup == null)
        //{
        //    CommandGroup = GetComponent<CommandGroup>();
        //}
    }

    private void OnDestroy()
    {
        GameController.Main.EntityController.RemoveEntity(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (!perch && !idle && CommandGroup != null)
        {
            Vector3 v1 = Vector3.zero;
            Vector3 v2 = Vector3.zero;
            Vector3 v3 = Vector3.zero;

            distance = Vector3.Distance(CommandGroup.centerVector, targetObject.transform.position);
            
            if (CommandGroup.entities.Count > 1)
            {
                v1 = Alignment();
                v2 = Seperation();
                v3 = Cohesion();
            }
            Vector3 pc = (targetPos - CommandGroup.centerVector).normalized * CommandGroup.followStr;
            if (!CommandGroup.flock)
            {
                velocity = Vector3.zero;

                velocity = velocity + v1 + v2 + v3 + pc;
            }

            if (CommandGroup.flock)
            {
                //idle = false;
                velocity = velocity + v1 + v2 + v3 + BoundPosition();
            }

            LimitVelocity();
            if (CommandGroup.twoD)
            {
                float X = transform.position.x;
                float Z = transform.position.z;
                transform.position = new Vector3(X, 0, Z);
                velocity.y = 0;
            }

            gameObject.transform.position += (velocity.normalized * CommandGroup.followSpeed * Time.deltaTime);
            //go.gameObject.transform.position += BoundPosition(go);
        }
        else
        {
            Vector3 tree = Seperation();
            tree.y = 0;
            gameObject.transform.position += (tree * Time.deltaTime);
        }
    }

    /// <summary>
    /// boid alignment function
    /// </summary>
    public Vector3 Alignment()
    {
        if (CommandGroup.entities.Count <= 1)
        {
            return Vector3.zero;
        }

        Vector3 perceivedCenter;

        if (!CommandGroup.flock)
        {
            perceivedCenter = Vector3.zero;
        }
        else
        {
            perceivedCenter = CommandGroup.centerVector.normalized* CommandGroup.followStr;
        }

        foreach (Entity boid in CommandGroup.entities)
        {
            if (boid != this)
            {
                perceivedCenter += boid.transform.position;
            }
        }
        perceivedCenter = perceivedCenter / (CommandGroup.entities.Count - 1);
        return (perceivedCenter - transform.position) * CommandGroup.alignmentFactor;
    }

    /// <summary>
    /// Boid seperation Function
    /// </summary>
    public Vector3 Seperation()
    {
        if (CommandGroup.entities.Count <= 1)
        {
            return Vector3.zero;
        }

        Vector3 c = Vector3.zero;
        foreach (Entity boid in GameController.Main.EntityController.Entities)
        {
            if (boid != this)
            {
                if (Vector3.Distance(boid.transform.position, transform.position) < effectiveDistance)
                {
                    c = c - (boid.transform.position - transform.position);
                }
            }
        }
        return c;
    }

    /// <summary>
    /// Boid cohesion function
    /// </summary>
    public Vector3 Cohesion()
    {
        if (CommandGroup.entities.Count <= 1)
        {
            return Vector3.zero;
        }

        //percieved velocity
        Vector3 pv = Vector3.zero;
        foreach (Entity boid in CommandGroup.entities)
        {
            if (boid != this)
            {
                pv += boid.velocity;
            }
        }
        pv /= (CommandGroup.entities.Count - 1);
        return (pv - velocity) * CommandGroup.cohesionFactor;
    }

    /// <summary>
    /// Limits the velocity to the max possible velocity
    /// </summary>
    public void LimitVelocity()
    {
        if (velocity.magnitude > CommandGroup.speedLimit)
        {
            velocity = velocity.normalized * CommandGroup.speedLimit;
        }
    }

    /// <summary>
    /// Keeps units within bound (Only works when flocking)
    /// </summary>
    public Vector3 BoundPosition()
    {
        if (CommandGroup.flock)
        {
            return Vector3.zero;
        }

        Vector3 adjustedPath = Vector3.zero;
        if (transform.position.x < CommandGroup.min.x)
        {
            adjustedPath.x = (CommandGroup.min.x - transform.position.x);
        }
        else if (gameObject.transform.position.x > CommandGroup.max.x)
        {
            adjustedPath.x = (CommandGroup.max.x - transform.position.x);
        }

        if (gameObject.transform.position.y < CommandGroup.min.y)
        {
            adjustedPath.y = (CommandGroup.min.y - transform.position.y);
        }
        else if (gameObject.transform.position.y > CommandGroup.max.y)
        {
            adjustedPath.y = (CommandGroup.max.y - transform.position.y);
        }

        if (gameObject.transform.position.z < CommandGroup.min.z)
        {
            adjustedPath.z = (CommandGroup.min.z - transform.position.z);
        }
        else if (gameObject.transform.position.z > CommandGroup.max.z)
        {
            adjustedPath.z = (CommandGroup.max.z - transform.position.z);
        }
        return adjustedPath * CommandGroup.boundFactor;
    }

    public void Perching()
    {
        if (targetObject != null && Vector3.Distance(transform.position, targetObject.transform.position) <= CommandGroup.distanceFromTarget)
        {
            velocity = Vector3.zero;
            perch = true;
            idle = true;
        }
        else
        {
            perch = false;
        }
    }
    public void SetTargetPos()
    {
        targetPos = targetObject.transform.position;
    }
}
