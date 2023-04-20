using UnityEngine;

public class EnemyProd : ProductionStructure
{
    // Start is called before the first frame update

    public GameObject RallyPoint;
    new void Start()
    {
        base.Start();
        originialTime = 15;
        Vector3 offset = new Vector3(6, 0, -6);
       // rallyPoint = spawnPoint.transform.position + offset;
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            timeLeft = originialTime;
            SpawnUnits(base.unitToSpawn);
            unitsQueued--;
        }
    }

    public override void SpawnUnits(GameObject unit)
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
