using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Battalion : MonoBehaviour
{
    public List<Unit> units = new List<Unit>();
    [SerializeField] PlayerController playerController;

    public void OnSelect()
    {
        foreach (Unit unit in units)
        {
            unit.SetSelectedVisible(true);
            playerController.selectedEntityList.Add(unit);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        // all of this to get all units in scene
        var tree = GameObject.FindGameObjectsWithTag("Unit");
        foreach (var vre in tree)
        {
            if (vre.GetComponent<Unit>() != null)
            {
                units.Add(vre.GetComponent<Unit>());
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
