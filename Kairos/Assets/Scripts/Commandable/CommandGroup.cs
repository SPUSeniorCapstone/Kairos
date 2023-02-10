using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CommandGroup : MonoBehaviour
{
    public List<Entity> entities;

    public int speedLimit;
    public Vector3 max, min;
    public Vector3 centerVector => transform.position;
    public float followStr;
    public float followSpeed;
    public GameObject groupTargetObj;
    public bool selected;
    public bool lesser = true;

    public bool flock;

    public bool twoD = false;

    [Range(0, 1f)]
    public float alignmentFactor;
    [Range(0, 50f)]
    public float effectiveDistance;
    [Range(0, 1f)]
    public float cohesionFactor;
    [Range(0, 1f)]
    public float boundFactor;
    [Range(0, 100f)]
    public float distanceFromTarget;

    void Update()
    {
        CalculateCenter();
        // is there a better way?
        if (lesser)
        {
            CheckIfEmpty();
        }
        else 
        {
            foreach (CommandGroup group in GameController.Main.CommandController.CommandGroups)
            {
                if (group != this)
                {
                    group.effectiveDistance = effectiveDistance;
                    group.distanceFromTarget = distanceFromTarget;
                    group.boundFactor = boundFactor;
                    group.cohesionFactor = cohesionFactor;
                    group.alignmentFactor = alignmentFactor;
                    group.followSpeed = followSpeed;
                    group.followStr = followStr;
                }
            }
        }
    }

    public void CalculateCenter()
    {
        if (entities.Count == 0)
        {
            transform.position = Vector3.zero;
        }

        Vector3 center = Vector3.zero;
        for(int i = 0; i < entities.Count; i++)
        {
            var entity = entities[i];
            if (entity.Perching())
            {
                entity.CommandGroup = null;
                entities.RemoveAt(i);
                i--;
            }
            else
            {
                center += entity.transform.position;
            }
        }
        if(entities.Count > 0)
        {
            center /= entities.Count;
            center.y = 0;
        }
        transform.position = center;
    }
    public void SetGroupTarget(GameObject gameObject)
    {
        groupTargetObj = gameObject;
        foreach (Entity entity in entities)
        {
            entity.targetObject = groupTargetObj;
            entity.SetTargetPos();
            entity.idle = false;
        }
    }
    public void CheckIfEmpty()
    {
        if (entities.Count == 0)
        {
            GameController.Main.CommandController.CommandGroups.Remove(this);
            Destroy(gameObject);
        }
    }
}
