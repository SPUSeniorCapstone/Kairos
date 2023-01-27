using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode 
{
    public enum NodeState
    {
        OPEN, 
        CLOSED,
        RESTRICTED,
        PATH
    }

    public Vector2Int position{ get; private set; }
    
    /// <summary>
    /// Distance from starting node
    /// </summary>
    public float g_cost{ get; set; }
    
    /// <summary>
    /// Distance from end node
    /// </summary>
    public float h_cost { get; set; }
    public float Total_Cost
    {
        get { return g_cost + h_cost; }
    }

    public NodeState state = NodeState.OPEN;

    public PathNode previous = null;




    /// <summary>
    /// <para>Instantiates a new Node for use</para>
    /// ! For increased efficiency, consider implimenting an object pool !
    /// </summary>
    /// <param name="position">The position on the map the node represents</param>
    /// <param name="g_cost">Distance cost from starting node</param>
    /// <param name="h_cost">Distance cost from end node</param>
    /// <param name="state">The current state of the Node</param>
    public static PathNode GetNewNode(Vector2Int position, float g_cost, float h_cost, NodeState state = NodeState.OPEN)
    {
        PathNode ret = new PathNode(position, g_cost, h_cost, state);
        return ret;
    }

    PathNode(Vector2Int position, float g_cost, float h_cost, NodeState state)
    {
        this.position = position;
        this.g_cost = g_cost;
        this.h_cost = h_cost;
        this.state = state;
    }

    /// <summary>
    /// <para>Instantiates a new Node for use</para>
    /// ! For increased efficiency, consider implimenting an object pool !
    /// </summary>
    /// <param name="position">The position on the map the node represents</param>
    /// <param name="g_cost">Distance cost from starting node</param>
    /// <param name="h_cost">Distance cost from end node</param>
    /// <param name="prev">The path node with the lowest g_cost adjacent to this node</param>
    /// <param name="state">The current state of the Node</param>
    public static PathNode GetNewNode(Vector2Int position, float g_cost, float h_cost, PathNode previous, NodeState state = NodeState.OPEN)
    {
        PathNode ret = new PathNode(position, g_cost, h_cost, previous, state);
        return ret;
    }

    PathNode(Vector2Int position, float g_cost, float h_cost, PathNode previous, NodeState state)
    {
        this.position = position;
        this.g_cost = g_cost;
        this.h_cost = h_cost;
        this.previous = previous;
        this.state = state;
    }
}
