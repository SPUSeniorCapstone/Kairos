using UnityEngine;

public class ProductionStructure : Structure
{

    public int unitsQueued;
    protected float timeLeft = 5;
    protected float originialTime = 5;
    public GameObject unitToSpawn;
    public GameObject spawnPoint;
    public Vector3 rallyPoint;


    private void Update()
    {
        if (unitsQueued > 0)
        {
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
        GameController.Main.StructureController.selected = this;
        GameController.Main.UIController.MenuController.structureMenuUI.SetActive(true);
    }

    //Add a unit to queue if train units button is clicked
    public void QueueUnits()
    {
        unitsQueued++;
    }

    //Spawn unit function
    public void SpawnUnits()
    {
        GameObject tree = Instantiate(unitToSpawn, spawnPoint.transform.position, Quaternion.identity);
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
