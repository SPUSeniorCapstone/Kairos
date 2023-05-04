using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;

public class AttackTower : Unit
{
    public bool autoAttack = true;
    public Damageable target;

    public float personalDistance = 1;
    public float attackDistance = 1;
    public float attackCoolDown = 1;
    public float attackDamage = 10f;
    private float lastAttackTime = 0;
    public projectileArrow projectile;
    public float projectileSpeed;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
       if (autoAttack && target == null)
        {
            // passive auto attack
            var enemy = EnemyDetection();
            if (enemy != null)
            {

                if (autoAttack)
                {
                    SetTarget(enemy);
                }

            }
        }

        // is this neccesary? why not update all the time?
        if (target != null && Visible(target.GetComponent<Selectable>()) && autoAttack)
        {
            //when within attack range
            // bootleg combat

            // neccessary ?
            if (target != null)
            {
                if (Time.time - lastAttackTime > attackCoolDown)
                {
                    //Body.clip = BodySounds.ElementAt(10);
                    //Body.Play();
                    Vector3 offset = new Vector3(0, 10, 0);
                    Instantiate(projectile.gameObject, transform, false);
                    projectile.transform.position = offset;
                    projectile.speed = projectileSpeed;
                    projectile.target = target.gameObject;
                    target.Damage(attackDamage);
                    lastAttackTime = Time.time;

                    // neccessary? doesn't work all the time (race condition)
                    if (target.Health <= 0)
                    {
                        target = null;
                    }
                }
            }

        }
    }
    public Damageable EnemyDetection()
    {
        Damageable damageable = null;

        foreach (Selectable selectable in GameController.Main.SelectionController.masterSelect)
        {
            // No friendly fire
            if (GetComponent<Selectable>().faction ^ selectable.faction) // !^ = Not XOR
            {


                //Don't bother fighting things that cannot be damaged
                var temp = selectable.GetComponent<Damageable>();
                if (temp == null || temp.Invulnerable)
                {
                    continue;
                }

                if (Visible(selectable))
                {
                    if (damageable == null || Vector3.Distance(temp.transform.position, transform.position) < Vector3.Distance(damageable.transform.position, transform.position))
                    {
                        damageable = temp;

                    }
                }
            }

        }
        return damageable;
    }
    public bool Visible(Selectable selectable)
    {
        // if within square bounds
        if (Helpers.InSquareRadius(searchRadius, transform.position.ToVector2(), selectable.transform.position.ToVector2()))
        {
            Ray ray;
            // need the vector up or else only works in postive quadrant
            ray = new Ray(transform.position + Vector3.up, selectable.transform.position - transform.position);
            RaycastHit hitData;
            // if within line of sight
            // , LayerMask.NameToLayer(layerMask)
            if (Physics.Raycast(ray, out hitData, searchRadius, layerMask) && (hitData.transform == selectable.transform)) // <- will this be problematic?
            {
                //Debug.Log(hitData.transform.name + " spotted by " + name);
                return true;
            }
        }

        return false;
    }

    public void SetTarget(Damageable target)
    {
        this.target = target;
        isPerformingTask = true;
    }


    public override void PerformTaskOn(Selectable selectable)
    {
        var tar = selectable.GetComponent<Damageable>();
        Debug.Log(tar + " perform");
        if (tar != null)
        {
            SetTarget(tar);
        }
        else
        {
            Debug.Log("TAR IS NULL! => " + selectable);
        }
    }
}
