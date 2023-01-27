using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Unit : Entity
{
    public LineRenderer lineRenderer;
    public bool drawPath;
    bool isMoving = false;

    public float moveSpeed = 5;

    Vector3[] path = null;
    Vector3 target;
    Vector3 formationPoint; //<- store this in battalion?
    int index;

    EntityAnimator animator;

    Coroutine followPath;

    Task<Vector2Int[]> PathRequest;

    protected void Start()
    {
        animator = GetComponent<EntityAnimator>();
        base.Start();
        StartCoroutine(FollowPath());
    }


    public void Move(Vector3 mapPosition)
    {

        Vector3Int start = MapController.main.grid.WorldToCell(transform.position);
        Vector3Int end = MapController.main.grid.WorldToCell(mapPosition);

        Debug.Log(end);

        var path = PathManager.main.RequestPath(new Vector2Int(start.x, start.z), new Vector2Int(end.x, end.y));
        if (path == null)
        {
            Debug.Log("Path could not be found");
            return;
        }

        index = -1;
        Vector3[] positions = new Vector3[path.Length];
        for (int i = 0; i < path.Length; i++)
        {
            //Vector3Int p = new Vector3Int(path[i].x, 0, path[i].y);
            positions[i] = MapController.main.grid.GetCellCenterWorld(new Vector3Int(path[i].x, 0, path[i].y));
        }
        this.path = positions;
        index = 0;


        if (lineRenderer != null && drawPath)
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = path.Length;

            lineRenderer.SetPositions(positions);
        }
        else
        {
            lineRenderer.enabled = false;
        }

    }

    public void MoveAsync(Vector3 mapPosition)
    {
        if (!GetClosestValidPosition(mapPosition, out mapPosition))
        {
            return;
        }

        target = mapPosition;

        //Debug.Log("async called");
        Vector3Int start = MapController.main.grid.WorldToCell(transform.position);
        Vector3Int end = MapController.main.grid.WorldToCell(mapPosition);
        

        PathRequest = null;
        index = -2;
        PathRequest = PathManager.main.RequestPathAsync((Vector2Int)start, (Vector2Int)end);
    }

    void RotateTowardsMovement(Vector3 move)
    {
        Vector3 test = move;
        test.y = 0;
        if (test != Vector3.zero)
        {
            RotateTowards(transform.position + move);
        }
    }

    private void Update()
    {
        if(animator != null)
        {
            if (isMoving)
            {
                animator.SetState(EntityAnimator.AnimatorState.RUN_FORWARD);
            }
            else
            {
                animator.SetState(EntityAnimator.AnimatorState.IDLE);
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            drawPath = !drawPath;
            if (drawPath)
            {
                lineRenderer.enabled = true;
            }
            else
            {
                lineRenderer.enabled = false;
            }
        }
    }

    protected IEnumerator FollowPath()
    {
        while (true)
        {
            if (enabled)
            {
               

                if (index < 0)
                {
                    isMoving = false;
                    if (PathRequest != null && PathRequest.IsCompleted && PathRequest.Result != null)
                    {
                        var intPath = PathRequest.Result;
                        Vector3[] positions = new Vector3[intPath.Length];
                        for (int i = 0; i < intPath.Length; i++)
                        {
                            //Vector3Int p = new Vector3Int(path[i].x, 0, path[i].y);
                            positions[i] = MapController.main.grid.GetCellCenterWorld((Vector3Int)intPath[i]);
                        }
                        positions[positions.Length - 1] = target;
                        path = positions;
                        index = 0;


                        if (lineRenderer != null)
                        {
                            lineRenderer.positionCount = path.Length;

                            lineRenderer.SetPositions(positions);
                        }
                    }
                }
                else if (path != null && index < path.Length)
                {
                    if (animator != null)
                    {
                       // animator.SetState(EntityAnimator.AnimatorState.RUN_FORWARD);
                    }
                    Vector3 target = path[index];
                    target.y = transform.position.y;
                    while (Vector3.Distance(transform.position, target) > MapController.main.grid.cellSize.x / 2)
                    {
                        isMoving = true;
                        if (index < 0)
                        {
                            break;
                        }

                        Vector3 dir = target - transform.position;
                        dir.y = 0;
                        dir = dir.normalized;

                        Vector3 neo = transform.position;
                        neo += dir * moveSpeed * Time.deltaTime;
                        neo.y = MapController.main.RTS.SampleHeight(neo) + 0.1f;
                        transform.position = neo;
                        if(GetComponent<Hero>() == null || (GetComponent<Hero>() != null && !GetComponent<Hero>().controlled))
                        {
                            RotateTowardsMovement(dir);
                        }                      
                        target.y = transform.position.y;

                        Debug.DrawRay(transform.position, path.Last() - transform.position, Color.black);

                        yield return null;
                    }
                    if (index >= 0)
                    {
                        index++;

                        var currPos = transform.position;
                        var t = path.Last();
                        var direction = t - currPos;
                        if (Physics.Raycast(currPos, direction, Vector3.Distance(currPos, t), LayerMask.GetMask("Terrain", "Structure")))
                        {
                            //Debug.Log("Hit Terrain");
                        }
                        else
                        {
                            index = path.Length - 1;
                        }
                        // idle
                        // this fixed a bug when they would stutter
                        if (index >= path.Length - 1)
                        {
                            isMoving = false;
                            // animator.SetState(EntityAnimator.AnimatorState.IDLE);
                        }
                    }
                }            
            }
            yield return null;
        }
    }

    bool GetClosestValidPosition(Vector3 pos, out Vector3 newPos)
    {
        if (PathManager.main.IsValidMovePosition(Helpers.ToVector2Int(new Vector2(pos.x, pos.z))))
        {
            newPos = pos;
            return true;
        }

        Vector3[] directions =
        {
            Vector3.forward,
            -Vector3.forward,
            Vector3.right,
            -Vector3.right
        };


        for (int i = 0; i < 20; i++)
        {
            foreach (var dir in directions)
            {
                Vector3 test = pos + dir * i;
                if (PathManager.main.IsValidMovePosition(Helpers.ToVector2Int(new Vector2(test.x, test.z))))
                {
                    newPos = test;
                    return true;
                }
            }
        }

        Debug.Log("Could not find valid position");

        newPos = Vector3.zero;
        return false;
    }
}
