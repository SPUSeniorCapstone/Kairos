using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{
    public enum State
    {
        PATH_NOT_FOUND,
        PATH_FOUND,
        PATH_DOES_NOT_EXIST
    }

    public bool TrackNodes = true;
    public List<PathNode> NewNodes = new List<PathNode>();

    public PathNode[] Nodes
    {
        get
        {
            if(nodes == null)
            {
                return null;
            }
            return nodes.Nodes;
        }
    }

    public State state = State.PATH_NOT_FOUND;

    Func<Vector3, bool> IsValidMovePosition;
    PathNodeList nodes = null;

    public float step = 1;

    public Vector3 Start
    {
        get
        {
            return start;
        }
        set
        {
            if(value != start)
            {
                start = value;
                ResetFinder();
            }
        }
    }
    public Vector3 End
    {
        get
        {
            return end;
        }
        set
        {
            if (value != end)
            {
                end = value;
                ResetFinder();
            }
        }
    }

    Vector3 start, end;

    public PathNode currentNode = null;

    public Vector3[] directions =
    {
        //Adjacent 
        new Vector3(0,0,1),
        new Vector3(0,0,-1),
        new Vector3(-1,0,0),
        new Vector3(1,0,0),
                  
        //Diagonal
        new Vector3(1,0,1),
        new Vector3(-1,0,-1),
        new Vector3(-1,0,1),
        new Vector3(1,0,-1),
    };

    public Vector3[] Path;

    public float maxRunTime = 5;

    public PathFinder(Func<Vector3, bool> IsValidMovePosition)
    {
        this.IsValidMovePosition = IsValidMovePosition;
    }

    public PathFinder(Vector3 start, Vector3 end, Func<Vector3, bool> IsValidMovePosition)
    {
        this.start = start;
        this.end = end;
        this.IsValidMovePosition = IsValidMovePosition;
    }

    public void ResetFinder()
    {
        currentNode = null;
        
        if (nodes == null)
        {
            nodes = new PathNodeList(PathNode.GetNewNode(start, 0, Vector3.Distance(start, end), null, PathNode.NodeState.OPEN));
        }
        else
        {
            nodes.Clear();
            nodes.AddNode(PathNode.GetNewNode(start, 0, Vector3.Distance(start, end), null, PathNode.NodeState.OPEN));
        }
        Path = null;

        state = State.PATH_NOT_FOUND;
    }

    public Vector3[] FindPath(Vector3 start, Vector3 end)
    {
        DateTime startTime = DateTime.UtcNow;

        Start = start;
        End = end;

        if(Path != null)
        {
            return Path;
        }

        while(state == State.PATH_NOT_FOUND)
        {
            if((DateTime.UtcNow - startTime).Seconds > maxRunTime)
            {
                Debug.LogError("Pathfinder timed out");
                return null;
            }
            Progress(1);
        }

        if (state == State.PATH_DOES_NOT_EXIST)
        {
            return null;
        }
        else
        {
            ReconstructPath();
        }

        return Path;
    }

    private void ReconstructPath()
    {
        if(state == State.PATH_NOT_FOUND)
        {
            Debug.Log("Cannot reconstruct path because a path has not been found");
            return;
        }
        if(state == State.PATH_DOES_NOT_EXIST)
        {
            Debug.Log("Cannot reconstruct path because a path does not exist");
            return;
        }

        DateTime startTime = DateTime.UtcNow;


        var current = currentNode;
        List<Vector3> path = new List<Vector3>();
        while(current != null)
        {
            if ((DateTime.UtcNow - startTime).Seconds > maxRunTime)
            {
                Debug.LogError("Pathfinder reconstruction timed out");
                return;
            }

            path.Insert(0,current.position);
            current = current.previous;
        }

        if (path[0] != start)
        {
            Debug.LogError("Path could not be reconstructed");
            return;
        }

        Path = path.ToArray();
    }

    public void Progress(int steps)
    {
        if (TrackNodes)
        {
            NewNodes.Clear();
        }

        if (nodes == null)
        {
            nodes = new PathNodeList(PathNode.GetNewNode(start, 0, Vector3.Distance(start, end), null, PathNode.NodeState.OPEN));
        }

        if(steps <= 0)
        {
            Debug.LogError("Cannot have negative number of steps");
            return;
        }

        if(state == State.PATH_FOUND ){
            Debug.Log("Path already found");
            return;
        }
        
        if(state == State.PATH_DOES_NOT_EXIST)
        {
            Debug.Log("Path does not exist");
            return;
        }

        for(int i = 0; i < steps; i++)
        {
            currentNode = nodes.GetNextNode();
            if(currentNode == null)
            {
                state = State.PATH_DOES_NOT_EXIST;
                return;
            }

            if(Vector3.Distance(currentNode.position, end) < step)
            {
                state = State.PATH_FOUND;
                return;
            }

            if (IsValidMovePosition(currentNode.position))
            {
                CreateAdjacentPathNodes();
            }
            else
            {
                currentNode.state = PathNode.NodeState.RESTRICTED;
                nodes.UpdateNode(currentNode);
                if (TrackNodes)
                {
                    NewNodes.Add(currentNode);
                }
            }

        }


    }

    public void CreateAdjacentPathNodes()
    {
        foreach (var dir in directions)
        {
            Vector3 newPosition = currentNode.position + (dir * step);

            if(newPosition != start )
            {
                if(currentNode.previous == null || newPosition != currentNode.previous.position)
                {
                    var n = PathNode.GetNewNode(newPosition, currentNode.g_cost + (dir.magnitude * step), Vector3.Distance(newPosition, end), currentNode, PathNode.NodeState.OPEN);
                    nodes.AddNode(n);

                    if (TrackNodes)
                    {
                        NewNodes.Add(n);
                    }
                }
            }


        }

    }

}
