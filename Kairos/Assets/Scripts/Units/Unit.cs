using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Selectable), typeof(Damageable))]
public class Unit : MonoBehaviour, ICommandable
{
    public float searchRadius = 15f;
    public bool isPerformingTask = false;


    virtual public void PerformTaskOn(Selectable selectable)
    {

    }

    virtual public void MoveTo(Vector3 position)
    {

    }
}
