using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TinkerDefaultState : State
{
    public TinkerDefaultState(Pawn context, PawnStateFactory factory) : base(context, factory)
    {
        m_isRootState = true;
        m_animationName = "";
        InitializeSubState();
    }

    public override void EnterState()
    {
        Debug.Log("Switched to TinkerDefault");
    }
    public override void UpdateState()
    {

    }
    public override void ExitState()
    {

    }
    public override void InitializeSubState()
    {
        if (!m_context.IsGrounded)
        {
            SetSubState(m_factory.Falling());
        }
        else if (m_context.IsJumping)
        {
            SetSubState(m_factory.Jumping());
        }
        else if (m_context.IsMoving)
        {
            SetSubState(m_factory.Moving());
        }
        else
        {
            SetSubState(m_factory.Grounded());
        }

        m_subState.EnterState();
    }

    public override void CheckSwitchState()
    {
        if (m_context.IsHoldingItem)
        {
            SwitchState(m_factory.TinkerItemState());
        }
        else if(((TinkerPawn)m_context).IsHeld)
        {
            SwitchState(m_factory.TinkerHeldState());
        }
        else if (((TinkerPawn)m_context).IsShooting)
        {
            SwitchState(m_factory.TinkerShootState());
        }
    }
}
