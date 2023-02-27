using System.Collections.Generic;
using UnityEngine;

public class CommandController : MonoBehaviour
{
    public GameObject wayPoint;
    public float debugY;

    public int stepHeight = 1;

    /// <summary>
    /// Command Group Master List
    /// </summary>
    public List<CommandGroup> CommandGroups
    {
        get { return commandGroups; }
    }
    List<CommandGroup> commandGroups = new List<CommandGroup>();


    [SerializeField] CommandGroup commandGroup;
    [SerializeField] GameObject playerFaction;

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
                    //MoveSelected(GameController.Main.SelectionController.enemy);
                }
                else
                {
                    RaycastHit hit;
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000, LayerMask.GetMask("Terrain")))
                    {
                        Debug.Log(hit);
                        Vector3 point = hit.point;
                        point.y = GameController.Main.WorldController.World.GetHeight(point.x, point.z) + 0.05f;
                        wayPoint.transform.position = point;
                        if (GameController.Main.SelectionController.currentlySelect.Count > 0)
                        {
                            MoveSelected(point);
                        }
                    }
                }
            }
        }
    }
    public void MoveSelected(Vector3 target)
    {
        if (GameController.Main.SelectionController.currentlySelect.Count == 1 && GameController.Main.SelectionController.currentlySelect[0].GetComponent<ProductionStructure>() == null)
        {
            var t = GameController.Main.SelectionController.currentlySelect[0];
            var u = t.GetComponent<Unit>();
            if (u != null)
            {
                u.MoveTo(target);
            }
            return;
        }

        var cg = Instantiate<CommandGroup>(commandGroup, playerFaction.transform);

        cg.followSpeed = -1;
        foreach (Selectable selectable in GameController.Main.SelectionController.currentlySelect)
        {
            Debug.Log("Try strucute");
            Unit unit = selectable.GetComponent<Unit>();
            ProductionStructure production = selectable.GetComponent<ProductionStructure>();
            //if (entity == null)
            //{
            //    entity = selectable.GetComponentInParent<Entity>();
            //}
            if (unit != null)
            {
                //Debug.Log("entity does not = null (MOVESELECTED)");
                //entity.pathindex = 0;
                CommandGroup old = unit.command;
                if (old != null)
                {
                    old.unitList.Remove(unit);
                }
                unit.command = cg;
                cg.unitList.Add(unit);
                //if (entity.movementSpeed < cg.followSpeed || cg.followSpeed == -1)
                //{
                //    cg.followSpeed = entity.movementSpeed;
                //}
                //entity.GetComponent<Unit>().isAttacking = false;
                ////entity.idle = false;
            }

            if (production != null)
            {
                Debug.Log("Structure not null");
                production.rallyPoint = wayPoint.transform.position;
            }
        }

        cg.CalculateCenter();
        //Debug.Log("AFTER LOOP: cg.entites = " + cg.entities[0].name);
        cg.pathTask = GameController.Main.PathFinder.FindPath(cg.transform.position, target, stepHeight, false);

        //DEBUG
        if (GetComponent<CheckPathFinding>() != null)
            GetComponent<CheckPathFinding>().task = cg.pathTask;


        commandGroups.Add(cg);
        foreach (Unit unit in cg.unitList)
        {
            unit.MoveTo(target);
        }

        //cg.SetGroupTarget(target);

        // this doesn't work, changes length while in the loop
        //int count = commandGroups.Count;
        //for (int i = 0; i < count; i++)
        //{
        //    //Debug.Log(i.ToString() + " " +commandGroups.ElementAt(i));
        //    commandGroups.ElementAt(i).CheckIfEmpty();

        //}
    }
}
