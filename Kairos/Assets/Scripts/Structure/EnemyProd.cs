using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProd : ProductionStructure
{
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        originialTime = 12;
        Vector3 offset = new Vector3 (3, 0, 3);
        rallyPoint = spawnPoint.transform.position + offset;
    }

    // Update is called once per frame
    void Update()
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
