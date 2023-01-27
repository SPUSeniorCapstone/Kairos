using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureController : MonoBehaviour
{
    private int unitsQueued;
    private float timeLeft = 5;
    private float originialTime = 5;
    public GameObject unitToSpawn;
    public GameObject spawnPoint;


    private void Update()
    {
        if (unitsQueued > 0) {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                timeLeft = originialTime;
                SpawnUnits();
                unitsQueued--;
            }
        }
    }

    //Function for when stronghold is clicked activate structureMenuUI
    public void OnMouseDown()
    {
        GetComponent<MenuController>().structureMenuUI.SetActive(true);
    }

    //Add a unit to queue if train units button is clicked
    public void QueueUnits()
    {
        unitsQueued++;
    }

    //Spawn unit function
    public void SpawnUnits()
    {
        Instantiate(unitToSpawn, spawnPoint.transform.position, Quaternion.identity);
    }
}