using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float deathTimer;
    public int curWounds;
    public int curEndurance;
    Stats stats;
    public bool guarding;
    public float guardTime;
    float guardTimer;
    public GameObject shield;
    public GameObject weapon;
    public GameObject cam;
    GameStateControl gameState;
    CharacterController con;
    HUDControl hud;
    PlayerMovement move;
    public GameObject armor;
    int finalDamage;
    bool partyWipe;
    public bool combatActive;
    public bool currentActive;

    bool sprinting;
    bool tumbling;
    public float sprintTime;
    float sprintTimer;
    public float tumbleTime;
    float tumbleTimer;
    float tempRes;

    // Start is called before the first frame update
    void Awake()
    {
        hud = GameObject.Find("HUD").GetComponent<HUDControl>();
        gameState = GameObject.Find("GameStateController").GetComponent<GameStateControl>();
        move = GetComponent<PlayerMovement>();
        stats = GetComponent<Stats>();
        con = GetComponent<CharacterController>();
        curWounds = stats.wounds;
        curEndurance = stats.endurance;
        weapon = GetComponent<PlayerAttack>().weapon;
    }

    // Update is called once per frame
    void Update()
    {
        DetectWound();
        DetectDamage();

        if (curWounds <= 0)
        {
            StartCoroutine(TransitionDeath());
        }
    }

    void Death()
    {              
        GameObject[] allPlayerUnits = GameObject.FindGameObjectsWithTag("Player");

        for(int i = 0; i < allPlayerUnits.Length ; i++)
        {
            if (allPlayerUnits[i] != null)
            {
                if (allPlayerUnits[i] != gameObject)
                {
                    FindObjectOfType<GameStateControl>().AssignActivePlayer();
                    partyWipe = false;
                    break;
                }
                else
                {
                    partyWipe = true;
                }
            }
            else
            {
                partyWipe = true;
            }
        }

        if (partyWipe)
        {
            cam.transform.SetParent(GameObject.Find("GameStateController").transform);
            gameState.stateMachine.ChangeState(new GameOverState(gameState));
        }
        else
        {
            hud.WriteToLog(gameObject.name + " Defeated!");
            Destroy(gameObject);
        }
    }

    IEnumerator TransitionDeath()
    {
        stats.dead = true;
        con.Move(new Vector3(0, 0, 0));       
        yield return new WaitForSeconds(deathTimer);

        Death();
    }

    void DetectWound()
    {
        if (stats.wound > 0)
        {
            curWounds -= stats.wound;
            stats.wound = 0;
        }
    }

    void DetectDamage()
    {
        if (stats.damage > 0)
        {         
            if (stats.armorRate > 0)
            {
                float tempDamage = stats.damage;
                finalDamage = Mathf.RoundToInt(tempDamage * (1 - stats.armorRate));
            }
            else
            {
                finalDamage = stats.damage;
            }

            curEndurance -= finalDamage;
            stats.damage = 0;

            if (curEndurance < 0)
                curEndurance = 0;

            finalDamage = 0;
        }
    }

    public void Guard()
    {
        if (Input.GetButtonDown("Guard") && !guarding && !gameState.sprinting && !gameState.tumbling && curEndurance >= stats.guardCost && con.isGrounded)
        {
            guardTimer = guardTime;
            curEndurance -= stats.guardCost;
            guarding = true;
            stats.avoidance += stats.guardRate;
            shield.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
            hud.WriteToLog("Player Avoidance increased!");
        }

        if (guarding)
        {
            guardTimer -= Time.deltaTime;

            if (guardTimer <= 0)
            {
                shield.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
                stats.avoidance -= stats.guardRate;
                guarding = false;
                guardTimer = guardTime;
            }
        }
    }

    public void Sprint()
    {
        if (Input.GetButtonDown("Sprint") && !sprinting && curEndurance >= stats.sprintCost && !tumbling && !guarding)
        {
            curEndurance -= stats.sprintCost;
            GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
            move.speed += stats.sprintSpeed;
            sprinting = true;
            sprintTimer = sprintTime;
            hud.WriteToLog("Speed increased!");
        }

        if (sprinting)
        {
            sprintTimer -= Time.deltaTime;

            if (sprintTimer <= 0)
            {
                GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                move.speed -= stats.sprintSpeed;
                sprinting = false;
            }
        }
    }

    public void Tumble()
    {
        if (Input.GetButtonDown("Tumble") && !tumbling && !sprinting && !guarding && curEndurance >= stats.tumbleCost && !con.isGrounded)
        {
            tempRes = stats.fallResistance;
            GetComponent<PlayerControl>().curEndurance -= stats.tumbleCost;

            tumbling = true;
            float tumbleChance = Random.Range(1, 101);
            if (tumbleChance <= stats.tumbleSuccessRate)
            {
                GetComponent<Renderer>().material.SetColor("_Color", Color.white);
                stats.fallResistance += 2;
                tumbling = true;
                hud.WriteToLog("Tumbling!");
            }

            tumbleTimer = tumbleTime;
        }

        if (tumbling)
        {
            tumbleTimer -= Time.deltaTime;

            if (tumbleTimer <= 0)
            {
                GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                stats.fallResistance = tempRes;
                tumbling = false;
            }
        }
    }
}
