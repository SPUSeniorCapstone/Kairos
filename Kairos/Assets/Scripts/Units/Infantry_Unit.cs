using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Infantry_Entity))]
public class Infantry_Unit : Unit
{
    public float personalDistance = 1;
    public float attackDistance = 1;
    public float attackCoolDown = 1;
    public float attackDamage = 10f;
    public bool autoAttack = false;
    private float lastAttackTime = 0;



    public Damageable target;

    // i want to move this to base unit class
    //public CommandGroup command;

    Infantry_Entity entity;

    private void Start()
    {
        base.Start();
        entity = GetComponent<Infantry_Entity>();
    }

    private void Update()
    {
        // will this work if target is "missing"?
        if (target == null && entity.movementMode == Infantry_Entity.MovementMode.ATTACK_FOLLOW)
        {
            entity.movementMode = Infantry_Entity.MovementMode.IDLE;
        }
        if(entity.movementMode == Infantry_Entity.MovementMode.IDLE)
        {
            // if idle, should be done path finding, so remove self from cg
            if (command != null)
            {
                command.unitList.Remove(this);
                command = null;
            }

            // passive auto attack
            var enemy = EnemyDetection();
            if(enemy != null)
            {
                SetTarget(enemy);
                if (autoAttack)
                entity.movementMode = Infantry_Entity.MovementMode.ATTACK_FOLLOW;
            }
        }
        // is this neccesary? why not update all the time?
        if(entity.movementMode == Infantry_Entity.MovementMode.ATTACK_FOLLOW && target != null && autoAttack)
        {
            entity.targetPos = target.transform.position;
            //when within attack range
            // bootleg combat
            if (entity.movementDirection == Vector3.zero)
            {
                // neccessary ?
                if (target != null)
                {
                    if (Time.time - lastAttackTime > attackCoolDown)
                    {
                        target.Damage(attackDamage);
                        lastAttackTime = Time.time;

                        // neccessary? doesn't work all the time (race condition)
                        if (target.Health <= 0)
                        {
                            target = null;
                            entity.movementMode = Infantry_Entity.MovementMode.IDLE;
                        }
                    }
                }
            }
        }
    }

    public override void PerformTaskOn(Selectable selectable)
    {
        var tar = selectable.GetComponent<Damageable>();
        if (tar != null)
        {
            SetTarget(tar, true);
        }
    }

    public override void MoveTo(Vector3 position)
    {
        entity.retrievingPath = true;
        //entity.pathingTask = GameController.Main.PathFinder.FindPath(transform.position, position, entity.stepHeight, false);
        entity.pathingTask = SetPath(position);
    }
    
    public Task<List<Vector3>> SetPath(Vector3 position)
    {
        if (command != null)
        {
            return GameController.Main.PathFinder.FindPath(command.centerVector, position, entity.stepHeight, false);
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
                Debug.Log(hitData.transform.name + " spotted by " + name);
                return true;
            }
        }

        return false;
    }
}
