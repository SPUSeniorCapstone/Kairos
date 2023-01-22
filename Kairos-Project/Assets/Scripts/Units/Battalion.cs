using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Battalion : MonoBehaviour
{
    public List<Unit> units = new List<Unit>();
    public bool selected = false;
    public Vector3 center;
    private float avgX;
    private float avgY;
    private float avgZ;
    private float xTotal;
    private float yTotal;
    private float zTotal;
    public Vector3 destination = Vector3.zero;

    public Vector3 testNormal = Vector3.right;
    public bool attacking = false;
    public bool pseudoAttacking = false;
    public bool isMoving = false;

    public bool isEnemy;
    public bool isPlayer;
    public bool isNeutral;
    public bool autoAttack;
    public bool deathReel;
    public Battalion targetBattalion;

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


        //Debug.Log("Battlaion move called");
        // is this broken?
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
            isMoving = false;
        }
        //center = GetCenter();
    }
    public void Attack()
    {
        // looking for enemies to attack


        if (autoAttack && isPlayer)
        {
        
            foreach (Entity entity in GameController.main.playerController.playerEntities)
            {
               
                if (entity.isEnemy)
                {
                    if (entity.GetComponentInParent<Battalion>() != null)
                    {
                        targetBattalion = entity.GetComponentInParent<Battalion>();
                        break;
                    }
                    else
                    {
                        //targetBattalion = entity;
                        //break;
                    }

                }
            }
        }
        else if (autoAttack && isEnemy)
        {
          
            foreach (Entity entity in GameController.main.playerController.playerEntities)
            {
                // Debug.Log(entity);
                if (entity.isPlayer)
                {
                    Debug.Log("Good found");
                  if (entity.GetComponentInParent<Battalion>() != null)
                  {
                      targetBattalion = entity.GetComponentInParent<Battalion>();
                      Debug.Log("Good found");
                      break;
                  }
                  else
                  {
                      //targetBattalion = entity;
                      //Debug.Log("Good found");
                      //break;
                  }

                }
            }
        }

        if (targetBattalion != null)
        {
            attacking = true;
            if (targetBattalion != null)
            {
                Move(targetBattalion.center);
                Debug.Log("target.GetComponent");
            }
            else {
                Debug.Log("HELPME");
           // Move(targetBattalion.transform.position);
        }
        }
    }


    // Start is called before the first frame update
    //void Start()
    //{


    //}
    int count = 0;
    private void Update()
    {
        count++;
        center = GetCenter();
        //if (target != null && attacking == true)
        //{
        //    center = GetCenter();
        //    var oldCenter = target.GetComponent<Battalion>().center;
        //    if ( oldCenter != target.GetComponent<Battalion>().center)
        //    {
        //        Move(target.GetComponent<Battalion>().center);
        //    }
        //    count = 0;
        //}
        if (targetBattalion != null && attacking == true) {

            foreach (Unit unit in targetBattalion.units) {
                if (unit.isMoving)
                {
                    Move(targetBattalion.center);
                    count = 0;
                    break;
                }
            }


            if (center == targetBattalion.center)
            {
                attacking = false;
            }
        }
    }

  
    private void Start()
    {
        //base.Start();

        Unit[] unitsArray = this.GetComponentsInChildren<Unit>();
        foreach (Unit unit in unitsArray)
        {
            units.Add(unit);
        }
        center = GetCenter();
        //Move(destination);

    }
    private void OnDestroy()
    {
        foreach (Unit unit in units)
        {

            GameController.main.playerController.playerEntities.Remove(unit);
        }
       
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
