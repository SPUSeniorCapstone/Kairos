using UnityEngine;

[RequireComponent(typeof(Selectable), typeof(Damageable))]
public class Unit : MonoBehaviour
{
    public CommandGroup command;
    public float searchRadius = 15f;
    public bool isPerformingTask = false;
    public LayerMask layerMask;
    // best way to do this?
    private Selectable goal;

    virtual public void PerformTaskOn(Selectable selectable)
    {

    }

    virtual public void MoveTo(Vector3 position)
    {

    }
    virtual public void AttackCommand()
    {

    }
    virtual public void ClearTarget()
    {

    }
    public virtual void OnSelect()
    {

    }
    public virtual void OnDeselect()
    {

    }
    public void OnDestroy()
    {
        if (command != null)
        {
            command.unitList.Remove(this);
        }
        // calls master destroy
        if (GameController.Main != null)
            GameController.Main.MasterDestory(gameObject);
    }

    public void Start()
    {
        var faction = GetComponent<Selectable>();
        // faction is enemy or player?
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
