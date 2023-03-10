using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class AshePunchingState : State
{
    public AshePunchingState(Pawn context, PawnStateFactory factory) : base(context, factory)
    {
        m_isRootState = true;
        m_animationName = "Punching";
    }

    public override void EnterState()
    {
        m_context.CanJump = false;
        m_context.CanMove = false;
        ((AshePawn)m_context).LifitingRegion.enabled = false;
        m_context.Animator.speed = 1;
        m_context.Animator.Play(m_animationName);
        m_context.AudioSource.clip = m_context.Data.ScratchPadSounds[0];
    }
    public override void UpdateState()
    {
        CheckSwitchState();
    }
    public override void ExitState() 
    {
        m_context.CanJump = true;
        m_context.CanMove = true;
        ((AshePawn)m_context).LifitingRegion.enabled = true;
    }
    public override void InitializeSubState()
    {
        
    }

    public override void CheckSwitchState()
    {
        if (((AshePawn)m_context).IsLifting || !((AshePawn)m_context).IsPunching)
        {
            SwitchState(m_factory.AsheDefaultState());
        } 
    }
}