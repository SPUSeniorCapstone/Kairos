using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Selectable), typeof(Damageable))]
public class Unit : MonoBehaviour
{
    public CommandGroup commandGroup;
    public float searchRadius = 15f;
    public bool isPerformingTask = false;
    public LayerMask layerMask;
    // best way to do this?
    protected Selectable mySelectable;

    public AudioSource Head;
    public AudioSource Body;
    public AudioSource Feet;
    public bool badGuy = false;
 
    public bool doubleClicked = false;

    virtual public void PerformTaskOn(Selectable selectable)
    {

    }

    virtual public void MoveTo(Vector3 position)
    {

    }
    virtual public void MoveToTarget(Vector3 pos)
    {

    }
    virtual public void AttackCommand()
    {

    }
    virtual public void ClearTarget()
    {

    }
    public virtual void OnSelect()
    {
        if (doubleClicked)
        {
            doubleClicked = false;
            DoubleClick(GetType());
        }
    }
    public virtual void OnDeselect()
    {

    }
    public void DoubleClick(Type t)
    {
        UnityEngine.Object[] temp = FindObjectsByType(t, FindObjectsSortMode.None);
        //Debug.Log(temp.Count());
        foreach (var obj in temp)
        {
            var selectable = obj.GetComponent<Selectable>();
            selectable.massSelected = true;
            //!GameController.Main.SelectionController.currentlySelect.Contains(selectable)
            if (!selectable.selected)
            {
                GameController.Main.SelectionController.currentlySelect.Add(selectable);
            }  
            selectable.selected = true;
            selectable.Activate();
        }
    }
    public void OnDestroy()
    {
        if (commandGroup != null)
        {
            commandGroup.unitList.Remove(this);
        }
        // calls master destroy
        if (GameController.Main != null)
            GameController.Main.MasterDestory(gameObject);
    }

    public void Start()
    {
        var faction = GetComponent<Selectable>();
        mySelectable = GetComponent<Selectable>();
        // faction is enemy or player?
        if (faction.faction)
        {
            layerMask = LayerMask.GetMask("Terrain", "Player");
        }
        else
        {
            layerMask = LayerMask.GetMask("Terrain", "Enemy");
        }

    }
}
