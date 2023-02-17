using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;
using Debug = UnityEngine.Debug;

public class Entity : MonoBehaviour
{
    public bool pathing;

    /// <summary>
    /// Distance at which the entity looks for collisions with other entities
    /// </summary>
    [Range(0, 10)]
    public float AvoidEntityRadius = 3;

    /// <summary>
    /// Distance at which the entity looks for collisions with walls and structures
    /// </summary>
    [Range(0,10)]
    public float AvoidWallRadius = 3;

    /// <summary>
    /// How fast the entity moves 
    /// </summary>
    [Tooltip("Speed of movement after calculating direction")]
    public float movementSpeed = 10;

    public float avoidStrength;

    public float personalDistance;
    public float attackDistance;

    public float targetHealth;
    public float lastAttackTime;
    public float AttackCoolDown;


    public bool viewDebugInfo = false;

    [ConditionalHide(nameof(viewDebugInfo), true)][Disable]
    public int pathindex = 0;

    [ConditionalHide(nameof(viewDebugInfo), true)][Disable]
    public CommandGroup CommandGroup;

    [ConditionalHide(nameof(viewDebugInfo), true)][Disable]
    public GameObject targetObject;

    [ConditionalHide(nameof(viewDebugInfo), true)][Disable]
    public Vector3 movementDirection = Vector3.zero;

    [ConditionalHide(nameof(viewDebugInfo), true)][Disable]
    public Vector3 targetPos;

    [ConditionalHide(nameof(viewDebugInfo), true)][Disable]
    public float distance;

    [ConditionalHide(nameof(viewDebugInfo), true)]
    public bool perch = false;

    
    [ConditionalHide(nameof(viewDebugInfo), true)]
    public bool idle = true;

    [ConditionalHide(nameof(viewDebugInfo), true)]
    [Disable]
    public Vector3 center;


    void Start()
    {
        GameController.Main.EntityController.AddEntity(this);
        float height = GameController.Main.WorldController.World.GetHeight(transform.position.x, transform.position.z);
        transform.position = new Vector3(transform.position.x, height, transform.position.z);
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
        
        CalculateMovementDirection();
        if(CommandGroup != null)
        {
            center = CommandGroup.centerVector;
            transform.position += (movementDirection.normalized * CommandGroup.followSpeed * Time.deltaTime);
        }
        else if (GetComponent<Damageable>().isAttacking && !perch)
        {
            transform.position += (movementDirection.normalized * movementSpeed * Time.deltaTime);
        }
        else if (idle)
        {
            transform.position += (movementDirection.normalized * movementSpeed * Time.deltaTime);
        }

        // Set's height to block height it valid move
        float height = GameController.Main.WorldController.World.GetHeight(transform.position.x, transform.position.z);
        if (Mathf.Abs(height - transform.position.y) <= GameController.Main.CommandController.stepHeight)
        {
            transform.position = new Vector3(transform.position.x, height, transform.position.z);
           
        }

        personalDistance = Vector3.Distance(transform.position, targetPos);
    }

