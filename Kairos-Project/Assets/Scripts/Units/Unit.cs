using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Unit : Entity
{
    public LineRenderer lineRenderer;
    public bool drawPath;

    public float moveSpeed = 5;

    Vector3[] path = null;
    int index;

    Coroutine followPath;

    Task<Vector2Int[]> PathRequest;

    protected void Start()
    {
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
            lineRenderer.positionCount = path.Length;

            lineRenderer.SetPositions(positions);
        }

    }

    public void MoveAsync(Vector3 mapPosition)
    {
        Vector3Int start = MapController.main.grid.WorldToCell(transform.position);
        Vector3Int end = MapController.main.grid.WorldToCell(mapPosition);

        PathRequest = null;
        index = -2;
        PathRequest = PathManager.main.RequestPathAsync((Vector2Int)start, (Vector2Int)end);
    }

    protected IEnumerator FollowPath()
    {
        while (true)
        {
            if (index < 0)
            {
                if (PathRequest != null && PathRequest.IsCompleted)
                {
                    var intPath = PathRequest.Result;
                    Vector3[] positions = new Vector3[intPath.Length];
                    for (int i = 0; i < intPath.Length; i++)
                    {
                        //Vector3Int p = new Vector3Int(path[i].x, 0, path[i].y);
                        positions[i] = MapController.main.grid.GetCellCenterWorld((Vector3Int)intPath[i]);
                    }
                    path = positions;
                    index = 0;


                    if (lineRenderer != null && drawPath)
                    {
                        lineRenderer.positionCount = path.Length;

                        lineRenderer.SetPositions(positions);
                    }
                }
            }
            else if (path != null && index < path.Length)
            {
                Vector3 target = path[index];
                target.y = transform.position.y;
                while (Vector3.Distance(transform.position, target) > MapController.main.grid.cellSize.x / 2)
                {
                    if(index < 0)
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
                    target.y = transform.position.y;

                    Debug.DrawRay(transform.position, path.Last() - transform.position, Color.black);

                    yield return null;
                }
                if (index >= 0) {
                    index++;

                    var currPos = transform.position;
                    var t = path.Last();
                    var direction = t - currPos;
                    if (Physics.Raycast(currPos, direction, Vector3.Distance(currPos, t)))
                    {
                        Debug.Log("Hit Terrain");
                    }
                    else
                    {
                        index = path.Length - 1;
                    }
                }



            }
            yield return null;
        }
    }
}
