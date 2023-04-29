using UnityEngine;
using System.Collections.Generic;
// delete this later
using System.Text.RegularExpressions;
using Unity.VisualScripting;

public class EnemyProd : ProductionStructure
{
    // Start is called before the first frame update

    public GameObject RallyPoint;


    [SerializeField] List<GameObject> guard = new List<GameObject>();
    public int guardNumber = 4;

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
        if (GameController.Main.CorruptionController.SpawnUnits)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                int rand = Random.Range(0, 2);
                timeLeft = originialTime;
                SpawnUnits(base.unitTypes[rand]);
                unitsQueued--;
            }
        }

    }

    public override void SpawnUnits(GameObject unit)
    {
        GameObject tree = Instantiate(unit, spawnPoint.transform.position, Quaternion.identity, GameController.Main.StructureController.EnemyUnits.transform);
        // does this work?
        if ( guard.Count < guardNumber)
        {
            if (tree.GetComponent<Infantry_Unit>() != null)
            {
                tree.GetComponent<Infantry_Unit>().MoveTo(RallyPoint.transform.position);
                guard.Add(tree);
            }
        }
        else
        {
            // something like this
            //tree.GetComponent<Unit>().MoveTo(GameObject.Find(Regex.Match(GameObject.Find("Stronghold").name, "Stronghold").Value).transform.position);
            //Debug.Log(Regex.Match(GameObject.Find("Stronghold").name, "Stronghold").Value);
            tree.GetComponent<Unit>().MoveTo(GameObject.Find("StrongHold(Clone)").transform.position);
        }
    }
}
