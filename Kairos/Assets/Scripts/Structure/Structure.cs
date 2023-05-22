
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
 
    public float healTime, originalHeal = 1;

    public float buildDistance = 6;
     
    public Damageable damaging; 


    protected void Start()
    {
        if (!example)
        {
            damaging = GetComponent<Damageable>();
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

    private void Update()
    {
        if (damaging != null && damaging.MaxHealth > damaging.Health)
        {
            RepairStructure();
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

    public virtual void RepairStructure()
    {
        Debug.Log("Max Health is greather than current Health: " + damaging.Health);
        foreach (Builder_Unit b in GameController.Main.masterBuilder)
        {
            if (builder == null)
            {
                builder = b;
            }
            else if (Vector3.Distance(transform.position, builder.transform.position) > Vector3.Distance(transform.position, b.transform.position))
            {
                builder = b;
            
            }
        }
        if (!example && builder != null)
        {
            healTime -= Time.deltaTime;
            if (healTime <= 0 && Vector3.Distance(transform.position, builder.transform.position) < buildDistance && builder.entity.movementMode == Builder_Entity.MovementMode.IDLE)
            {
                healTime = originalHeal;
                damaging.Heal(100);
            }
            if (damaging.Health == damaging.MaxHealth)
            {
                builder = null;

            }
        }
    }
}
