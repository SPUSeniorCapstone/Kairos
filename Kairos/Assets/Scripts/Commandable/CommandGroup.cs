using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class CommandGroup : MonoBehaviour
{
    //public List<Entity> entities;
    public bool example = false;
    public List<Unit> unitList;
    public List<Vector3> path;
    public Task<List<Vector3>> pathTask;
    public Vector3 nextPoint;

    public CommandGroup settings;

    public int speedLimit;
    public Vector3 max, min;
    public bool attackCommand = false;
    public Vector3 centerVector => transform.position;
    public float followStr;
    public float followSpeed;
    public GameObject groupTargetObj;
    public bool destroyOnEmpty = true;
    public bool needsPath = true;
    public float minimumCenterDistance = 7;
    public bool retrievingPath = false;

    /// <summary>
    /// Distance at which the entity looks for collisions with other entities
    /// </summary>
    [Range(0, 10)]
    public float AvoidEntityRadius = 3;

    /// <summary>
    /// Distance at which the entity looks for collisions with walls and structures
    /// </summary>
    [Range(0, 10)]
    public int AvoidWallRadius = 3;

    public int stepHeight = 1;

    /// <summary>
    /// How fast the entity moves 
    /// </summary>
    [Tooltip("Speed of movement after calculating direction")]
    public float movementSpeed = 10;

    [Tooltip("The distance at which the entity will stop following it's target")]
    public float stopFollowDistance = 1;

    [Range(0, 10)]
    public float avoidStrength = 1;
    [Range(0, 10)]
    public float followStrength = 1;

    void Update()
    {
        CalculateCenter();
        if (destroyOnEmpty)
        {
            CheckIfEmpty();
        }
        UpdateSettings();
        if (path.Count > 0 && Vector3.Distance(centerVector, path.ElementAt(0)) < minimumCenterDistance)
        {
            if (path.Count > 1)
            {
                SetGroupTarget(path.ElementAt(1));
                path.RemoveAt(0);
            }
            else
            {
                SetGroupTarget(path.ElementAt(0));
                path.RemoveAt(0);
            }
   
        }
        if (retrievingPath)
        {
            if (pathTask == null)
            {
                retrievingPath = false;
            }
            else if (pathTask.IsCompleted)
            {
                var tempPath = pathTask.Result;
                if (tempPath != null && tempPath.Count > 0)
                {
                    path = tempPath;

                    //movementMode = MovementMode.FOLLOW_PATH;
                    //targetPos = path[0];
                    //pathIndex = 1;
                    pathTask = null;
                    foreach (Unit unit in unitList)
                    {
                        unit.ClearTarget();
                        unit.MoveToTarget(path.ElementAt(0));
                    }
                }
                else
                {
                    Debug.Log("Path not found");
                }
                retrievingPath = false;
            }
        }
        //if (pathTask != null && pathTask.IsCompleted && needsPath)
        //{
        //    path = pathTask.Result;
        //    if (path == null)
        //    {
        //        Debug.Log("null path");
        //    }
        //    else
        //    {
        //        Debug.Log("path = " + pathTask.Result);
        //        needsPath = false;

        //        foreach (Entity entity in entities)
        //        {
        //            //entity.NextPoint();
        //            entity.idle = false;
        //        }
        //    }

        //    // if null no path
        //}
        //// is there a better way?
        //if (destroyOnEmpty)
        //{
        //    CheckIfEmpty();
        //}
        //else 
        //{
        //    foreach (CommandGroup group in GameController.Main.CommandController.CommandGroups)
        //    {
        //        if (group != this)
        //        {
        //            group.effectiveDistance = effectiveDistance;
        //            group.distanceFromTarget = distanceFromTarget;
        //            group.cohesionFactor = cohesionFactor;
        //            group.alignmentFactor = alignmentFactor;
        //            group.followSpeed = followSpeed;
        //            group.followStr = followStr;
        //        }
        //    }
        //}
    }

    private void Start()
    {
        if (!example)
        {
            settings = GameController.Main.CGSettings;
        }

        UpdateSettings();
    }

    public void UpdateSettings()
    {
        if (settings != null)
        {
            AvoidEntityRadius = settings.AvoidEntityRadius;


            AvoidWallRadius = settings.AvoidWallRadius;
            stepHeight = settings.stepHeight;

            movementSpeed = settings.movementSpeed;

            stopFollowDistance = settings.stopFollowDistance;

            avoidStrength = settings.avoidStrength;
            followStrength = settings.followStrength;
        }
    }

    public void CalculateCenter()
    {
        // why is this neccessary?
        if (unitList.Count == 0)
        {
            transform.position = Vector3.zero;
            return;
        }

        Vector3 center = Vector3.zero;
        int count = 0;
        for (int i = 0; i < unitList.Count; i++)
        {
            var unit = unitList[i];

            center += unit.transform.position.Flat();
            count++;
        }
        if (count > 0)
        {
            center /= count;
            center.y = 0;
        }
        transform.position = center;
    }
    public void SetGroupTarget(Vector3 pos)
    {
        foreach (Unit unit in unitList)
        {
            unit.MoveToTarget(pos);
        }
        //foreach (Unit units in unitList)
        //{
        //entity.targetObject = groupTargetObj;
        //if (gameObject == GameController.Main.CommandController.wayPoint)
        //{
        //    Debug.Log("GAMEOBJECT WAYPOINT");
        //    entity.pathing = true;
        //    entity.NextPoint();
        //    //entity.SetTargetPos();
        //}
        //else
        //{
        //    Debug.Log("NOT WAYPOINT");
        //    entity.pathing = true;
        //    entity.NextPoint();
        //    //entity.SetTargetPos();
        //    //entity.idle = false;
        //}
        //entity.SetTargetPos();
        //entity.idle = false;

        //}
    }
    public void CheckIfEmpty()
    {
        if (unitList.Count == 0)
        {
            GameController.Main.CommandController.CommandGroups.Remove(this);
            Destroy(gameObject);
        }
    }


}
