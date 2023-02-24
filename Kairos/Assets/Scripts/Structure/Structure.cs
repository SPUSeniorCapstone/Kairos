using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Structure : MonoBehaviour
{
    public Vector3Int Size = Vector3Int.one;

    protected void Start()
    {
        GameController.Main.StructureController.masterStructure.Add(this);
        if (GetComponent<Selectable>().faction)
        {
            GameController.Main.enemyCount++;
        }
        else
        {
            GameController.Main.playerCount++;
        }
    }
    private void OnDestroy()
    {
        GameController.Main.StructureController.masterStructure.Remove(this);
        GameController.Main.CheckVictory();
    }
}
