using System.Collections.Generic;
using UnityEngine;

public class ProductionStructure : Structure
{

    public int unitsQueued;
    protected float timeLeft = 5;
    protected float originialTime = 5;
    public GameObject unitToSpawn;
    public List<GameObject> unitTypes;
    public GameObject spawnPoint;
    public Vector3 rallyPoint;
    public Queue<GameObject> buildQue;


    private void Update()
    {
        if (unitsQueued > 0)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                timeLeft = originialTime;
                SpawnUnits(buildQue.Dequeue());
                unitsQueued--;
            }
        }
    }
    public void Start()
    {
        base.Start();
        buildQue = new Queue<GameObject>();
    }

    //Function for when stronghold is clicked activate structureMenuUI
    public void OnMouseDown()
    {
        GameController.Main.StructureController.selected = this;
        GameController.Main.UIController.MenuController.structureMenuUI.SetActive(true);
    }

    //Add a unit to queue if train units button is clicked
    public void QueueUnits(GameObject unit)
    {
        if (buildQue == null)
        {
            Debug.Log("WHY NULL?!");
        }
        buildQue.Enqueue(unit);
        unitsQueued++;
    }

    //Spawn unit function
    public void SpawnUnits(GameObject unit)
    {
        GameObject tree = Instantiate(unit, spawnPoint.transform.position, Quaternion.identity);
        // does this work?
        if (rallyPoint != null && rallyPoint != Vector3.zero)
        {
            if (tree.GetComponent<Infantry_Unit>() != null)
            {
                tree.GetComponent<Infantry_Unit>().MoveTo(rallyPoint);
            }
        }
    }
}
