using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerInput m_playerInput;
    [SerializeField] private InputActions m_inputActions;
    [SerializeField] private Pawn controlledPawn;
    public Pawn ControlledPawn => controlledPawn;
    public PlayerInput PlayerInput => m_playerInput;
    public bool JumpActive
    {
        set 
        {
            if (value)
            {
                m_playerInput.actions["Jump"].Enable();
            } 
            else
            {
                m_playerInput.actions["Jump"].Disable();
            }        
        }
    }
    public bool MoveActive
    {
        set
        {
            if (value)
            {
                m_playerInput.actions["Movement"].Enable();
            }
            else
            {
                m_playerInput.actions["Movement"].Disable();
            }
        }
    }
    public bool PrimaryActive
    {
        set
        {
            if (value)
            {
                m_playerInput.actions["UseToolPrimary"].Enable();
            }
            else
            {
                m_playerInput.actions["UseToolPrimary"].Disable();
            }
        }
    }
    public bool SecondaryActive
    {
        set
        {
            if (value)
            {
                m_playerInput.actions["UseToolSecondary"].Enable();
            }
            else
            {
                m_playerInput.actions["UseToolSecondary"].Disable();
            }
        }
    }

    private void Awake()
    {
        //m_inputActions = new InputActions();
        m_playerInput = GetComponent<PlayerInput>();

        //Old -- Adding Methods to inputActions using InputActions()
        //m_inputActions.Player.Jump.performed += JumpAction;
        //m_inputActions.Player.Jump.canceled += JumpAction;
        //m_inputActions.Player.Enable();

        //Adding methods to inputActions in PlayerInput
        m_playerInput.actions["Jump"].performed += JumpAction;
        m_playerInput.actions["Jump"].canceled += JumpAction;
        m_playerInput.actions["UseToolPrimary"].performed += UseToolPrimaryAction;
        m_playerInput.actions["UseToolSecondary"].performed += UseToolSecondaryAction;
        m_playerInput.actions["UseToolSecondary"].canceled += UseToolSecondaryAction;
        m_playerInput.actions["Interact"].performed += InteractAction;
        m_playerInput.actions["Pause"].performed += PauseAction;
        //m_playerInput.actions["Swap"].performed += SwapAction;

        m_playerInput.actions.actionMaps[0].Enable();
        //m_playerInput.actions["Pause"].Disable();
        m_playerInput.actions.actionMaps[1].Enable();
    }
    private void FixedUpdate()
    {
        //Old -- Using a new InputActions()
        //controlledPawn.Move(m_inputActions.Player.Movement.ReadValue<Vector2>());

        //Using PlayerInput itself
        // Hardcoded threshold disgusting
        // CURRENTLY HANDLES PAWN PHYSICS! -> When its not called, the physics and animation stops working correctly
        // Pawn should be handling its own physics while this should delgate the force, thats it!
        Vector2 inputVector = m_playerInput.actions["Movement"].ReadValue<Vector2>();
        inputVector.x = (Mathf.Abs(inputVector.x) > 0.6f) ? Mathf.Sign(inputVector.x) : 0;
        controlledPawn?.Move(inputVector);
    }
    #region Actions
    private void JumpAction(InputAction.CallbackContext context)
    {
        //This certainly could be coupled :p
        if (context.performed)
        {
            controlledPawn.Jump();
        }
        if (context.canceled)
        {
            controlledPawn.JumpCut();
        }
    }
    private void UseToolPrimaryAction(InputAction.CallbackContext context)
    {
        controlledPawn.PrimaryAction(context);
    }
    private void UseToolSecondaryAction(InputAction.CallbackContext context)
    {
        controlledPawn.SecondaryAction(context);
    }
    //VERY COUPPLED DO NOT PUSH FOR FINAL GAME
    private void InteractAction(InputAction.CallbackContext context)
    {
        //GameManager.Instance.CurrLevelManager.OnLevelExit();
        //SlideManager.Instance.CurrSlide.RemoveInfo();
        controlledPawn.ToggleGrabRope();
    }
    private void PauseAction(InputAction.CallbackContext context)
    {
        GameManager.Instance.TogglePause(this);
    }
    private void SwapAction(InputAction.CallbackContext context)
    {
        if (controlledPawn == GameManager.Instance.Ashe)
        {
            ControlPawn(GameManager.Instance.Tinker);
        }
        else if (controlledPawn == GameManager.Instance.Tinker)
        {
            ControlPawn(GameManager.Instance.Ashe);
        }
    }
    #endregion
    public void ControlPawn(Pawn pawn)
    {
        controlledPawn = pawn;
        m_playerInput.actions["Pause"].Enable();
    }
    public void EnablePawnControl()
    {
        m_playerInput.actions.actionMaps[0].Enable();
    }
    public void DisablePawnControl()
    {
        m_playerInput.actions.actionMaps[0].Disable();
        m_playerInput.actions["Pause"].Enable();
    }
}
