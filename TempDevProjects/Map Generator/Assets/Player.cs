using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : Entity
{
    public float speed = 10;
    public float sprintModifier = 1.5f;
    public float jumpHeight = 3;

    public float attackCooldown = 0.5f;
    float lockAttackTill = 0;

    bool focusOnEnemy = false;

    override protected void Start()
    {
        base.Start();


    }

    override protected void Update()
    {
        base.Update();
        HandleInput();
    }

    void HandleInput()
    {
        if (state > EntityAnimator.AnimatorState.IDLE_COMBAT && lockAttackTill > Time.time)
        {
            return;
        }
        //Movement
        Vector3 h = Input.GetAxis("Horizontal") * FindObjectOfType<CameraController>().GetXZRight();
        Vector3 v = Input.GetAxis("Vertical") * FindObjectOfType<CameraController>().GetXZForward();
        Vector3 move = h + v;

        float effectiveSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            effectiveSpeed *= sprintModifier;
        }
        Move(move * effectiveSpeed * Time.deltaTime);

        Vector3 test = move;
        test.y = 0;
        if (test.magnitude > 0 && character.isGrounded)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                state = EntityAnimator.AnimatorState.SPRINT;

            }
            else
            {
                state = EntityAnimator.AnimatorState.RUN_FORWARD;
            }
        }
        else if (character.isGrounded)
        {
            //var enemies = FindObjectsOfType<Enemy>();
            //if (enemies.Length > 0)
            //{
            //    Enemy e = enemies.Aggregate((minItem, nextItem) => Vector3.Distance(minItem.transform.position, transform.position) < Vector3.Distance(nextItem.transform.position, transform.position) || nextItem.dead ? minItem : nextItem);

            //    RotateTowards(e.transform.position);
            //}
            //if (weaponIndex != 0)
            //{
            //    state = EntityAnimator.AnimatorState.IDLE_COMBAT;
            //}
            
            
                state = EntityAnimator.AnimatorState.IDLE;
            
        }
        else
        {
            state = EntityAnimator.AnimatorState.FALLING_LOOP;
        }

        //Jump
        if (Input.GetAxis("Jump") > 0 && character.isGrounded)
        {
            if (state == EntityAnimator.AnimatorState.RUN_FORWARD || state == EntityAnimator.AnimatorState.SPRINT)
            {
                state = EntityAnimator.AnimatorState.JUMP_WHILE_RUNNING;
            }
            else
            {
                state = EntityAnimator.AnimatorState.JUMP;
            }
            Jump(jumpHeight);
        }

        //Attack
        //if (lockAttackTill < Time.time)
        //{
        //    weapon.attacking = false;
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        if (weaponIndex != 0)
        //        {
        //            state = EntityAnimator.AnimatorState.MELEE_ATTACK_ONE_HANDED;
        //        }
        //        else
        //        {
        //            state = EntityAnimator.AnimatorState.PUNCH_LEFT;
        //        }
        //        weapon.attacking = true;
        //        lockAttackTill = Time.time + attackCooldown;
        //    }
        //}

        //if (Input.GetKey(KeyCode.Alpha1))
        //{
        //    weapon.gameObject.SetActive(false);
        //    weaponIndex = 0;
        //    weapon = weapons.GetChild(weaponIndex).GetComponent<Weapon>();
        //    weapon.gameObject.SetActive(true);
        //}
        //else if (Input.GetKey(KeyCode.Alpha2))
        //{
        //    weapon.gameObject.SetActive(false);
        //    weaponIndex = 1;
        //    weapon = weapons.GetChild(weaponIndex).GetComponent<Weapon>();
        //    weapon.gameObject.SetActive(true);
        //}
        //else if (Input.GetKey(KeyCode.Alpha3))
        //{
        //    weapon.gameObject.SetActive(false);
        //    weaponIndex = 2;
        //    weapon = weapons.GetChild(weaponIndex).GetComponent<Weapon>();
        //    weapon.gameObject.SetActive(true);
        //}
    }
}
