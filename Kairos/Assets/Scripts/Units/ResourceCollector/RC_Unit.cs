using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(RC_Entity))]
public class RC_Unit : Unit
{

    public string Task;

    public float personalDistance = 1;



    // if certain stance, flee when enemy detected
    public Vector3 display;



    public Damageable target;

    public Stronghold Stronghold;
    public ResourceNode ResourceNode;

    // should this be public? protected?
    public RC_Entity entity;

    private void Start()
    {
        base.Start();
        // is this supposed to be for on spawn moving to way point?
        if (GetComponent<CheckPathFinding>() != null)
        {
            GetComponent<CheckPathFinding>().end = GameController.Main.CommandController.wayPoint;
        }

    }
    private void Awake()
    {
        entity = GetComponent<RC_Entity>();
    }

    private void Update()
    {
        if (entity.movementMode == RC_Entity.MovementMode.IDLE && commandGroup.path.Count == 1)
        {
            // if idle, should be done path finding, so remove self from cg
            if (commandGroup != null)
            {
                commandGroup.unitList.Remove(this);
                commandGroup = null;
            }
        }
        else
        {
            entity.RotateTowards(entity.movementDirection);
        }
    }

    public override void OnSelect()
    {
        // neccessary?

    }
    public override void OnDeselect()
    {

    }


    public override void PerformTaskOn(Selectable selectable)
    {

        var target = selectable.GetComponent<ResourceNode>();
        if (target != null && !target.taken)
        {
            Debug.Log("Resource Found");
            entity.TargetVector = target.transform.position;
            target.unit = this;
            target.taken = true;
            ResourceNode = target;
        }
        else
        {
            
            var home = selectable.GetComponent<Stronghold>();
            Debug.Log(home);
            if (home != null)
            {
                Debug.Log("PLZ WORK");
                entity.HomeVector = home.transform.position;
                Stronghold = home;
            }
        }
        if (Stronghold != null && ResourceNode != null)
        {
            if (entity.count == entity.maxHold)
            {
                MoveTo(Stronghold.transform.position);
            }
            else
            {
                MoveTo(ResourceNode.transform.position);
            }
        }
    }

    public void MoveToTask()
    {
        entity.retrievingPath = true;
        //entity.pathingTask = GameController.Main.PathFinder.FindPath(transform.position, position, entity.stepHeight, false);
        //entity.movementMode = RC_Entity.MovementMode.COLLECT_RESOURCE;
        entity.pathingTask = SetPath(entity.TargetVector);
    }

    public override void MoveTo(Vector3 position)
    {
        entity.retrievingPath = true;
        //entity.pathingTask = GameController.Main.PathFinder.FindPath(transform.position, position, entity.stepHeight, false);
        entity.movementMode = RC_Entity.MovementMode.FOLLOW_PATH;
        entity.pathingTask = SetPath(position);
    }

    public override void MoveToTarget(Vector3 pos)
    {
        //if (commandGroup != null)

        entity.movementMode = RC_Entity.MovementMode.FOLLOW_TARGET;
        entity.targetPos = pos;
        entity.AvoidEntityRadius = commandGroup.AvoidEntityRadius;


        entity.AvoidWallRadius = commandGroup.AvoidWallRadius;

        entity.stepHeight = commandGroup.stepHeight;

        entity.movementSpeed = commandGroup.movementSpeed;

        entity.stopFollowDistance = commandGroup.stopFollowDistance;

        entity.avoidStrength = commandGroup.avoidStrength;
        entity.followStrength = commandGroup.followStrength;

    }

    public Task<List<Vector3>> SetPath(Vector3 position)
    {
        if (commandGroup != null)
        {
            return GameController.Main.PathFinder.FindPath(commandGroup.centerVector, position, entity.stepHeight, false);
        }
        else
        {
            return GameController.Main.PathFinder.FindPath(transform.position, position, entity.stepHeight, false);
        }
    }

    public void SetTarget(Damageable target, bool pathfind = false)
    {
        if (!Visible(target.GetComponent<Selectable>()))
        {
            if (!pathfind)
            {
                Debug.Log("Cannot see target");
                return;
            }
            else
            {
                MoveTo(target.transform.position);
            }
        }
        this.target = target;
        entity.targetPos = target.transform.position;
        entity.idle = false;
        entity.perch = false;
        isPerformingTask = true;
    }

    /// <summary>
    /// Returns the closest enemy the unit can see. Returns null if no enemies are found
    /// </summary>
    public Damageable EnemyDetection()
    {
        Damageable damageable = null;

        foreach (Selectable selectable in GameController.Main.SelectionController.masterSelect)
        {
            // No friendly fire
            if (GetComponent<Selectable>().faction ^ selectable.faction) // !^ = Not XOR
            {


                //Don't bother fighting things that cannot be damaged
                var temp = selectable.GetComponent<Damageable>();
                if (temp == null || temp.Invulnerable)
                {
                    continue;
                }

                if (Visible(selectable))
                {
                    if (damageable == null || Vector3.Distance(temp.transform.position, transform.position) < Vector3.Distance(damageable.transform.position, transform.position))
                    {
                        damageable = temp;

                    }
                }
            }

        }
        return damageable;
    }

    public bool Visible(Selectable selectable)
    {
        // if within square bounds
        if (Helpers.InSquareRadius(searchRadius, transform.position.ToVector2(), selectable.transform.position.ToVector2()))
        {
            Ray ray;
            // need the vector up or else only works in postive quadrant
            ray = new Ray(transform.position + Vector3.up, selectable.transform.position - transform.position);
            RaycastHit hitData;
            // if within line of sight
            // , LayerMask.NameToLayer(layerMask)
            if (Physics.Raycast(ray, out hitData, searchRadius, layerMask) && (hitData.transform == selectable.transform)) // <- will this be problematic?
            {
                //Debug.Log(hitData.transform.name + " spotted by " + name);
                return true;
            }
        }

        return false;
    }
   
    public override void ClearTarget()
    {
        target = null;
    }
}
