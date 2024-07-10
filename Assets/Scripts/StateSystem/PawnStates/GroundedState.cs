using UnityEngine;

public class GroundedState : State
{
    public GroundedState(Pawn context, PawnStateFactory factory) : base(context, factory) 
    {
        m_animationName = "Idle";
    }
    public override void EnterState()
    {
        Debug.Log($"Entered Ground with {m_superState.AnimationName}");
        m_context.Animator.speed = 1;
        m_context.Animator.Play(m_animationName + m_superState.AnimationName);
    }

    public override void UpdateState()
    {
        //CheckSwitchState();
    }
    public override void CheckSwitchState()
    {
        if (m_context.IsJumping)
        {
            SwitchState(m_factory.Jumping());
        }
        else if (m_context.IsMoving && m_context.IsGrounded)
        {
            SwitchState(m_factory.Moving());
        }
        else if (!m_context.IsGrounded)
        {
            SwitchState(m_factory.Falling());
        }
        //else if (m_context.IsHeld)
        //{
        //    SwitchState(m_factory.Held());
        //}
    }
}