    #region MovementFunctions
    /// <summary>
    /// Calculates the movement direction of the entity
    /// </summary>
    void CalculateMovementDirection()
    {
        if (targetObject != null)
        {
            targetPos = targetObject.transform.position;
        }
        if (!perch && !idle && CommandGroup != null)
        {
            movementDirection = Alignment() + EntityAvoidance() + Cohesion() + WallAvoidance();
            movementDirection += (targetPos.Flat() - transform.position.Flat()).normalized * CommandGroup.followStr;
        }
        else if (GetComponent<Damageable>().isAttacking)
        {
            if (!idle && !perch)
            {
                movementDirection = Alignment() + EntityAvoidance() + Cohesion();
                movementDirection += (targetPos - transform.position).normalized;
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
            movementDirection = EntityAvoidance() + WallAvoidance();
        }

        movementDirection = movementDirection.Flat().normalized;
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


        perceivedCenter = CommandGroup.centerVector.normalized* CommandGroup.followStr;

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
                pv += boid.movementDirection;
            }
        }
        pv /= (CommandGroup.entities.Count - 1);
        return (pv - movementDirection) * CommandGroup.cohesionFactor;
    }

    /// <summary>
    /// Pushes boids away from each other
    /// </summary>
    public Vector3 EntityAvoidance()
    {
        Vector3 c = Vector3.zero;
        foreach (Entity boid in GameController.Main.EntityController.Entities)
        {
            if (boid != this)
            {
                float mag = Vector3.Distance(boid.transform.position, transform.position) / AvoidEntityRadius;
                if (mag < 1)
                {
                    c = c - (boid.transform.position - transform.position) / mag;
                }
            }
        }
        if (c.magnitude > 100)
        {
            c = c.normalized * 100;
        }
        return c * avoidStrength;
    }

    /// <summary>
    /// Pushes entity away from walls/structures
    /// </summary>
    /// <returns></returns>
    public Vector3 WallAvoidance()
    {
        Vector3 v = Vector3.zero;

        var world = GameController.Main.WorldController;

        Vector3Int position = world.WorldToBlockPosition(transform.position);
        var checkPos = position/* + (velocity.normalized * 2).ToVector3Int()*/;

        int dist = (int)AvoidWallRadius;
        for (int x = -dist; x <= dist; x++)
        {
            for(int z = -dist; z <= dist; z++)
            {
                var check = checkPos + new Vector3Int(x, 0, z);
                int h = world.World.GetHeight(check.x, check.z);
                if (Mathf.Abs(h - checkPos.y) > GameController.Main.CommandController.stepHeight)
                {
                    var p =  new Vector3(check.x, 0, check.z) - transform.position.Flat();
                    v -= p / (p.magnitude / AvoidWallRadius);
                }
            }
        }
        return v * avoidStrength;
    }

    /// <summary>
    /// Limits the velocity to the max possible velocity
    /// </summary>
    public void LimitVelocity()
    {
        if (CommandGroup != null && movementDirection.magnitude > CommandGroup.speedLimit)
        {
            movementDirection = movementDirection.normalized * CommandGroup.speedLimit;
        }
    }

    #endregion

    /// <summary>
    /// Keeps units within map bounds
    /// </summary>
    public Vector3 BoundAvoidance()
    {
        Vector3 v = Vector3.zero;
        var bounds = GameController.Main.WorldController.World.bounds;

        if (!bounds.Contains(transform.position))
        {
            v = bounds.ClosestPointOnBounds(transform.position) - transform.position;
            v.y = 0;
        }

        return v;
        ///Vector3 adjustedPath = Vector3.zero;
        ///if (transform.position.x < CommandGroup.min.x)
        ///{
        ///    adjustedPath.x = (CommandGroup.min.x - transform.position.x);
        ///}
        ///else if (gameObject.transform.position.x > CommandGroup.max.x)
        ///{
        ///    adjustedPath.x = (CommandGroup.max.x - transform.position.x);
        ///}
        ///if (gameObject.transform.position.y < CommandGroup.min.y)
        ///{
        ///    adjustedPath.y = (CommandGroup.min.y - transform.position.y);
        ///}
        ///else if (gameObject.transform.position.y > CommandGroup.max.y)
        ///{
        ///    adjustedPath.y = (CommandGroup.max.y - transform.position.y);
        ///}
        ///if (gameObject.transform.position.z < CommandGroup.min.z)
        ///{
        ///    adjustedPath.z = (CommandGroup.min.z - transform.position.z);
        ///}
        ///else if (gameObject.transform.position.z > CommandGroup.max.z)
        ///{
        ///    adjustedPath.z = (CommandGroup.max.z - transform.position.z);
        ///}
        ///return adjustedPath * CommandGroup.boundFactor;
    }

    public bool Perching()
    {
        //Debug.Log(personalDistance + " <= " + CommandGroup.distanceFromTarget);
        //Debug.Log(Vector3.Distance(transform.position, targetPos) <= CommandGroup.distanceFromTarget);
        if (Vector3.Distance(transform.position.Flat(), targetPos.Flat()) <= CommandGroup.distanceFromTarget)
        {
            if (CommandGroup.path != null && pathing && pathindex < CommandGroup.path.Count - 1) {
                Debug.Log("YEAHG!");
                pathindex++;
                NextPoint();
                return false;
            }
            movementDirection = Vector3.zero;
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
