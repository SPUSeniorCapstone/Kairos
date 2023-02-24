using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Infantry_Entity))]
public class Infantry_Unit : Unit
{
    public float personalDistance = 1;
    public float attackDistance = 1;
    public float attackCoolDown = 1;

    private float lastAttackTime = 0;

    public Damageable target;

    public CommandGroup command;

    Infantry_Entity entity;

    private void Start()
    {
        //base.Start();
        entity = GetComponent<Infantry_Entity>();
    }

    private void Update()
    {
        if(entity.movementMode == Infantry_Entity.MovementMode.IDLE)
        {
            var enemy = EnemyDetection();
            if(enemy != null)
            {
                SetTarget(enemy);
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
        entity.pathingTask = GameController.Main.PathFinder.FindPath(transform.position, position, entity.stepHeight, false);
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
            if (GetComponent<Selectable>().faction! ^ selectable.faction) // !^ = Not XOR
            {
                continue;
            }

            //Don't bother fighting things that cannot be damaged
            var temp = selectable.GetComponent<Damageable>();
            if (temp == null || temp.Invulnerable)
            {
                continue;
            }

            if (Visible(selectable))
            {
                if (Vector3.Distance(temp.transform.position, transform.position) < Vector3.Distance(damageable.transform.position, transform.position))
                {
                    damageable = temp;
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
            ray = new Ray(transform.position, selectable.transform.position - transform.position);
            RaycastHit hitData;
            // if within line of sight
            if (Physics.Raycast(ray, out hitData, searchRadius) && (hitData.transform == selectable.transform)) // <- will this be problematic?
            {
                return true;
            }
        }

        return false;
    }
}
