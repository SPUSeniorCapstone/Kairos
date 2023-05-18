using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProductionStructure : Structure
{

    public int unitsQueued;
    protected float timeLeft = 5;
    protected float originialTime = 5;
    public GameObject unitToSpawn;
    public List<GameObject> unitTypes;
    public GameObject spawnPoint;
    public Queue<GameObject> buildQue;

    public int infantryCount;
    public int archerCount;
    public int builderCount;
    public int rcCount;

    //public struct QueItem(GameObject unit, count)



    private void Update()
    {
        if (unitsQueued > 0)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                timeLeft = originialTime;
                UpdateCount(buildQue.Peek(), 0);
                SpawnUnits(buildQue.Dequeue());
                unitsQueued--;
            }
        }
    }
    new public void Start()
    {
        base.Start();
        buildQue = new Queue<GameObject>();
        if (Preview != null)
        {
            Destroy(Preview);
        }
    }

    //Function for when stronghold is clicked activate structureMenuUI
    public void OnMouseDown()
    {
        //if (GameController.Main.SelectionController.currentlySelect[0].gameObject == this)
        //{
        //    GameController.Main.StructureController.selected = this;
        //    GameController.Main.UIController.MenuController.structureMenuUI.SetActive(true);
        //}
    }

    //Add a unit to queue if train units button is clicked
    public void QueueUnits(GameObject unit)
    {
        if (buildQue == null)
        {
            Debug.Log("WHY NULL?!");
        }
        unitsQueued++;
        UpdateCount(unit, 1);
        buildQue.Enqueue(unit);
    }

    public void DequeueUnits(GameObject unit)
    {
        if (buildQue.Count != 0)
        {
            timeLeft = originialTime;
            List<GameObject> see = buildQue.ToList<GameObject>();
            see.Remove(unit);
            var queue = new Queue<GameObject>(see);
            unitsQueued--;
            UpdateCount(unit, 1);
            buildQue = queue;
        }
    }

    //Spawn unit function
    public virtual void SpawnUnits(GameObject unit)
    {
        GameObject tree = Instantiate(unit, spawnPoint.transform.position, Quaternion.identity, GameController.Main.StructureController.PlayerUnits.transform);
        // does this work?
        if (rallyPoint.activeSelf)
        {
            if (tree.GetComponent<Unit>() != null)
            {
                tree.GetComponent<Unit>().MoveTo(rallyPoint.transform.position);
            }
        }
    }
    public override void OnSelect()
    {
        rallyPoint.GetComponentInChildren<MeshRenderer>().material.shader = GameController.Main.highlight;
        if (!GameController.Main.StructureController.selected.Contains(this))
        {
            GameController.Main.StructureController.selected.Add(this);
        }
    }
    public override void OnDeselect()
    {
        rallyPoint.GetComponentInChildren<MeshRenderer>().material.shader = GameController.Main.unHighlight;
        GameController.Main.StructureController.selected.Remove(this);
    }
    public void UpdateCount(GameObject unit, int index)
    {
        if (unit == GameController.Main.StructureController.archer)
        {
            if (index == 0)
            {
                archerCount--;
            }
            
                string sub = GameController.Main.UIController.gameUI.ProductionMenu.archerInfo.text;
                sub = sub.Substring(0, sub.IndexOf("("));
                sub += "(" + archerCount + ")";
                GameController.Main.UIController.gameUI.ProductionMenu.archerInfo.text = sub;
            
        }
        else if (unit == GameController.Main.StructureController.infantry)
        {
            if (index == 0)
            {
                infantryCount--;
            }
            
                string sub = GameController.Main.UIController.gameUI.ProductionMenu.infantryInfo.text;
                sub = sub.Substring(0, sub.IndexOf("("));
                sub += "(" + infantryCount + ")";
                GameController.Main.UIController.gameUI.ProductionMenu.infantryInfo.text = sub;
            
        }
        else if (unit == GameController.Main.StructureController.builder)
        {
            if (index == 0)
            {
                builderCount--;
            }
            
                string sub = GameController.Main.UIController.gameUI.ProductionMenu.builderInfo.text;
                sub = sub.Substring(0, sub.IndexOf("("));
                sub += "(" + builderCount + ")";
                GameController.Main.UIController.gameUI.ProductionMenu.builderInfo.text = sub;
            
        }
        else if (unit == GameController.Main.StructureController.resourceCollector)
        {
            if (index == 0)
            {
                rcCount--;
            }
            
                string sub = GameController.Main.UIController.gameUI.ProductionMenu.collectorInfo.text;
                sub = sub.Substring(0, sub.IndexOf("("));
                sub += "(" + rcCount + ")";
                GameController.Main.UIController.gameUI.ProductionMenu.collectorInfo.text = sub;
            
        }
        else
        {
            Debug.Log("This shouldn't happen");
        }
    }

    public void SetProdUI()
    {
        string sub = GameController.Main.UIController.gameUI.ProductionMenu.archerInfo.text;
        sub = sub.Substring(0, sub.IndexOf("("));
        sub += "(" + archerCount + ")";
        GameController.Main.UIController.gameUI.ProductionMenu.archerInfo.text = sub;



        sub = GameController.Main.UIController.gameUI.ProductionMenu.infantryInfo.text;
        sub = sub.Substring(0, sub.IndexOf("("));
        sub += "(" + infantryCount + ")";
        GameController.Main.UIController.gameUI.ProductionMenu.infantryInfo.text = sub;



        sub = GameController.Main.UIController.gameUI.ProductionMenu.builderInfo.text;
        sub = sub.Substring(0, sub.IndexOf("("));
        sub += "(" + builderCount + ")";
        GameController.Main.UIController.gameUI.ProductionMenu.builderInfo.text = sub;



        sub = GameController.Main.UIController.gameUI.ProductionMenu.collectorInfo.text;
        sub = sub.Substring(0, sub.IndexOf("("));
        sub += "(" + rcCount + ")";
        GameController.Main.UIController.gameUI.ProductionMenu.collectorInfo.text = sub;

    }
}