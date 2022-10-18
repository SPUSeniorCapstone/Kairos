using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathTest : MonoBehaviour
{

    PathFinder finder = new PathFinder((pos) =>
    {
        RaycastHit hit;
        if (Physics.CheckSphere(pos, 1))
        {
            return false;
        }
        return true;
    });

    public GameObject startPos, endPos;
    public Vector3 start, end;
    public int ProgressAmount = 1;
    public float step = 1;

    LineRenderer lineRenderer;

    public GameObject nodeObject;


    public float speed = 1;
    bool moveTowardsTarget = false;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    bool searching = false;

    int index = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!searching)
            {
                start = startPos.transform.position;
                end = endPos.transform.position;
                FindPath();
            }
            moveTowardsTarget = true;
        }

        if (moveTowardsTarget)
        {
            var path = finder.FindPath(start, end);
            if (path != null && index < path.Length)
            {
                if (Vector3.Distance(startPos.transform.position, path[index]) < speed * Time.deltaTime)
                {
                    index++;
                }
                var dir = path[index] - startPos.transform.position;

                startPos.transform.position += dir.normalized * speed * Time.deltaTime;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (startPos.transform.position != finder.Start || endPos.transform.position != finder.End)
            {
                finder.Start = startPos.transform.position;
                finder.End = endPos.transform.position;
            }

            if (finder.state == PathFinder.State.PATH_FOUND)
            {
                var path = finder.FindPath(startPos.transform.position, endPos.transform.position);
                if (path != null)
                {
                    lineRenderer.positionCount = path.Length;
                    lineRenderer.SetPositions(path);
                }
            }

            finder.Progress(ProgressAmount);
            AddUpdatedNodes();
        }
    }

    void FindPath()
    {
        searching = true;
        finder.step = step;

        finder.ResetFinder();
        var path = finder.FindPath(start, end);
        if (path != null)
        {
            lineRenderer.positionCount = path.Length;
            lineRenderer.SetPositions(path);
        }
        searching = false;
    }

    void AddUpdatedNodes()
    {
        foreach (var node in finder.NewNodes)
        {
            Instantiate(nodeObject, node.position, Quaternion.identity, transform);
        }
    }

}
