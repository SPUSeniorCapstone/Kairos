using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

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

    public bool useWeights = false;

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
    Func<Vector2Int, bool> IsValidMovePosition;

    /// <summary>
    /// A function for getting the move weight of a specified position
    /// </summary>
    Func<Vector2Int, float> MovePositionWeight;

    /// <summary>
    /// Object to keep track of the nodes in the PathFinder
    /// </summary>
    PathNodeList nodes = null;

    /// <summary>
    /// The step size the Pathfinder should take when searching for a new path
    /// </summary>
    //public float step = 1;

    /// <summary>
    /// The start position of the PathFinder
    /// </summary>
    public Vector2Int Start
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
    private Vector2Int start;

    /// <summary>
    /// The end position of the Pathfinder
    /// </summary>
    public Vector2Int End
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
    private Vector2Int end;

    /// <summary>
    /// The current node of the pathfinder
    /// </summary>
    PathNode currentNode = null;

    /// <summary>
    /// The relative position from the center of each node to create new nodes at
    /// </summary>
    public Vector2Int[] directions =
    {
        //Adjacent 
        new Vector2Int(0,1),
        new Vector2Int(0,-1),
        new Vector2Int(-1,0),
        new Vector2Int(1,0),
                  
        //Diagonal
        new Vector2Int(1,1),
        new Vector2Int(-1,-1),
        new Vector2Int(-1,1),
        new Vector2Int(1,-1),
    };

    /// <summary>
    /// The Path from start to end. <b>null</b> if no path has been found or if path does not exist
    /// </summary>
    public Vector2Int[] Path
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
    Vector2Int[] path;

    /// <summary>
    /// The maximum amount of time to search for a path before timeing out
    /// </summary>
    public float maxRunTime = 5;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="IsValidMovePosition">A function for deterimining if a position on the map can be traversed</param>
    public PathFinder(Func<Vector2Int, bool> IsValidMovePosition)
    {
        this.IsValidMovePosition = IsValidMovePosition;
        useWeights = false;
    }

    public PathFinder(Func<Vector2Int, bool> IsValidMovePosition, Func<Vector2Int, float> MovePositionWeight)
    {
        this.IsValidMovePosition = IsValidMovePosition;
        this.MovePositionWeight = MovePositionWeight;
        useWeights = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="IsValidMovePosition">A function for deterimining if a position on the map can be traversed</param>
    public PathFinder(Vector2Int start, Vector2Int end, Func<Vector2Int, bool> IsValidMovePosition)
    {
        this.start = start;
        this.end = end;
        this.IsValidMovePosition = IsValidMovePosition;
        useWeights = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="MovePositionWeight">A function that returns the weight of a given positon. A weight of 0 means not passable</param>
    public PathFinder(Vector2Int start, Vector2Int end, Func<Vector2Int, bool> IsValidMovePosition, Func<Vector2Int, float> MovePositionWeight)
    {
        this.start = start;
        this.end = end;
        this.IsValidMovePosition = IsValidMovePosition;
        this.MovePositionWeight = MovePositionWeight;
        useWeights = true;
    }

    /// <summary>
    /// Resets the PathFinder
    /// </summary>
    public void ResetFinder()
    {
        currentNode = null;

        nodes = new PathNodeList(PathNode.GetNewNode(start, 0, Vector2Int.Distance(start, end), null, PathNode.NodeState.OPEN));


        if (nodes == null)
        {
            nodes = new PathNodeList(PathNode.GetNewNode(start, 0, Vector2Int.Distance(start, end), null, PathNode.NodeState.OPEN));
        }
        else
        {
            nodes.Clear();
            nodes.AddNode(PathNode.GetNewNode(start, 0, Vector2Int.Distance(start, end), null, PathNode.NodeState.OPEN));
        }
        path = null;

        state = State.PATH_NOT_FOUND;
    }

    public async Task<Vector2Int[]> FindPathAsync(Vector2Int start, Vector2Int end)
    {
        Start = start;
        End = end;
        return await Task.Run<Vector2Int[]>(FindPath);
    }

    /// <summary>
    /// Returns a valid path from start to end or null if no path exists
    /// <para>Avoid running this on the main thread. Use Progress() on the main thread instead</para>
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public Vector2Int[] FindPath()
    {
        DateTime startTime = DateTime.UtcNow;



        if(path != null)
        {
            return path;
        }

        while(state == State.PATH_NOT_FOUND)
        {
            if((DateTime.UtcNow - startTime).Seconds > maxRunTime)
            {
                //Debug.LogError("Pathfinder timed out");
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
        List<Vector2Int> path = new List<Vector2Int>();
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
            nodes = new PathNodeList(PathNode.GetNewNode(start, 0, Vector2Int.Distance(start, end), null, PathNode.NodeState.OPEN));
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

            if(Vector2Int.Distance(currentNode.position, end) == 0)
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
            Vector2Int newPosition = (currentNode.position + (dir));

            if(newPosition != start )
            {
                if(currentNode.previous == null || newPosition != currentNode.previous.position)
                {
                    

                    //if (!nodes.Contains(newPosition))
                    //{
                        var n = PathNode.GetNewNode(newPosition, (GetG_Cost(currentNode.g_cost, newPosition, dir)) , GetH_Cost(newPosition), currentNode, PathNode.NodeState.OPEN);
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

    float GetG_Cost(float prev, Vector2Int position, Vector2Int dir)
    {
        float ret = prev;
        if (useWeights)
        {
            ret += dir.magnitude / MovePositionWeight(position);
        }
        return ret;
    }

    float GetH_Cost(Vector2Int position)
    {
        float ret = Vector2Int.Distance(position, end);
        if (useWeights)
        {
            ret /= MovePositionWeight(position);
        }
        return ret;
    }

    /// <summary>
    /// [TESTING]
    /// </summary>
    public void RunTest()
    {
        nodes.RunTest();
    }

}
