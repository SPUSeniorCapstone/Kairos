using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

[RequireComponent(typeof(Infantry_Entity))]
public class Infantry_Unit : Unit
{
    public float personalDistance = 1;
    public float attackDistance = 1;
    public float attackCoolDown = 1;
    public float attackDamage = 10f;
    public bool autoAttack = false;
    private float lastAttackTime = 0;
    public bool archer = false;
    public bool guard = false;
    public projectileArrow projectile;
    public float projectileSpeed;

    public Damageable target;

    [SerializeField] List<AudioClip> HeadSounds = new List<AudioClip>();
    [SerializeField] List<AudioClip> BodySounds = new List<AudioClip>();
    [SerializeField] List<AudioClip> FeetSounds = new List<AudioClip>();

    // i want to move this to base unit class
    //public CommandGroup command;

    Infantry_Entity entity;

    private void Start()
    {
        base.Start();
        if (GetComponent<CheckPathFinding>() != null)
        {
            GetComponent<CheckPathFinding>().end = GameController.Main.CommandController.wayPoint;
        }

    }
    private void Awake()
    {
        entity = GetComponent<Infantry_Entity>();
    }

    private void Update()
    {
        if (GetComponent<Damageable>().Dead)
        {
            return;
        }
        // will this work if target is "missing"?
        if (target == null && entity.movementMode == Infantry_Entity.MovementMode.ATTACK_FOLLOW)
        {
            entity.movementMode = Infantry_Entity.MovementMode.IDLE;
        }
        if (commandGroup != null && commandGroup.path.Count == 1)
        {
            Debug.Log("I SHOULD BE GONE");
            commandGroup.unitList.Remove(this);
            commandGroup = null;
            entity.movementMode = Infantry_Entity.MovementMode.IDLE;
        }
        //if (entity.movementMode == Infantry_Entity.MovementMode.IDLE)
        //{
        //    // if idle, should be done path finding, so remove self from cg
            
        //}

        if (autoAttack)
        {
            // passive auto attack
            var enemy = EnemyDetection();
            if (enemy != null)
            {
                if (badGuy)
                {
                    Debug.Log("enemy detects opponent");
                }
                

                if (autoAttack && (entity.movementMode == Infantry_Entity.MovementMode.IDLE || badGuy))
                {
                    if (!badGuy)
                    {
                        Debug.Log("HELPELPELPELPELPLE");
                    }
                    SetTarget(enemy);
                    entity.movementMode = Infantry_Entity.MovementMode.ATTACK_FOLLOW;
                }

            } 
            else if (target == null && badGuy && !guard && entity.movementMode == Infantry_Entity.MovementMode.IDLE)
            {
                if (GameController.Main.StructureController.PlayerStructures.GetComponentInChildren<Stronghold>() != null)
                {
                    MoveTo(GameController.Main.StructureController.PlayerStructures.GetComponentInChildren<Stronghold>().transform.position);
                }
                else if (GameController.Main.StructureController.PlayerUnits.GetComponentInChildren<Builder_Unit>())
                {
                    MoveTo(GameController.Main.StructureController.PlayerUnits.GetComponentInChildren<Builder_Unit>().transform.position);
                }
            }
        }

        // is this neccesary? why not update all the time?
        if (entity.movementMode == Infantry_Entity.MovementMode.ATTACK_FOLLOW && target != null && Visible(target.GetComponent<Selectable>()) && autoAttack)
        {
            entity.targetPos = target.transform.position;
            //when within attack range
            // bootleg combat
            // && Vector3.Distance(transform.position, target.transform.position) < attackDistance
            if ((entity.movementDirection == Vector3.zero && archer || !archer))
            {
                // neccessary ?
                if (target != null)
                {
                    Debug.Log("Should be rotating towards");
                    //entity.RotateTowards(target.transform.position.normalized);
                    transform.LookAt(target.transform);
                    //transform.rotation = Quaternion.LookRotation(target.transform.position);
                    if (Time.time - lastAttackTime > attackCoolDown)
                    {
                        //Body.clip = BodySounds.ElementAt(10);
                        //Body.Play();
                        if (GameController.Main.randomDamageModifier)
                        {
                            float rand = Random.Range(1f, 2f);
                            target.Damage(attackDamage * rand);
                        }
                        else
                        {
                            if (archer)
                            {
                                Vector3 offset = new Vector3(0, 2, 0);
                                Instantiate(projectile.gameObject, transform, false);
                                projectile.transform.position = offset;
                                projectile.speed = projectileSpeed;
                                projectile.target = target.gameObject;
                            }
                            target.Damage(attackDamage);
                        }

                        lastAttackTime = Time.time;

                        // neccessary? doesn't work all the time (race condition)
                        if (target.Health <= 0)
                        {
                            entity.movementMode = Infantry_Entity.MovementMode.IDLE;
                            target = null;
                        }
                    }
                }
            }
        }
        // these rotate towards are fighting each other, causing jittering. need exclusive
       else if (entity.movementMode != Infantry_Entity.MovementMode.IDLE)
        {
            entity.RotateTowards(entity.movementDirection);
        }     
    }

    public override void PerformTaskOn(Selectable selectable)
    {
        var tar = selectable.GetComponent<Damageable>();
        if (tar != null)
        {
            SetTarget(tar, true);
        }
        else
        {
            Debug.Log("TAR IS NULL! => " + selectable);
        }
    }

    public override void MoveTo(Vector3 position)
    {
        entity.retrievingPath = true;
        //entity.pathingTask = GameController.Main.PathFinder.FindPath(transform.position, position, entity.stepHeight, false);
        entity.movementMode = Infantry_Entity.MovementMode.FOLLOW_PATH;
 
        entity.pathingTask = SetPath(position);
    }

    public override void MoveToTarget(Vector3 pos)
    {
        //if (commandGroup != null)
        
            entity.movementMode = Infantry_Entity.MovementMode.FOLLOW_TARGET;
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
                    Debug.Log("Visiible");
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
            Debug.Log("in the square");
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
    public override void AttackCommand()
    {
        Debug.Log("attack command");
        GetComponent<Infantry_Entity>().movementMode = Infantry_Entity.MovementMode.ATTACK_FOLLOW;
    }
    public override void ClearTarget()
    {
        //Debug.Log("CLEAR");
        target = null;
    }
}
