using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class RedInfantry_Unit : Infantry_Unit
{
    public GameObject neutral;
    public GameObject frame1;
    public GameObject frame2;
    public override void Attack()
    {
        entity.targetPos = target.transform.position;
        //when within attack range
        // bootleg combat
        // && Vector3.Distance(transform.position, target.transform.position) < attackDistance
        if ((entity.movementDirection == Vector3.zero && archer || !archer))
        {
            // neccessary ?
            if (target != null)
            {
                //entity.RotateTowards(target.transform.position.normalized);
                transform.LookAt(target.transform);

                //transform.rotation = Quaternion.LookRotation(target.transform.position);
                if (GameController.Main.randomAttackCooldown)
                {
                    RandomAttack();
                }
                else
                {
                    if (Time.time - lastAttackTime > animationCoolDown)
                    {
                        Debug.Log("will first anim play?");
                        neutral.SetActive(false);
                        frame1.SetActive(true);
                        frame2.SetActive(false);

                    }
                    if (Time.time - lastAttackTime > attackCoolDown)
                    {
                        //Body.clip = BodySounds.ElementAt(10);
                        //Body.Play();

                        
                        
                            Debug.Log("ITS TIME BITCH");
                            neutral.SetActive(false);
                            frame1.SetActive(false);
                            frame2.SetActive(true);
                        if (GameController.Main.randomDamageModifier)
                        {
                            float rand = Random.Range(1f, 2f);
                            target.Damage(attackDamage * rand);
                        }
                        else
                        {
                            target.Damage(attackDamage);
                        }
                     
                        

                        lastAttackTime = Time.time;

                        // neccessary? doesn't work all the time (race condition)
                        if (target.Health <= 0)
                        {
                            neutral.SetActive(true);
                            frame1.SetActive(false);
                            frame2.SetActive(false);
                            entity.movementMode = Infantry_Entity.MovementMode.IDLE;
                            target = null;
                        }
                    }
                }
              
            }
        }
    }
    public override void RandomAttack()
    {
        float randCoolDown = Random.Range(1f, 1.2f);
        if (Time.time - lastAttackTime > animationCoolDown * randCoolDown)
        {
            Debug.Log("will first anim play?");
            neutral.SetActive(false);
            frame1.SetActive(true);
            frame2.SetActive(false);

        }
        if (Time.time - lastAttackTime > attackCoolDown * randCoolDown)
        {
            //Body.clip = BodySounds.ElementAt(10);
            //Body.Play();


            neutral.SetActive(false);
            frame1.SetActive(false);
            frame2.SetActive(true);
            if (GameController.Main.randomDamageModifier)
            {
                float rand = Random.Range(1f, 2f);
                target.Damage(attackDamage * rand);
            }
            else
            {
                target.Damage(attackDamage);
            }

            lastAttackTime = Time.time;

            // neccessary? doesn't work all the time (race condition)
            if (target.Health <= 0)
            {
                neutral.SetActive(true);
                frame1.SetActive(false);
                frame2.SetActive(false);
                entity.movementMode = Infantry_Entity.MovementMode.IDLE;
                target = null;
            }
        }
    }
    public override void OnSelect()
    {
        base.OnSelect();
        neutral.GetComponent<MeshRenderer>().material.shader = GameController.Main.highlight;
        frame1.GetComponent<MeshRenderer>().material.shader = GameController.Main.highlight;
        frame2.GetComponent<MeshRenderer>().material.shader = GameController.Main.highlight;
    }
    public override void OnDeselect()
    {
        neutral.GetComponent<MeshRenderer>().material.shader = mySelectable.unHighlight;
        frame1.GetComponent<MeshRenderer>().material.shader = mySelectable.unHighlight;
        frame2.GetComponent<MeshRenderer>().material.shader = mySelectable.unHighlight;
    }
}
