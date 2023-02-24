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
    #region Fields

    /// <summary>
    /// Distance at which the entity looks for collisions with other entities
    /// </summary>
    [Range(0, 10)]
    public float AvoidEntityRadius = 3;

    /// <summary>
    /// Distance at which the entity looks for collisions with walls and structures
    /// </summary>
    [Range(0,10)]
    public int AvoidWallRadius = 3;

    public int stepHeight = 1;

    /// <summary>
    /// How fast the entity moves 
    /// </summary>
    [Tooltip("Speed of movement after calculating direction")]
    public float movementSpeed = 10;

    [Tooltip("The distance at which the entity will stop following it's target")]
    public float stopFollowDistance = 1;

    [Range (0, 10)]
    public float avoidStrength = 1;
    [Range(0,10)]
    public float followStrength = 1;

    public bool viewDebugInfo = false;
    [ConditionalHide(nameof(viewDebugInfo), true)][Disable]
    public Vector3 movementDirection = Vector3.zero;

    [ConditionalHide(nameof(viewDebugInfo), true)][Disable]
    public Vector3 targetPos;


    [ConditionalHide(nameof(viewDebugInfo), true)]
    public bool perch = false;

    [ConditionalHide(nameof(viewDebugInfo), true)]
    public bool idle = true;

    #endregion

    #region Object Cache
    //Cache
    Unit unit;

    #endregion

    #region Unity Messages

    protected void Start()
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

    protected void Update()
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

    #endregion

    #region MovementFunctions
    /// <summary>
    /// Calculates the movement direction of the entity
    /// </summary>
    virtual protected void CalculateMovementDirection()
    {
        movementDirection = EntityAvoidance() + WallAvoidance() + TargetAttraction();
        movementDirection = movementDirection.Flat().normalized;
    }

    public Vector3 TargetAttraction()
    {
        if(Vector3.Distance(transform.position, targetPos) < stopFollowDistance)
        {
            return Vector3.zero;
        }
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
    }

    #endregion

    public void SetTargetPos(Vector3 pos)
    {
        targetPos = pos;
    }
}
