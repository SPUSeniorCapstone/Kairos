using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Battalion : Entity
{
    public List<Unit> units = new List<Unit>();
    public bool selected = false;
    [SerializeField] PlayerController playerController;
    public Vector3 center;
    private float avgX;
    private float avgY;
    private float avgZ;
    private float xTotal;
    private float yTotal;
    private float zTotal;

    public void Select()
    {
        selected = true;
        foreach (Unit unit in units)
        {
            if (!unit.select)
            unit.SetSelectedVisible(true);
            //playerController.selectedEntityList.Add(unit);
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

    public void Move(Vector3 point)
    {
        Debug.Log("Battlaion move called");
        center.Set(avgX, avgY, avgZ);
        int unitNum = 0;
        // List<Vector3> positions = new List<Vector3>();
        int count = units.Count / 2;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < count; j++)
            {
                Vector3Int newPos = MapController.main.grid.WorldToCell(point);
                    //new Vector3Int((int)point.x,(int)point.y, (int)point.z);
                if (j % 2 == 0)
                {
                    newPos.x += j / 2;//  * 2;
                }
                else
                {
                    newPos.x -= (j / 2) + 1;//  * 2;
                }
                if(i % 2 == 0)
                {
                    newPos.y += i ;//  * 1.5f;
                }
                else
                {
                    //Debug.Log(units[j + i * (count)] + "");
                    newPos.y -= i + 1;// * 1.5f;
                }

                units[j+i * (count)].MoveAsync(MapController.main.grid.GetCellCenterWorld(newPos));
            }
            
        }
    }



    // Start is called before the first frame update
    //void Start()
    //{
       

    //}
    private void Awake()
    {
        // all of this to get all units in scene
        Unit[] unitsArray = GetComponentsInChildren<Unit>();
        foreach (Unit unit in unitsArray)
        {
            units.Add(unit);
            xTotal += unit.transform.position.x;
            yTotal += unit.transform.position.y;
            zTotal += unit.transform.position.z;
        }
        //GameController.main.playerController.playerEntities.Add(gameObject);
        avgX = xTotal / units.Count;
        avgY = yTotal / units.Count;
        avgZ = zTotal / units.Count;


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
