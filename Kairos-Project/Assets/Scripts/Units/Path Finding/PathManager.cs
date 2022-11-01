using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PathManager
{
    public static PathManager main = null;
    public static void Init(PathManager finder)
    {
        main = finder;
    }

    public float cellSize = 1;

    public PathManager(float cellSize)
    {
        this.cellSize = cellSize;
    }

    public Vector2Int[] RequestPath(Vector2Int start, Vector2Int end, Func<Vector2Int, bool> IsValidMove = null)
    {
        if (IsValidMove == null)
        {
            IsValidMove = IsValidMovePosition;
        }
        if (!IsValidMove(start) || !IsValidMove(end))
        {
            return null;
        }

        PathFinder path = new PathFinder(IsValidMove);
        path.Start = start;
        path.End = end;
        return path.FindPath();
    }

    public Task<Vector2Int[]> RequestPathAsync(Vector2Int start, Vector2Int end, Func<Vector2Int, bool> IsValidMove = null)
    {
        if (IsValidMove == null)
        {
            IsValidMove = IsValidMovePosition;
        }
        PathFinder path = new PathFinder(IsValidMove);

        Task<Vector2Int[]> task = path.FindPathAsync(start, end);

        return task;
    }

    private bool IsValidMovePosition(Vector2Int position)
    {
        //Vector3 pos = MapController.main.grid.CellToWorld((Vector3Int)position);
        ////pos = new Vector3(pos.x, 0, pos.y);
        //if (Physics.CheckSphere(pos, 1))
        //{
        //    return false;
        //}
        int x = MapController.main.mapData.tiles.GetLength(0);
        int y = MapController.main.mapData.tiles.GetLength(1);

        if (position.x >= x || position.x < 0 || position.y >= y || position.y < 0) return false;

        if (MapController.main.mapData.tiles[position.x, position.y].isPassable)
        {
            return true;
        }
        return false;
    }
}
