using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

/// <summary>
/// Not final unit design...
/// </summary>
public class Unit : MonoBehaviour
{
    PathFinder finder;

    public LineRenderer line;

    public bool debugPath = false;
    public bool selected = false;
    public float speed = 1;
    public float step = 3;
    public float radius = 1;

    Vector3[] path = null;
    int index = 0;
    bool searching = false;

    public int nodesPerFrame = 100;

    Vector3 target = Vector3.zero;

    private void Start()
    {
        finder = new PathFinder(CheckPosition);

        finder.step = step;
    }

    private void Update()
    {
        if (selected)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000, 1 << 6))
                {
                    Vector3 t = hit.point;
                    t.y = 0;

                    if (GetClosestValidPosition(t, out target))
                    {
                        StartCoroutine(FindPath());
                    }
                }
            }
        }

        if (path != null && index < path.Length)
        {
            if (Vector3.Distance(transform.position, path[index]) < radius)
            {
                index++;
                if (index >= path.Length)
                {
                    index = 0;
                    path = null;
                }
            }
            if (path != null)
            {
                var dir = path[index] - transform.position;

                transform.position += dir.normalized * speed * Time.deltaTime;
            }

        }


    }

    void DrawPath()
    {
        if (line == null)
        {
            return;
        }

        if (finder.state == PathFinder.State.PATH_FOUND)
        {
            if (path != null)
            {
                line.positionCount = path.Length;
                line.SetPositions(path);
            }
        }
    }

    IEnumerator FindPath()
    {
        finder.Start = transform.position;
        finder.End = target;

        searching = true;
        while (finder.state == PathFinder.State.PATH_NOT_FOUND)
        {
            finder.Progress(nodesPerFrame);

            if (finder.End != target)
            {
                yield break;
            }
            if (finder.state == PathFinder.State.PATH_DOES_NOT_EXIST)
            {
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
        index = 0;
        path = finder.Path;
        searching = false;
        if (debugPath)
        {
            DrawPath();
        }



    }
    bool CheckPosition(Vector3 pos)
    {
        if (Physics.CheckSphere(pos, radius, 1))
        {
            return false;
        }
        return true;
    }

    bool GetClosestValidPosition(Vector3 pos, out Vector3 newPos)
    {
        if (CheckPosition(pos))
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
                Vector3 test = pos + dir * i * radius;
                if (CheckPosition(test))
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
