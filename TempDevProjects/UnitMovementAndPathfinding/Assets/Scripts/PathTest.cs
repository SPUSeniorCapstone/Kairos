using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;

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

    public Dictionary<Vector3, GameObject> nodes = new Dictionary<Vector3, GameObject>();

    public Material Open, Closed, Path, Blocked, None;

    public GameObject startPos, endPos;
    public Vector3 start, end;
    public int ProgressAmount = 1;
    public float step = 1;

    public bool showSteps = false;

    LineRenderer lineRenderer;

    public GameObject nodeObject;


    public float speed = 1;
    bool moveTowardsTarget = false;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        Debug.Log("Testing MinHeap Functionality");

        finder.step = step;
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

        if (Input.GetKeyDown(KeyCode.U))
        {
            showSteps = !showSteps;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            finder.RunTest();
        }

        if (showSteps)
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
            GameObject o;

            if (nodes.ContainsKey(node.position))
            {
                o = nodes[node.position];
            }
            else
            {
                o = Instantiate(nodeObject, node.position, Quaternion.identity, transform);
                nodes.Add(node.position, o);
            }

            if (node.state == PathNode.NodeState.OPEN)
            {
                o.GetComponent<MeshRenderer>().material = Open;
            }

            if(node.state == PathNode.NodeState.CLOSED)
            {
                o.GetComponent<MeshRenderer>().material = Closed;
            }

            if (node.state == PathNode.NodeState.PATH)
            {
                o.GetComponent<MeshRenderer>().material = Path;
            }

            if (node.state == PathNode.NodeState.RESTRICTED)
            {
                o.GetComponent<MeshRenderer>().material = Blocked;
            }
        }
    }

}
