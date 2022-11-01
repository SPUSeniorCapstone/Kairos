using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Battalion : MonoBehaviour
{
    public List<Unit> units = new List<Unit>();
    public bool selected = false;
    [SerializeField] PlayerController playerController;

    public void Select()
    {
        selected = true;
        foreach (Unit unit in units)
        {
            if (!unit.select)
            unit.SetSelectedVisible(true);
            playerController.selectedEntityList.Add(unit);
        }
    }
    // worth making a function for? versus direct access to public bool?
    public void Deselect()
    {
        selected = false;
        foreach (Unit unit in units)
        {
            unit.SetSelectedVisible(false);
            //playerController.selectedEntityList.Add(unit);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        // all of this to get all units in scene
        Unit[] unitsArray = GetComponentsInChildren<Unit>();
        foreach (Unit unit in unitsArray)
        {  
                units.Add(unit);              
        }
        //GameController.main.playerController.playerEntities.Add(gameObject);

    }
    private void OnDestroy()
    {
        foreach (Unit unit in units)
        {

            GameController.main.playerController.playerEntities.Remove(unit.gameObject);
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
