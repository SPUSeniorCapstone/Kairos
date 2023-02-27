using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CheckPathFinding : MonoBehaviour
{
    public GameObject start;
    public GameObject end;
    public LineRenderer pathLine;
    public int stepHeight = 0;

    public Task<List<Vector3>> task;

    private void Update()
    {
        if (task != null && task.IsCompleted)
        {
            List<Vector3> path = task.Result;

            if (path == null)
            {
                Debug.Log("Path not found");
                task = null;
            }
            else
            {
                pathLine.positionCount = (path.Count);
                for (int i = 0; i < path.Count; i++)
                {
                    //path[i] = new Vector3(path[i].x + 0.5f, 10, path[i].z + 0.5f);
                    path[i] = new Vector3(path[i].x, GameController.Main.WorldController.World.GetHeight(path[i].x, path[i].z) + 0.5f, path[i].z);
                }
                pathLine.SetPositions(path.ToArray());
                task = null;
            }
        }
    }

    [Button(nameof(GeneratePath))]
    public bool Button_GeneratePath;
    public void GeneratePath()
    {
        task = GameController.Main.PathFinder.FindPath(start.transform.position, end.transform.position, stepHeight);
    }

}
