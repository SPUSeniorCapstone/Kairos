using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Damageable : MonoBehaviour
{
    [field: SerializeField] public float MaxHealth { get; private set; }
    [field: SerializeField] public float Health { get; private set; }
    public float HealthRatio { get { return Mathf.Clamp(Health / MaxHealth, 0, 1); } }
    [field: SerializeField] public bool Invulnerable { get; private set; }

    public float squareSide = 15f;
    public bool isAttacking;

    public Vector3 healthBarPosition;
    public bool Dead
    {
        get
        {
            return dead;
        }
    }
    bool dead = false;


    private void Start()
    {
        GameController.Main.UIController.CreateHealthBar(this);
    }

    private void Update()
    {
        EnemyDetection();
        if (Health <= 0)
        {
            dead = true;
            GameController.Main.MasterDestory(this.gameObject);
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Damages the entity. Pass this function the raw damage value, and it will determine how much health to subtract from the entity
    /// <para>**Currently just removes the raw damage amount from health**</para> 
    /// </summary>
    /// <param name="damage">The raw damage value</param>
    public float Damage(float damage)
    {
        if (!Invulnerable)
        {
            Health -= damage;
            if(Health < 0)
            {
                dead = true;
            }
        }
        return Health;
    }

    public void Heal(float heal)
    {
        Health += heal;
        if(Health > MaxHealth)
        {
            Health = MaxHealth;
        }
    }

    public void EnemyDetection()
    {
        foreach (Selectable selectable in GameController.Main.SelectionController.masterSelect)
        {
            var thisSelect = GetComponent<Selectable>();

            if (!selectable.isEnemy && (this.gameObject != selectable.gameObject && thisSelect.isEnemy))
            {
                var damn = selectable.GetComponent<Damageable>();
                if (damn != null)
                {
                    if (Mathf.Abs(transform.position.x - damn.transform.position.x) <= squareSide && Mathf.Abs(transform.position.z - damn.transform.position.z) <= squareSide)
                    {
                        Debug.Log("is friendly" + selectable.isEnemy);
                        Ray ray;
                        ray = new Ray(transform.position, damn.transform.position - transform.position);
                        RaycastHit hitData;
                        if (Physics.Raycast(ray, out hitData, squareSide) && (hitData.transform.GetComponent<Selectable>())) // <- will this be problematic?
                        {
                            Debug.Log(hitData.transform.name + " enemy sees unit");
                            if (!isAttacking && GetComponent<Entity>().idle )
                            {
                                MoveToAttack(selectable);
                            }
                            //hitData.transform.GetComponentInParent<Selectable>();
                        }
                    }
                }
            }
            else if (selectable.isEnemy && (this.gameObject != selectable.gameObject && !thisSelect.isEnemy))
            {
               
                var damn = selectable.GetComponent<Damageable>();
                if (damn != null)
                {
             
                    if (Mathf.Abs(transform.position.x - damn.transform.position.x) <= squareSide && Mathf.Abs(transform.position.z - damn.transform.position.z) <= squareSide)
                    {
                        //Debug.Log("is enemy" + selectable.isEnemy);

                        Ray ray;
                        //+ Vector3.up
                        ray = new Ray(transform.position , damn.transform.position - transform.position);
                        RaycastHit hitData;
                        //Debug.Log(Physics.Raycast(ray, out hitData, squareSide));
                        if (Physics.Raycast(ray, out hitData, squareSide) && (hitData.transform.GetComponent<Selectable>())) // <- will this be problematic?
                        {
                            //Debug.Log(hitData.transform.name + " unit sees enemy");
                            // && GetComponent<Entity>().CommandGroup == null
                            if (!isAttacking && GetComponent<Entity>().idle)
                            {
                                MoveToAttack(selectable);
                            }
                            //hitData.transform.GetComponentInParent<Selectable>();
                        }
                    }
                }
            }
        }
    }
    public void MoveToAttack(Selectable target)
    {
        isAttacking = true;
            Entity entity = GetComponent<Entity>();
            if (entity != null)
            {
                entity.pathindex = 0;
                CommandGroup old = entity.CommandGroup;
                if (old != null)
                {
                    old.entities.Remove(entity);
                }
                //entity.CommandGroup.entities.Remove(entity);

                //entity.idle = false;
            }
        entity.targetObject = target.gameObject;
        entity.targetPos = entity.targetObject.transform.position;
        entity.targetHealth = target.GetComponent<Damageable>().Health;
        entity.idle = false;
        entity.perch = false;
        
    }
    public void Attack()
    {
        
    }
}
