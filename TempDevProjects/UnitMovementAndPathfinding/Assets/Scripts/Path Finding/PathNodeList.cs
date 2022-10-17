using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PathNodeList
{



    public PathNode[] Nodes
    {
        get { return nodes.Values.ToArray(); }
    }


    Dictionary<Vector3, PathNode> nodes = new Dictionary<Vector3, PathNode>();
    PriorityQueue<float, PathNode> openNodes = new PriorityQueue<float, PathNode>();


    public PathNodeList(PathNode startNode)
    {
        AddNode(startNode);
    }

    public PathNode GetNextNode()
    {
        var r = openNodes.Dequeue();
        if(r == null)
        {
            return null;
        }
        r.state = PathNode.NodeState.CLOSED;
        return r;
    }

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
                    openNodes.Enqueue(nodes[node.position].Total_Cost, nodes[node.position]);
                }
                return;
            }
            else
            {
                return;
            }
        }

        if(node.state == PathNode.NodeState.OPEN)
        {
            openNodes.Enqueue(node.Total_Cost, node);
        }
        nodes.Add(node.position, node);

    }

    public void UpdateNode(PathNode node)
    {
        if (!nodes.ContainsKey(node.position))
        {
            AddNode(node);
            if(node.state == PathNode.NodeState.OPEN)
            {
                openNodes.Enqueue(node.Total_Cost, node);
            }
        }
        else
        {
            nodes[node.position] = node;
        }
    }

    public bool Contains(Vector3 position)
    {
        return nodes.ContainsKey(position);
    }

    public void Clear()
    {
        nodes.Clear();
        openNodes.Clear();
    }

}
