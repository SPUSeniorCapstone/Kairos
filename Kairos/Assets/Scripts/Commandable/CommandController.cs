using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CommandController : MonoBehaviour
{
    public GameObject wayPoint;
    public float debugY;
    public float fadeTime;
    public float fadeTimer = 5f;

    public int stepHeight = 1;

    /// <summary>
    /// Command Group Master List
    /// </summary>
    public List<CommandGroup> CommandGroups
    {
        get { return commandGroups; }
    }
    public List<CommandGroup> commandGroups = new List<CommandGroup>();

    public bool attackCommand = false;

    [SerializeField] CommandGroup commandGroup;
    [SerializeField] GameObject playerFaction;

    void Update()
    {
        if (!GameController.Main.paused)
        {
            if (wayPoint.activeSelf)
            {
                if (Time.time - fadeTime > fadeTimer)
                {
                    wayPoint.SetActive(false);
                }
                
            }

            if (GameController.Main.InputController.Command.Down())
            {
                Debug.Log("Mouse1 down");
                if (GameController.Main.SelectionController.onEnemy)
                {
                    //-----------------------------
                    // less ugly way to get this, change later
                    attackCommand = true;
                    if (GameController.Main.SelectionController.currentlySelect.Count > 0)
                    {
                        AttackWithSelected(GameController.Main.SelectionController.actionTarget);
                    }
                }
                else if (GameController.Main.SelectionController.actionTarget != null)
                {
                    foreach (var s in GameController.Main.SelectionController.currentlySelect)
                    {
                        if (s.GetComponent<Unit>() != null)
                        {
                            s.GetComponent<Unit>().PerformTaskOn(GameController.Main.SelectionController.actionTarget.GetComponent<Selectable>());
                        }
                    }
                }
                else
                {
                    RaycastHit hit;
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000, LayerMask.GetMask("Terrain")))
                    {
                        wayPoint.SetActive(true);
                        fadeTime = Time.time;
                        Vector3 point = hit.point;
                        point.y = GameController.Main.WorldController.World.GetHeight(point.x, point.z) + 0.05f;
                        wayPoint.transform.position = point;
                        attackCommand = false;
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
                CommandGroup old = u.commandGroup;
                if (old != null)
                {
                    old.unitList.Remove(u);
                }
                u.ClearTarget();                
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
                CommandGroup old = unit.commandGroup;
                if (old != null)
                {
                    old.unitList.Remove(unit);
                }
                unit.commandGroup = cg;
                cg.unitList.Add(unit);
            }

            if (production != null && unit == null)
            {
                Debug.Log("Structure not null");
                production.rallyPoint.GetComponentInChildren<MeshRenderer>().enabled = true;
                production.rallyPoint.transform.position = wayPoint.transform.position;
            }
        }

        cg.CalculateCenter();
        //Debug.Log("AFTER LOOP: cg.entites = " + cg.entities[0].name);
        cg.pathTask = GameController.Main.PathFinder.FindPath(cg.transform.position, target, stepHeight, false);
        cg.retrievingPath = true;

        //DEBUG
        if (GetComponent<CheckPathFinding>() != null)
            GetComponent<CheckPathFinding>().task = cg.pathTask;


        commandGroups.Add(cg);
        Debug.Log("Added cg");


        //cg.SetGroupTarget(target);

        // this doesn't work, changes length while in the loop
        //int count = commandGroups.Count;
        //for (int i = 0; i < count; i++)
        //{
        //    //Debug.Log(i.ToString() + " " +commandGroups.ElementAt(i));
        //    commandGroups.ElementAt(i).CheckIfEmpty();

        //}
    }
    public void AttackWithSelected(GameObject target)
    {
        if (GameController.Main.SelectionController.currentlySelect.Count == 1 && GameController.Main.SelectionController.currentlySelect[0].GetComponent<ProductionStructure>() == null)
        {
            var t = GameController.Main.SelectionController.currentlySelect[0];
            var u = t.GetComponent<Unit>();
            if (u != null)
            {
                u.ClearTarget();
                u.PerformTaskOn(target.GetComponent<Selectable>());
                u.AttackCommand();
            }
            return;
        }

        var cg = Instantiate<CommandGroup>(commandGroup, playerFaction.transform);

        cg.followSpeed = -1;
        foreach (Selectable selectable in GameController.Main.SelectionController.currentlySelect)
        {
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
                CommandGroup old = unit.commandGroup;
                if (old != null)
                {
                    old.unitList.Remove(unit);
                }
                unit.commandGroup = cg;
                cg.unitList.Add(unit);
                unit.ClearTarget();
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
                production.rallyPoint.GetComponentInChildren<MeshRenderer>().enabled = true;
                production.rallyPoint.transform.position = wayPoint.transform.position;
            }
        }

        cg.CalculateCenter();
        //Debug.Log("AFTER LOOP: cg.entites = " + cg.entities[0].name);
        cg.pathTask = GameController.Main.PathFinder.FindPath(cg.transform.position, target.transform.position, stepHeight, false);

        //DEBUG
        if (GetComponent<CheckPathFinding>() != null)
            GetComponent<CheckPathFinding>().task = cg.pathTask;


        commandGroups.Add(cg);
        foreach (Unit unit in cg.unitList)
        {
            unit.PerformTaskOn(target.GetComponent<Selectable>());
            unit.AttackCommand();
        }
    }
}
