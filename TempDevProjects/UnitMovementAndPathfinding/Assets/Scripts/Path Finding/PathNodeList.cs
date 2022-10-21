using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PathNodeList
{
    /// <summary>
    /// Returns all of the nodes in the list
    /// </summary>
    public PathNode[] Nodes
    {
        get { return nodes.Values.ToArray(); }
    }


    Dictionary<Vector3, PathNode> nodes = new Dictionary<Vector3, PathNode>();
    PriorityQueue<float, PathNode> openNodes = new PriorityQueue<float, PathNode>();

    /// <summary>
    /// Creates a new PathNodeList with a starting node
    /// </summary>
    public PathNodeList(PathNode startNode)
    {
        AddNode(startNode);
    }

    /// <summary>
    /// Returns the next best path node to the user
    /// </summary>
    public PathNode GetNextNode()
    {
        var r = openNodes.Dequeue();

        foreach (var item in openNodes.list.values)
        {
            if(r.h_cost > item.Key)
            {
                Debug.LogError("Didn't recieve the min item");
            }
        }

        if(r == null)
        {
            return null;
        }
        r.state = PathNode.NodeState.CLOSED;
        return r;
    }

    /// <summary>
    /// Adds a new PathNode to the Priority Queue
    /// </summary>
    /// <param name="node">The node to be added</param>
    public void AddNode(PathNode node)
    {
        if (nodes.ContainsKey(node.position))
        {
            if(
                node.previous.g_cost < nodes[node.position].previous.g_cost && 
                nodes[node.position].state != PathNode.NodeState.OPEN && 
                nodes[node.position].previous != null && 
                node.previous.previous.position != nodes[node.position].position &&
                nodes[node.position].state != PathNode.NodeState.RESTRICTED
                )
            {
                nodes[node.position].previous = node.previous;
                nodes[node.position].g_cost = node.g_cost;
                if (node.state == PathNode.NodeState.OPEN)
                {
                    openNodes.Enqueue(nodes[node.position].h_cost, nodes[node.position]);
                }
            }
            return;
        }

        if(node.state == PathNode.NodeState.OPEN)
        {
            openNodes.Enqueue(node.h_cost, node);
        }
        nodes.Add(node.position, node);

    }

    /// <summary>
    /// Updates the node that exists at the given position
    /// </summary>
    /// <param name="node">The node to be updated</param>
    public void UpdateNode(Vector3 position, PathNode node)
    {
        if(position != node.position)
        {
            Debug.LogError("position and node.position must match");
            return;
        }

        if (!nodes.ContainsKey(position))
        {
            AddNode(node);
            if(node.state == PathNode.NodeState.OPEN)
            {
                openNodes.Enqueue(node.h_cost, node);
            }
        }
        else
        {
            nodes[node.position] = node;
        }
    }


    /// <summary>
    /// Checks if a node exists for the existing position
    /// <para>(must be the exact position)</para>
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool Contains(Vector3 position)
    {
        return nodes.ContainsKey(position);
    }

    /// <summary>
    /// Clears the list
    /// </summary>
    public void Clear()
    {
        nodes.Clear();
        openNodes.Clear();
    }

    /// <summary>
    /// [TESTING] Checks if that values of the list are properly order
    /// <para>Do not use except for testing. This is not efficient</para>
    /// </summary>
    public void RunTest()
    {
        openNodes.RunTest();
    }
}
