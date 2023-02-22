using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, ICommandable
{
    public Selectable target; 

    public float squareSide = 15f;
    public bool isAttacking;

    Entity entity;

    private void Start()
    {
        entity = GetComponent<Entity>();
    }

    private void Update()
    {
        if(target == null)
        {
            //EnemyDetection();
        }
        else
        {
            if(entity != null)
            {
                entity.targetPos = target.transform.position;
            }
        }
    }

    public void EnemyDetection()
    {
        foreach (Selectable selectable in GameController.Main.SelectionController.masterSelect)
        {
            if (GetComponent<Selectable>().isEnemy !^ selectable.isEnemy) // !^ = Not XOR
            {
                continue;
            }
            // if within square bounds
            if (Mathf.Abs(transform.position.x - selectable.transform.position.x) <= squareSide && Mathf.Abs(transform.position.z - selectable.transform.position.z) <= squareSide)
            {
                Ray ray;
                ray = new Ray(transform.position, selectable.transform.position - transform.position);
                RaycastHit hitData;
                // if within line of sight
                if (Physics.Raycast(ray, out hitData, squareSide) && (hitData.transform.GetComponent<Selectable>())) // <- will this be problematic?
                {
                    // could place "if on aggressive mode" here
                    if (target != null)
                    {
                        // if has target object but there is a closer object
                        if (isAttacking && Vector3.Distance(transform.position, entity.targetObject.transform.position) > Vector3.Distance(transform.position, selectable.transform.position))
                        {
                            SetTarget(selectable);
                        } 
                    }
                    else
                    {
                        SetTarget(selectable);
                    }
                }
            }
        }
    }

    public void SetTarget(Selectable target)
    {
        isAttacking = true;
        Entity entity = GetComponent<Entity>();
        if (entity != null)
        {
            entity.pathindex = 0;
            CommandGroup old = entity.CommandGroup;
            if (old != null)
            {
                old.entities.Remove(entity);
            }
            //entity.CommandGroup.entities.Remove(entity);

            //entity.idle = false;
        }
        entity.targetObject = target.gameObject;
        entity.targetPos = entity.targetObject.transform.position;
        entity.targetHealth = target.GetComponent<Damageable>().Health;
        entity.idle = false;
        entity.perch = false;

    }
    public void Attack()
    {

    }
}
