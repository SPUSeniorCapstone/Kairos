using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DebugPath : MonoBehaviour
{
    public Vector2Int start;
    public Vector2Int end;
    PathFinder finder = new PathFinder(PathManager.main.IsValidMovePosition, PathManager.main.MovePositionWeight);

    public bool ShowNode = false;

    private void OnDrawGizmos()
    {
        if(ShowNode && finder != null)
        {
            foreach(var node in finder.Nodes)
            {
                Gizmos.DrawSphere(MapController.main.grid.CellToWorld(new Vector3Int(node.position.x, 0, node.position.y)), 1);
                if (node.state == PathNode.NodeState.OPEN)
                {
                }
            }
        }
    }

    [SerializeField]
    [Button(nameof(FindPath))]
    bool _FindPathButton;
    public void FindPath()
    {
        if(finder == null)
        {
            finder = new PathFinder(PathManager.main.IsValidMovePosition, PathManager.main.MovePositionWeight);
        }
        finder.Start = start;
        finder.End = end;
        finder.Progress(100);
    }
}
