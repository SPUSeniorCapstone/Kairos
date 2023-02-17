using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;
using Debug = UnityEngine.Debug;

public class Entity : MonoBehaviour
{
    public float debugY;
    public int pathindex = 0;
    public bool pathing;

    [Range(0, 50f)]
    public float effectiveDistance;

    public float personalDistance;
    public float attackDistance;

    public float moveSpeed = 10;
    public float avoidSpeed = 10;
    public float targetHealth;
    public float lastAttackTime;
    public float AttackCoolDown;


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

    [ConditionalHide(nameof(viewDebugInfo), true)]
    public bool perch = false;

    
    [ConditionalHide(nameof(viewDebugInfo), true)]
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
        if (targetObject == null)
        {
            idle = true;
            GetComponent<Damageable>().isAttacking = false;
        }
        if (targetObject != null)
        {
            var dam = targetObject.GetComponent<Damageable>();
            if (dam != null && dam.Dead)
            {
                Debug.Log("not null but dead");
                targetObject = null;
                GetComponent<Damageable>().isAttacking = false;
                idle = true;
                perch = false;
            }
            else if (targetObject == null && GetComponent<Damageable>().isAttacking == true)
            {
                Debug.Log("hefnafnje");
                GetComponent<Damageable>().isAttacking = false;
            }
        }
        CalculateVelocity();
        if(CommandGroup != null)
        {
            transform.position += (velocity.normalized * CommandGroup.followSpeed * Time.deltaTime);
        }
        else if (GetComponent<Damageable>().isAttacking && !perch)
        {
            transform.position += (velocity.normalized * moveSpeed * Time.deltaTime);
        }
        else if (idle)
        {
            transform.position += (velocity * Time.deltaTime * avoidSpeed);
        }
        if (targetPos != null)
        {
            personalDistance = Vector3.Distance(transform.position, targetPos);
           
        }
    }

    void CalculateVelocity()
    {
        if (targetObject != null)
        {
            targetPos = targetObject.transform.position;
            
            //else
            //{
            //    idle = false;
            //    GetComponent<Damageable>().isAttacking = false;
            //}
        }
        if (!perch && !idle && CommandGroup != null)
        {
            //distance = Vector3.Distance(CommandGroup.centerVector, targetObject.transform.position);
            // distance = Vector3.Distance(CommandGroup.centerVector, targetPos);

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
                transform.position = new Vector3(X, debugY, Z);
            }

        }
        else if (GetComponent<Damageable>().isAttacking)
        {
            if (!idle && !perch)
            {
                velocity = Alignment() + Seperation() + Cohesion();
                velocity += (targetPos - transform.position).normalized;


                LimitVelocity();

                float X = transform.position.x;
                float Z = transform.position.z;
                transform.position = new Vector3(X, debugY, Z);
            }
           
            // bootleg perch
            if (personalDistance <= attackDistance)
            {
                perch = true;
                if (targetObject != null)
                {
                    var dam = targetObject.GetComponent<Damageable>();
                    if (dam != null && (Time.time - lastAttackTime > AttackCoolDown))
                    {
                        targetHealth = targetObject.GetComponent<Damageable>().Damage(10f);
                        lastAttackTime = Time.time;
                        if (targetHealth <= 0)
                        {
                            targetObject = null;
                        }
                    }
                }

            }
            else
            {
                Debug.Log("FALSE PERCH");
                perch = false;
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
        if (CommandGroup == null || CommandGroup.entities.Count <= 1)
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
        if (CommandGroup == null || CommandGroup.entities.Count <= 1)
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
        if (CommandGroup != null && velocity.magnitude > CommandGroup.speedLimit)
        {
            velocity = velocity.normalized * CommandGroup.speedLimit;
        }
    }

    /// <summary>
    /// Keeps units within bound (Only works when flocking)
    /// </summary>
    public Vector3 BoundPosition()
    {
        if (CommandGroup == null || !CommandGroup.flock)
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
        //Debug.Log(personalDistance + " <= " + CommandGroup.distanceFromTarget);
        //Debug.Log(Vector3.Distance(transform.position, targetPos) <= CommandGroup.distanceFromTarget);
        if (Vector3.Distance(transform.position, targetPos) <= CommandGroup.distanceFromTarget)
        {
            if (pathing && pathindex < CommandGroup.path.Count - 1) {
                Debug.Log("YEAHG!");
                pathindex++;
                NextPoint();
                return false;
            }
            velocity = Vector3.zero;
            perch = true;
            idle = true;
            return true;
        }
        else
        {
            Debug.Log("NOPE");
            perch = false;
            return false;
        }
    }
    public void SetTargetPos()
    {
        targetPos = targetObject.transform.position;
    }
    public void NextPoint()
    {
        if (CommandGroup.path.Count != 0 && CommandGroup.path != null)
        {
            //UnityEngine.Debug.Log("Next Point count:" + CommandGroup.path.Count);
            //UnityEngine.Debug.Log("Next Point List:" + CommandGroup.path);
            targetPos = CommandGroup.path[pathindex];
        }
        else
        {
            Debug.Log("path null or 0 count");
        }
    }
}
