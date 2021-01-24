using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameStates : IState
{
    GameStateControl owner;

    public GameStates(GameStateControl owner) { this.owner = owner; }

    public void Enter()
    {
        Debug.Log("entering test state");
    }

    public void Execute()
    {
        Debug.Log("updating test state");
    }

    public void Exit()
    {
        Debug.Log("exiting test state");
    }
}

public class MenuState : IState
{
    GameStateControl owner;

    public MenuState(GameStateControl owner) { this.owner = owner; }

    public void Enter()
    {
        
    }

    public void Execute()
    {
        
    }

    public void Exit()
    {
        
    }
}

public class GameOverState : IState
{
    GameStateControl owner;

    public GameOverState(GameStateControl owner) { this.owner = owner; }

    public void Enter()
    {
        Time.timeScale = 0;
        owner.gameOverPanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void Execute()
    {

    }

    public void Exit()
    {

    }
}

public class CombatState : IState
{
    GameStateControl owner;

    public CombatState(GameStateControl owner) { this.owner = owner; }

    public void Enter()
    {
        owner.activePlayer.GetComponent<PlayerControl>().combatActive = true;

        foreach (EnemyControl enemy in owner.allEnemies)
        {
            if (enemy != null)
            enemy.stateMachine.ChangeState(new EnemyIdleState(enemy));
        }
    }

    public void Execute()
    {
        if (!owner.activePlayer.GetComponent<Stats>().dead)
        {
            owner.activePlayer.GetComponent<PlayerMovement>().CameraRotate();
            owner.activePlayer.GetComponent<PlayerControl>().Guard();
            owner.activePlayer.GetComponent<PlayerAttack>().CheckAttack();
        }

        foreach (EnemyControl enemy in owner.allEnemies)
        {
            if (enemy != null)
                enemy.AttackCheck();
        }
    }

    public void Exit()
    {
        owner.activePlayer.GetComponent<PlayerControl>().combatActive = false;

        foreach (EnemyControl enemy in owner.allEnemies)
        {
            if (enemy != null)
                enemy.stateMachine.ChangeState(new EnemySearchState(enemy));
        }

    }
}

public class MovementState : IState
{
    GameStateControl owner;
    

    public MovementState(GameStateControl owner) { this.owner = owner; }

    public void Enter()
    {

    }

    public void Execute()
    {
        foreach (GameObject player in owner.curParty)
        {
            if (player != null)
            {
                if (!player.GetComponent<Stats>().dead && player.GetComponent<PlayerControl>().currentActive)
                {
                    player.GetComponent<PlayerMovement>().BasicControls();
                    player.GetComponent<PlayerMovement>().CameraRotate();                    
                    player.GetComponent<PlayerControl>().Tumble();
                    player.GetComponent<PlayerControl>().Sprint();
                }
                else if (!player.GetComponent<Stats>().dead && !player.GetComponent<PlayerControl>().currentActive)
                {
                    player.GetComponent<PlayerMovement>().AIControl();
                }
            }
        }

        foreach (EnemyControl enemy in owner.allEnemies)
        {
            if (enemy != null)
            {

            }
        }
    }

    public void Exit()
    {

    }
}