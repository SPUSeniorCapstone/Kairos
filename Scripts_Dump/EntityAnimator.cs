using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;


public class EntityAnimator : MonoBehaviour
{
    Animator animator;
    HeroEntity entity;
    
    Unit unit;

    public AnimatorState animatorState
    {
        get { return state; }
        set { SetState(value); }
    }

    private AnimatorState state = AnimatorState.IDLE;

    public float lockTill = 0;

    private void Start()
    {
        animator = GetComponent<Animator>();
        entity = GetComponent<HeroEntity>();
     
        unit = GetComponent<Unit>();

        animatorState = AnimatorState.IDLE;
    }

    private void Update()
    {
        if (entity != null)
        {
            animatorState = entity.state;
        }
    }

    public void SetState(AnimatorState value)
    {

        if(lockTill > Time.time || value == state && value != AnimatorState.GET_HIT)
        {
            return;
        }

        string name = GetStateString(value);

        animator.CrossFade(name, 0.2f);
        state = value;
        if( state != AnimatorState.IDLE && 
            state != AnimatorState.IDLE_COMBAT &&
            state != AnimatorState.RUN_FORWARD && 
            //state != AnimatorState.JUMP && 
            //state != AnimatorState.JUMP_WHILE_RUNNING &&
            state != AnimatorState.FALLING_LOOP &&
            state != AnimatorState.SPRINT
        )
        {
            lockTill = Time.time + animator.GetCurrentAnimatorClipInfo(0).Length;
        }
    }

    public string GetStateString(AnimatorState value)
    {
        return Enum.GetName(typeof(AnimatorState), value);
    }



    public enum AnimatorState
    {
        //**Basic Movement**
        IDLE,
        RUN_FORWARD, RUN_LEFT, RUN_RIGHT,
        RUN_BACKWARD, RUN_BACKWARD_LEFT, RUN_BACKWARD_RIGHT,
        SPRINT,

        JUMP, JUMP_DOWN, JUMP_UP, 
        JUMP_WHILE_RUNNING, 
        FALLING_LOOP,

        ROLL_FORWARD, ROLL_BACKWARD, ROLL_LEFT, ROLL_RIGHT,
        STRAFE_LEFT, STRAFE_RIGHT,


        //**Combat Movement**
        IDLE_COMBAT,
        
        CASTING_LOOP,
        SPELL_CAST, SPELL_CAST_START, SPELL_CAST_END,

        STUNNED_LOOP, 
        BLOCKING_LOOP,
        
        BUFF,
        
        BOWSHOT,

        MELEE_ATTACK_TWO_HANDED,
        MELEE_ATTACK_ONE_HANDED,
        PUNCH_RIGHT, PUNCH_LEFT,

        //Other
        GATHERING,
        MINING_LOOP,
        GET_HIT,
        DEATH,

        T_POSE
    }

    public class StateInfo
    {
        public AnimatorState state;
        public string stateString;

    }
}
