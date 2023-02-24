using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProd : ProductionStructure
{
    // Start is called before the first frame update
    void Start()
    {
        originialTime = 12;
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
