using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameStateControl : MonoBehaviour
{
    [HideInInspector] public StateMachine stateMachine = new StateMachine();
    [HideInInspector] public HUDControl hud;
    [HideInInspector] public bool sprinting;
    [HideInInspector] public bool tumbling;
    public GameObject activePlayer;
    public GameObject activeEnemy;
    public GameObject[] curParty;
    public EnemyControl[] allEnemies;

    public GameObject gameOverPanel;
    public bool fallDamage;
    public float impactMultiplyer;
    public int impactDamage;

    // Start is called before the first frame update
    void Awake()
    {
        hud = FindObjectOfType<HUDControl>();
        AssignActivePlayer();    
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();

        if (Input.GetButtonDown("Submit"))
        {
            if (activePlayer.GetComponent<PlayerControl>().combatActive)
            {
                stateMachine.ChangeState(new MovementState(this));
            }
            else
            {
                stateMachine.ChangeState(new CombatState(this));
            }
        }
    }

    void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    public void AssignActivePlayer()
    {
        curParty = GameObject.FindGameObjectsWithTag("Player");
        allEnemies = FindObjectsOfType<EnemyControl>();
        activeEnemy = allEnemies[0].gameObject;

        for (int i = 0; i < curParty.Length; i++)
        {
            if (curParty[i] != null && curParty[i] != activePlayer)
            {
                activePlayer = curParty[i];
                break;
            }
        }

        foreach (GameObject player in curParty)
        {
            if (player == activePlayer)
            {
                player.GetComponent<CharacterController>().enabled = true;
                player.GetComponent<NavMeshAgent>().enabled = false;
                player.GetComponent<PlayerMovement>().cameraPivot.SetActive(true);
                player.GetComponent<PlayerControl>().currentActive = true;
            }
            else
            {
                player.GetComponent<CharacterController>().enabled = false;
                player.GetComponent<NavMeshAgent>().enabled = true;
                player.GetComponent<PlayerMovement>().cameraPivot.SetActive(false);
                player.GetComponent<PlayerControl>().currentActive = false;
            }
        }
    }   
}
