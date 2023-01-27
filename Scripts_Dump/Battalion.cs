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

    public Vector3 testNormal = Vector3.right;

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
        center = GetCenter();
        testNormal = (point - center).normalized;


        Debug.Log("Battlaion move called");
        center.Set(avgX, avgY, avgZ);
        // List<Vector3> positions = new List<Vector3>();
        int count = units.Count / 2;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < count; j++)
            {
                Vector3 offset = Vector3.zero;
                //new Vector3Int((int)point.x,(int)point.y, (int)point.z);
                if (j % 2 == 0)
                {
                    offset.x += j / 2;//  * 2;
                }
                else
                {
                    offset.x -= (j / 2) + 1;//  * 2;
                }
                if(i % 2 == 0)
                {
                    offset.z += i ;//  * 1.5f;
                }
                else
                {
                    //Debug.Log(units[j + i * (count)] + "");
                    offset.z -= i + 1;// * 1.5f;
                }

                float sign = (Vector3.forward.z > testNormal.z && testNormal.x < 0 ) ? -1.0f : 1.0f;
                Vector3 newPos = point + Quaternion.Euler(0, Vector3.Angle(Vector3.forward, testNormal) * sign, 0) * offset;



                units[j+i * (count)].MoveAsync(newPos);
            }
            
        }
    }



    // Start is called before the first frame update
    //void Start()
    //{
       

    //}
    private void Awake()
    {
        Unit[] unitsArray = GetComponentsInChildren<Unit>();
        foreach (Unit unit in unitsArray)
        {
            units.Add(unit);
        }
        center = GetCenter();


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

    public Vector3 GetCenter()
    {
        Vector3 avg = Vector3.zero;

        // all of this to get all units in scene
        foreach (Unit unit in units)
        {
            avg.x += unit.transform.position.x;
            avg.y += unit.transform.position.y;
            avg.z += unit.transform.position.z;
        }
        //GameController.main.playerController.playerEntities.Add(gameObject);
        avg /= units.Count;

        return avg; 
    }
}
