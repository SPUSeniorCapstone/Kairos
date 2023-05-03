using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class Infantry_Entity : Entity
{
    public enum MovementMode { IDLE, FOLLOW_PATH, FOLLOW_TARGET, ATTACK_FOLLOW }

    public MovementMode movementMode = MovementMode.IDLE;

    public bool retrievingPath;
    public Task<List<Vector3>> pathingTask;

    List<Vector3> path;
    int pathIndex = -1;

    public bool lockHorizontalRotation = true;

    public float rotateSpeed = 10f;


    new void Update()
    {
        //base.Update();

        //Debug test range
        //if (GetComponent<Infantry_Unit>().target != null)
        //{
        //    Debug.Log(Vector3.Distance(transform.position.Flat(), GetComponent<Infantry_Unit>().target.transform.position.Flat()) + " <= " + GetComponent<Infantry_Unit>().attackDistance);
        //} 

        if (retrievingPath)
        {
            if (pathingTask == null)
            {
                retrievingPath = false;
            }
            else if (pathingTask.IsCompleted)
            {
                var tempPath = pathingTask.Result;
                if (tempPath != null && tempPath.Count > 0)
                {
                    path = tempPath;

                    movementMode = MovementMode.FOLLOW_PATH;
                    targetPos = path[0];
                    pathIndex = 1;
                    pathingTask = null;
                }
                else
                {
                    Debug.Log("Path not found");
                }
                retrievingPath = false;
            }
        }

        Move();
    }

    void Move()
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

    protected override void CalculateMovementDirection()
    {
        movementDirection = Vector3.zero;
        switch (movementMode)
        {
            case MovementMode.IDLE:
                Idle();
                break;
            case MovementMode.FOLLOW_PATH:
                FollowPath();
                break;
            case MovementMode.FOLLOW_TARGET:
                FollowTarget();
                break;
            case MovementMode.ATTACK_FOLLOW:
                AttackFollow();
                break;
        }
    }

    void Idle()
    {
        movementDirection += EntityAvoidance() + WallAvoidance();// + BoundAvoidance();
    }

    void FollowPath()
    {
        if (path != null && Vector3.Distance(transform.position.Flat(), targetPos.Flat()) < stopFollowDistance)
        {
            if (pathIndex < path.Count - 1)
            {
                pathIndex++;
                targetPos = path[pathIndex];
            }
            else
            {
                pathIndex = -1;
                path = null;
                movementMode = MovementMode.IDLE;
            }
        }
        if (GetComponent<Infantry_Unit>().target != null && Vector3.Distance(transform.position.Flat(), GetComponent<Infantry_Unit>().target.transform.position.Flat()) <= GetComponent<Infantry_Unit>().attackDistance)
        {
            movementMode = MovementMode.ATTACK_FOLLOW;
            Debug.Log("IN RANGE!!");
            pathIndex = -1;
            path = null;
        }
        FollowTarget();
    }

    void FollowTarget()
    {
        Idle();
        movementDirection += TargetAttraction();
    }

    void AttackFollow()
    {
        Idle();
        if (GetComponent<Infantry_Unit>().target != null && Vector3.Distance(transform.position.Flat(), GetComponent<Infantry_Unit>().target.transform.position.Flat()) <= GetComponent<Infantry_Unit>().attackDistance)
        {
            movementMode = MovementMode.ATTACK_FOLLOW;
            //Debug.Log("IN RANGE!!");
        }
        else
        {
            movementDirection += TargetAttraction();
        }
    }


    public void RotateTowards(Vector3 pos)
    {
        pos += transform.position;


        if (lockHorizontalRotation)
        {
            pos.y = transform.position.y;
        }

        Quaternion rotation = transform.rotation;

        Vector3 direction = pos - transform.position;
        var lookRotation = Quaternion.LookRotation(direction);


        rotation = Quaternion.Slerp(rotation, lookRotation, Time.deltaTime * rotateSpeed);
        transform.rotation = rotation;
    }

}
