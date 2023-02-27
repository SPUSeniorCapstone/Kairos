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
        // last two conditions prevent checking again after game end
        if(GameController.Main != null && GameController.Main.StructureController != null && GameController.Main.won == false && GameController.Main.lost == false)
        {
            GameController.Main.StructureController.masterStructure.Remove(this);
            GameController.Main.CheckVictory(this);
        }
    }
}
