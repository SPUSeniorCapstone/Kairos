using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class Entity : MonoBehaviour
{

    [Range(0, 50f)]
    public float effectiveDistance;

    public float moveSpeed = 10;
    public float avoidSpeed = 10;

    public bool viewDebugInfo = false;
    [ConditionalHide(nameof(viewDebugInfo), true)][Disable]
    public CommandGroup CommandGroup;

    [ConditionalHide(nameof(viewDebugInfo), true)][Disable]
    public GameObject targetObject;

    [ConditionalHide(nameof(viewDebugInfo), true)][Disable]
    public Vector3 velocity = Vector3.zero;

    [ConditionalHide(nameof(viewDebugInfo), true)][Disable]
    public Vector3 targetPos;

    [ConditionalHide(nameof(viewDebugInfo), true)][Disable]
    public float distance;

    [ConditionalHide(nameof(viewDebugInfo), true)][Disable]
    public bool perch = false;

    
    [ConditionalHide(nameof(viewDebugInfo), true)][Disable]
    public bool idle = true;




    void Start()
    {
        GameController.Main.EntityController.AddEntity(this);
    }

    private void OnDestroy()
    {
        if(GameController.Main != null && GameController.Main.EntityController != null)
            GameController.Main.EntityController.RemoveEntity(this);
    }

    void Update()
    {
        CalculateVelocity();
        if(CommandGroup != null)
        {
            transform.position += (velocity.normalized * CommandGroup.followSpeed * Time.deltaTime);
        }
        else
        {
            transform.position += (velocity * Time.deltaTime * avoidSpeed);
        }
    }

    void CalculateVelocity()
    {
        if (!perch && !idle && CommandGroup != null)
        {
            distance = Vector3.Distance(CommandGroup.centerVector, targetObject.transform.position);

            velocity = Alignment() + Seperation() + Cohesion();
            velocity += (targetPos - transform.position).normalized * CommandGroup.followStr;
            if (CommandGroup.flock)
            {
                velocity += BoundPosition();
            }

            LimitVelocity();
            if (CommandGroup.twoD)
            {
                float X = transform.position.x;
                float Z = transform.position.z;
                transform.position = new Vector3(X, 0, Z);
            }

        }
        else
        {
            velocity = Seperation();
        }

        velocity.y = 0;
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
        if (!CommandGroup.flock)
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

    public bool Perching()
    {
        if (targetObject != null && Vector3.Distance(transform.position, targetPos) <= CommandGroup.distanceFromTarget)
        {
            velocity = Vector3.zero;
            perch = true;
            idle = true;
            return true;
        }
        else
        {
            perch = false;
            return false;
        }
    }
    public void SetTargetPos()
    {
        targetPos = targetObject.transform.position;
    }
}
