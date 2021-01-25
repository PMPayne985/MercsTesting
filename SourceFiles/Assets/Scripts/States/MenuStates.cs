using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuStates : MonoBehaviour, IState
{
    HUDControl owner;

    public MenuStates (HUDControl owner) { this.owner = owner; }

    public void Enter()
    {
        Debug.Log("entering test state");
    }

    public void Execute()
    {
        Debug.Log("updating test state");
    }

    void IState.FixedExecute()
    {
        
    }

    public void Exit()
    {
        Debug.Log("exiting test state");
    }
}

public class ClosedState : IState
{
    HUDControl owner;
    GameStateControl gameState;

    public ClosedState(HUDControl owner) { this.owner = owner; }

    public void Enter()
    {      
        Time.timeScale = 1;
        owner.menuPanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        gameState = GameObject.Find("GameStateController").GetComponent<GameStateControl>();
        gameState.stateMachine.ChangeState(new MovementState(gameState));
    }

    public void Execute()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            owner.stateMachine.ChangeState(new SystemState(owner));
        }
    }

    void IState.FixedExecute()
    {
        
    }

    public void Exit()
    {
        
    }
}

public class SystemState: IState
{
    HUDControl owner;
    GameStateControl gameState;

    public SystemState(HUDControl owner) { this.owner = owner; }

    public void Enter()
    {
        gameState = GameObject.Find("GameStateController").GetComponent<GameStateControl>();
        gameState.stateMachine.ChangeState(new MenuState(gameState));
        Time.timeScale = 0;
        owner.menuPanel.SetActive(true);
        owner.systemPanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void Execute()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            owner.stateMachine.ChangeState(new ClosedState(owner));
        }
    }

    void IState.FixedExecute()
    {
        
    }

    public void Exit()
    {
        owner.systemPanel.SetActive(false);
    }
}

public class ComingSoonState : IState
{
    HUDControl owner;

    public ComingSoonState(HUDControl owner) { this.owner = owner; }

    public void Enter()
    {
        owner.comingSoonPanel.SetActive(true);
    }

    public void Execute()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            owner.stateMachine.ChangeState(new ClosedState(owner));
        }
    }

    void IState.FixedExecute()
    {
        
    }

    public void Exit()
    {
        owner.comingSoonPanel.SetActive(false);
    }
}
