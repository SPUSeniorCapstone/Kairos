using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CommandController : MonoBehaviour
{
    public GameObject wayPoint;
    public List<CommandGroup> commandGroups = new List<CommandGroup>();
    [SerializeField] CommandGroup commandGroup;
    [SerializeField] GameObject playerFaction;
    // Start is called before the first frame update
    void Start()
    {
        commandGroups = FindObjectsOfType<CommandGroup>().ToList();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameController.Main.paused)
        {
            if (GameController.Main.InputController.Command.Down())
            {
                Debug.Log("Mouse1 down");
                if (GameController.Main.SelectionController.onEnemy)
                {
                    //-----------------------------
                    // less ugly way to get this, change later
                    MoveSelected(GameController.Main.SelectionController.enemy);
                }
                else
                {
                    RaycastHit hit;
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000, LayerMask.GetMask("Terrain")))
                    {
                        Vector3 point = hit.point;
                        point.y = 0;
                        wayPoint.transform.position = point;
                        MoveSelected(wayPoint);
                    }
                }
            }
        }
    }
    public void MoveSelected(GameObject target)
    {
        var cg = Instantiate<CommandGroup>(commandGroup,playerFaction.transform);
  
        commandGroups.Add(cg);
        //cg.groupTargetObj = target;
        foreach (Selectable selectable in GameController.Main.SelectionController.currentlySelect)
        {
            Entity entity = selectable.GetComponent<Entity>();
            if (entity == null)
            {
                entity = selectable.GetComponentInParent<Entity>();
            }
            if (entity != null)
            {
                CommandGroup old = entity.CommandGroup;
                if (old != null)
                {
                    entity.CommandGroup.entities.Remove(entity);
                }
                //entity.CommandGroup.entities.Remove(entity);
                entity.CommandGroup = cg;
                cg.entities.Add(entity);
                entity.idle = false;
            }      

            //var go = selectable.GetComponentInParent<CommandGroup>();
            //if (go == null)
            //{
            //    go = selectable.GetComponent<CommandGroup>();
            //}
            //go.SetGroupTarget(wayPoint);
            //cg.
        }
        cg.SetGroupTarget(target);
        
        // this doesn't work, changes length while in the loop
        //int count = commandGroups.Count;
        //for (int i = 0; i < count; i++)
        //{
        //    //Debug.Log(i.ToString() + " " +commandGroups.ElementAt(i));
        //    commandGroups.ElementAt(i).CheckIfEmpty();

        //}
    }
}
