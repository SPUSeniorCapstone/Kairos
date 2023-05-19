
using UnityEngine;

//[RequireComponent(typeof(Selectable))]
public class Structure : MonoBehaviour
{
    public string structureName;

    public Vector3Int Size = Vector3Int.one;

    public GameObject Preview;

    public bool enemy = false;

    public bool example = false;

    public Builder_Unit builder;

    public GameObject directory;

    public GameObject rallyPoint;


    protected void Start()
    {
        if (!example)
        {
            GameController.Main.StructureController.masterStructure.Add(this);
            if (GetComponent<Selectable>().faction)
            {
                GameController.Main.enemyCount++;
                enemy = true;
            }
            else if(GetComponent<ProductionStructure>())
            {
                GameController.Main.playerCount++;
            }
            if (!enemy)
            {
                directory = GameController.Main.StructureController.PlayerStructures;
            }
            else
            {
                directory = GameController.Main.StructureController.EnemyStructures;
            }
        }
    }
    private void OnDestroy()
    {
        // last two conditions prevent checking again after game end
        if(GetComponent<ProductionStructure>() != null && GameController.Main != null && GameController.Main.StructureController != null && GameController.Main.won == false && GameController.Main.lost == false)
        {
            GameController.Main.StructureController.masterStructure.Remove(this);
            GameController.Main.CheckVictory(this);
            GameController.Main.MasterDestory(gameObject);         
        }
    }

    public virtual void OnSelect()
    {

    }
    public virtual void OnDeselect()
    {

    }
}
