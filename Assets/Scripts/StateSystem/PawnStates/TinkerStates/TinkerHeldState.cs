using UnityEngine;

public class TinkerHeldState : State
{
    public TinkerHeldState(Pawn context, PawnStateFactory factory) : base(context, factory)
    {
        m_isRootState = true;
        m_animationName = "_gun";
        InitializeSubState();
    }

    public override void CheckSwitchState()
    {
        if (!((TinkerPawn)m_context).IsHeld)
        {
            SwitchState(m_factory.TinkerDefaultState());
        }
        if (m_context.IsJumping)
        {
            SwitchState(m_factory.TinkerDefaultState());
        }
    }

    public override void InitializeSubState()
    {
        SetSubState(m_factory.Grounded());
    }

    public override void EnterState()
    {
        Debug.Log("Switched to TinkerHeld");
        m_context.CanMove = false;   
    }

    public override void ExitState()
    {
        // Be able to move right before tinker lets tinker go
        m_context.CanMove = true;
        GameManager.Instance.Ashe.IsLifting = false;
    }
}
