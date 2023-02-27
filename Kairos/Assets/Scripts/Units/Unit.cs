using UnityEngine;

[RequireComponent(typeof(Selectable), typeof(Damageable))]
public class Unit : MonoBehaviour, ICommandable
{
    public CommandGroup command;
    public float searchRadius = 15f;
    public bool isPerformingTask = false;
    public LayerMask layerMask;

    virtual public void PerformTaskOn(Selectable selectable)
    {

    }

    virtual public void MoveTo(Vector3 position)
    {

    }
    public void OnDestroy()
    {
        if (command != null)
        {
            command.unitList.Remove(this);
        }
        // will this work?
        if (GameController.Main != null)
            GameController.Main.MasterDestory(gameObject);
    }

    public void Start()
    {
        var faction = GetComponent<Selectable>();
        if (faction.faction)
        {
            layerMask = LayerMask.GetMask("Terrain", "Player");
        }
        else
        {
            layerMask = LayerMask.GetMask("Terrain", "Enemy");
        }

    }
}
