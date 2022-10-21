using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{
    /// <summary>
    /// Stores the current state of the finder object
    /// </summary>
    public enum State
    {
        PATH_NOT_FOUND,
        PATH_FOUND,
        PATH_DOES_NOT_EXIST
    }

    /// <summary>
    /// Determines whether or nor to track changes on nodes
    /// </summary>
    public bool TrackNodes = true;
    /// <summary>
    /// if TrackNodes is enabled, this stores all the changes made since the last call of Progress()
    /// </summary>
    public List<PathNode> NewNodes = new List<PathNode>();

    /// <summary>
    /// Returns all of the current nodes in the PathFinder
    /// </summary>
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

    /// <summary>
    /// The state of the PathFinder
    /// </summary>
    public State state = State.PATH_NOT_FOUND;

    /// <summary>
    /// A function for determining if a move position is valid
    /// </summary>
    Func<Vector3, bool> IsValidMovePosition;
    /// <summary>
    /// Object to keep track of the nodes in the PathFinder
    /// </summary>
    PathNodeList nodes = null;

    /// <summary>
    /// The step size the Pathfinder should take when searching for a new path
    /// </summary>
    public float step = 1;

    /// <summary>
    /// The start position of the PathFinder
    /// </summary>
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
    private Vector3 start;

    /// <summary>
    /// The end position of the Pathfinder
    /// </summary>
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
    private Vector3 end;

    /// <summary>
    /// The current node of the pathfinder
    /// </summary>
    PathNode currentNode = null;

    /// <summary>
    /// The relative position from the center of each node to create new nodes at
    /// </summary>
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

    /// <summary>
    /// The Path from start to end. <b>null</b> if no path has been found or if path does not exist
    /// </summary>
    public Vector3[] Path
    {
        get
        {
            if(path == null && state == State.PATH_FOUND)
            {
                ReconstructPath();
            }
            return path;
        }
    }
    Vector3[] path;

    /// <summary>
    /// The maximum amount of time to search for a path before timeing out
    /// </summary>
    public float maxRunTime = 5;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="IsValidMovePosition">A function for deterimining if a position on the map can be traversed</param>
    public PathFinder(Func<Vector3, bool> IsValidMovePosition)
    {
        this.IsValidMovePosition = IsValidMovePosition;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="IsValidMovePosition">A function for deterimining if a position on the map can be traversed</param>
    public PathFinder(Vector3 start, Vector3 end, Func<Vector3, bool> IsValidMovePosition)
    {
        this.start = start;
        this.end = end;
        this.IsValidMovePosition = IsValidMovePosition;
    }


    /// <summary>
    /// Resets the PathFinder
    /// </summary>
    public void ResetFinder()
    {
        currentNode = null;

        nodes = new PathNodeList(PathNode.GetNewNode(start, 0, Vector3.Distance(start, end), null, PathNode.NodeState.OPEN));


        if (nodes == null)
        {
            nodes = new PathNodeList(PathNode.GetNewNode(start, 0, Vector3.Distance(start, end), null, PathNode.NodeState.OPEN));
        }
        else
        {
            nodes.Clear();
            nodes.AddNode(PathNode.GetNewNode(start, 0, Vector3.Distance(start, end), null, PathNode.NodeState.OPEN));
        }
        path = null;

        state = State.PATH_NOT_FOUND;
    }

    /// <summary>
    /// Returns a valid path from start to end or null if no path exists
    /// <para>Avoid running this on the main thread. Use Progress() on the main thread instead</para>
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public Vector3[] FindPath(Vector3 start, Vector3 end)
    {
        DateTime startTime = DateTime.UtcNow;

        Start = start;
        End = end;

        if(path != null)
        {
            return path;
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

        return path;
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

        this.path = path.ToArray();
    }

    /// <summary>
    /// Progresses steps number of times to the next node
    /// </summary>
    /// <param name="steps"></param>
    public void Progress(int steps = 1)
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
                nodes.UpdateNode(currentNode.position, currentNode);
                
            }

            if (TrackNodes)
            {
                NewNodes.Add(currentNode);
            }
        }


    }

    /// <summary>
    /// Creates the Adjacent nodes to the current node
    /// </summary>
    public void CreateAdjacentPathNodes()
    {
        foreach (var dir in directions)
        {
            Vector3 newPosition = currentNode.position + (dir * step);

            if(newPosition != start )
            {
                if(currentNode.previous == null || newPosition != currentNode.previous.position)
                {
                    

                    //if (!nodes.Contains(newPosition))
                    //{
                        var n = PathNode.GetNewNode(newPosition, currentNode.g_cost + (dir.magnitude * step), Vector3.Distance(newPosition, end), currentNode, PathNode.NodeState.OPEN);
                        nodes.AddNode(n);
                        if (TrackNodes)
                        {
                            NewNodes.Add(n);
                        }
                    //}


                }
            }


        }

    }

    /// <summary>
    /// [TESTING]
    /// </summary>
    public void RunTest()
    {
        nodes.RunTest();
    }

}
