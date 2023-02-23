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
    public float followStrength;

    public bool viewDebugInfo = false;


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



    //Cache
    Unit unit;

    void Start()
    {
        unit = GetComponent<Unit>();
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
        CalculateMovementDirection();
        transform.position += (movementDirection.normalized * movementSpeed * Time.deltaTime);

        // Set's height to block height it valid move
        float height = GameController.Main.WorldController.World.GetHeight(transform.position.x, transform.position.z);
        if (Mathf.Abs(height - transform.position.y) <= GameController.Main.CommandController.stepHeight)
        {
            transform.position = new Vector3(transform.position.x, height, transform.position.z);
           
        }
    }

    #region MovementFunctions
    /// <summary>
    /// Calculates the movement direction of the entity
    /// </summary>
    void CalculateMovementDirection()
    {
        movementDirection = EntityAvoidance() + WallAvoidance() + FollowTarget();
        movementDirection = movementDirection.Flat().normalized;
    }

    public Vector3 FollowTarget()
    {
        return (targetPos.Flat() - transform.position.Flat()).normalized * followStrength;
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
        //if (CommandGroup != null && movementDirection.magnitude > CommandGroup.speedLimit)
        //{
        //    movementDirection = movementDirection.normalized * CommandGroup.speedLimit;
        //}
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

    public void SetTargetPos(Vector3 pos)
    {
        targetPos = pos;
    }
}
