using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class CheckPathFinding : MonoBehaviour
{
    public GameObject start;
    public GameObject end;
    public LineRenderer pathLine;

    public Task<List<Vector3>> task;

    private void Update()
    {
        if(task != null && task.IsCompleted)
        {
            List<Vector3> path = task.Result;   

            if(path == null)
            {
                Debug.Log("Path not found");
            }
            else
            {
                pathLine.positionCount = (path.Count);
                pathLine.SetPositions(path.ToArray());
            }
        }
    }

    [Button(nameof(GeneratePath))]
    public bool Button_GeneratePath;
    public void GeneratePath()
    {
        //task = GameController.Main.PathFinder.FindPath(start.transform.position, end.transform.position);

    }

}
