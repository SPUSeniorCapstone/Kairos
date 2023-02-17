using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class PathFinder
{
    public float runTime = 10;

    public async Task<List<Vector3>> FindPath(Vector3 start, Vector3 end, int stepHeight, bool allowDiagonals = false, bool useWorldCoords = true)
    {
        return await Task.Run(() =>
        {
            return FindPathSyncronous(start, end, stepHeight, allowDiagonals, useWorldCoords);
        });

    }

    private List<Vector3> FindPathSyncronous(Vector3 start, Vector3 end, int stepHeight, bool allowDiagonals, bool useWorldCoords)
    {
        Vector3Int Start, End;

        if (useWorldCoords)
        {
            Start = WorldController.Main.WorldToBlockPosition(start).Flat();
            End = WorldController.Main.WorldToBlockPosition(end).Flat();
        }
        else
        {
            Start = start.ToVector3Int().Flat(); ;
            End = end.ToVector3Int().Flat();
        }
        

        var open = new NodeQueue();
        var closed = new HashSet<Vector3Int>();

        var startNode = new PathNode(Start, 0, Heuristic(Start, End));

        open.Enqueue(startNode);

        var startTime = DateTime.Now;
        while (open.Count > 0)
        {
            //if((DateTime.Now - startTime).Seconds > runTime)
            //{
            //    Debug.LogError("PATHFINDING EXCEEDED RUNTIME");
            //    return null;
            //}

            var current = open.Dequeue();

            if (current.position == End)
            {
                return GetPath(current);
            }

            closed.Add(current.position);

            foreach (var n in GetNeighbors(current, stepHeight, allowDiagonals))
            {
                var neighbor = n;
                if (closed.Contains(neighbor.position))
                {
                    continue;
                }

                Vector3Int diff = neighbor.position - current.position;

                float g = current.g + 1;
                if (diff.x != 0 && diff.z != 0)
                {
                    g = current.g + 2f;
                }

                if (!open.Contains(neighbor))
                {
                    neighbor.h = Heuristic(neighbor.position, End);
                    neighbor.parent = current;
                    neighbor.g = g;
                    open.Enqueue(neighbor);
                }
                else
                {
                    neighbor = open.GetNode(neighbor.position);
                    if(g < neighbor.g)
                    {
                        neighbor.parent = current;
                        neighbor.g = g;
                    }
                }
            }
        }
        return null;
    }

    int Heuristic(Vector3Int node, Vector3Int end)
    {
        return Mathf.Abs(node.x - end.x) + Mathf.Abs(node.z - end.z);
    }

    List<PathNode> GetNeighbors(PathNode node, int stepHeight, bool allowDiagonals)
    {
        List<PathNode> neighbors = new List<PathNode>();
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (x == 0 && z == 0)
                {
                    continue;
                }

                if (!allowDiagonals && (x != 0 && z != 0))
                {
                    continue;
                }

                Vector3Int pos = node.position + new Vector3Int(x, 0, z);

                if (pos.x > 0 && pos.x < WorldController.Main.World.widthInChunks * Chunk.width &&
                    pos.z > 0 && pos.z < WorldController.Main.World.lengthInChunks * Chunk.length &&
                    CheckMove(node.position, pos, stepHeight))
                {
                    neighbors.Add(new PathNode(pos));
                }
            }
        }
        return neighbors;
    }


    List<Vector3> GetPath(PathNode node)
    {
        var path = new List<Vector3>();
        var current = node;

        while(current != null)
        {
            path.Add(current.position + new Vector3(0.5f, 0, 0.5f) * WorldController.Main.blockScale);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }

    bool CheckMove(Vector3Int A, Vector3Int B, int stepHeight)
    {
        Vector3Int diff = B - A;
        if (Mathf.Abs(diff.x) > 1 || Mathf.Abs(diff.z) > 1)
        {
            return false;
        }




        int hA = WorldController.Main.World.GetHeight(A.x, A.z);
        int hB = WorldController.Main.World.GetHeight(B.x, B.z);

        if (MathF.Abs(hA - hB) > stepHeight || !WorldController.Main.World.IsPassable(B.x, B.z)) 
        {
            return false;
        }


        // Checks if diagonals work. Doesn't seem to work properly, but doesn't matter since unit 
        // Pathfinding shouldn't use diagonals
        if (diff.x != 0 && diff.z != 0)
        {
            Vector3Int diag = new Vector3Int(A.x + diff.x, 0, A.z);
            int h = WorldController.Main.World.GetHeight(diag.x, diag.z);
            if (MathF.Abs(hA - h) > stepHeight || !WorldController.Main.World.IsPassable(diag.x, diag.z))
            {
                return false;
            }

            diag = new Vector3Int(A.x, 0, A.z + diff.z);
            h = WorldController.Main.World.GetHeight(diag.x, diag.z);
            if (MathF.Abs(hA - h) > stepHeight || !WorldController.Main.World.IsPassable(diag.x, diag.z))
            {
                return false;
            }
        }
        return true;
    }

    public class PathNode : IComparable<PathNode>
    {
        public Vector3Int position;
        public float g = -1;
        public int h = -1;
        public PathNode parent = null;

        public PathNode(Vector3Int position)
        {
            this.position = position;
        }

        public PathNode(Vector3Int position, float g, int h)
        {
            this.position = position;
            this.g = g;
            this.h = h;
        }

        public int CompareTo(PathNode other)
        {
            return (g + h).CompareTo(other.g + other.h);
        }
    }

    public class NodeQueue
    {
        PriorityQueue<PathNode> priorityQueue = new PriorityQueue<PathNode>(PriorityQueueMode.MIN);
        Dictionary<Vector3Int, PathNode> hash = new Dictionary<Vector3Int, PathNode>();


        public void Enqueue(PathNode node)
        {
            priorityQueue.Enqueue(node);
            hash.Add(node.position, node);
        }

        public PathNode Dequeue()
        {
            var ret = priorityQueue.Dequeue();
            hash.Remove(ret.position);
            return ret;
        }

        public PathNode Peek()
        {
            return priorityQueue.Peek();
        }

        public bool Contains(PathNode node)
        {
            return hash.ContainsKey(node.position);
        }

        public PathNode GetNode(Vector3Int pos)
        {
            return hash[pos];
        }

        public int Count
        {
            get { return priorityQueue.Count; }
        }
    }
}


