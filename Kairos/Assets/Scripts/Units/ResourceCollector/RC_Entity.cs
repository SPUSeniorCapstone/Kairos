using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;


public class RC_Entity : Entity
{
    public enum MovementMode { IDLE, FOLLOW_PATH, FOLLOW_TARGET, COLLECT_RESOURCE, RETURN_HOME}

    public MovementMode movementMode = MovementMode.IDLE;

    public bool retrievingPath;
    public Task<List<Vector3>> pathingTask;

    public RC_Unit rcUnit;

    List<Vector3> path;
    int pathIndex = -1;

    public Vector3 HomeVector;
    public Vector3 TargetVector;
    public float collectCoolDown = 1;
    private float lastcollectTime = 0;
    public int count;
    public int maxHold = 4;
    public int collectDistance;

    public bool lockHorizontalRotation = true;

    public float rotateSpeed = 10f;


    new private void Start()
    {
        base.Start();
        rcUnit = GetComponent<RC_Unit>();
    }

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
                if (tempPath != null)
                {
                    path = tempPath;
                }
                else
                {
                    Debug.Log("Path not found");
                }
                movementMode = MovementMode.FOLLOW_PATH;
                targetPos = path[0];
                pathIndex = 1;
                retrievingPath = false;
                pathingTask = null;
            }
        }
        if (movementMode == MovementMode.IDLE && HomeVector != Vector3.zero && TargetVector != Vector3.zero && Vector3.Distance(transform.position.Flat(), TargetVector.Flat()) < collectDistance)
        {
            if (Time.time - lastcollectTime > collectCoolDown)
            {
                //Body.clip = BodySounds.ElementAt(10);
                //Body.Play();
                count++;
                lastcollectTime = Time.time;

                // neccessary? doesn't work all the time (race condition)
                if (count == maxHold)
                {
                    rcUnit.MoveTo(HomeVector);
                }
            }
        }
        else if (movementMode == MovementMode.IDLE && HomeVector != Vector3.zero && TargetVector != Vector3.zero && Vector3.Distance(transform.position.Flat(), HomeVector.Flat()) < collectDistance)
        {
            if (Time.time - lastcollectTime > collectCoolDown)
            {
                //Body.clip = BodySounds.ElementAt(10);
                //Body.Play();
                count--;
                GameController.Main.UpdateResource(-50);
                //GameController.Main.resouceCount += 40;
                //dont call this here
                //GameController.Main.UIController.gameUI.UpdateResource(GameController.Main.resouceCount);
                lastcollectTime = Time.time;

                // neccessary? doesn't work all the time (race condition)
                if (count == 0)
                {
                    rcUnit.MoveTo(TargetVector);
                }
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
            case MovementMode.COLLECT_RESOURCE:
                CollectResource();
                break;
            case MovementMode.RETURN_HOME:
                ReturnHome();
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
        //if (GetComponent<Infantry_Unit>().target != null && Vector3.Distance(transform.position.Flat(), GetComponent<Infantry_Unit>().target.transform.position.Flat()) <= GetComponent<Infantry_Unit>().attackDistance)
        //{
        //    movementMode = MovementMode.ATTACK_FOLLOW;
        //    Debug.Log("IN RANGE!!");
        //    pathIndex = -1;
        //    path = null;
        //}
        FollowTarget();
    }

    void FollowTarget()
    {
        Idle();
        movementDirection += TargetAttraction();
    }

    void CollectResource()
    {
        FollowPath();
    }

    void ReturnHome()
    {

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
