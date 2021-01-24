using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyControl : MonoBehaviour
{
    [HideInInspector] public StateMachine stateMachine = new StateMachine();

    Stats stats;
    public float damageTimer;
    public float damageTime = 1;
    public int curWounds;
    public int curEndurance;
    public GameObject weapon;
    public float facing;
    public int hitChance;
    public int toHit;
    public float enemyWanderRange;
    HUDControl hud;

    public NavMeshAgent agent;
    public Transform curTarget;
    public float chaseRange;
    public float stopRange;

    // Start is called before the first frame update
    void Start()
    {
        hud = GameObject.Find("HUD").GetComponent<HUDControl>();
        stats = GetComponent<Stats>();
        agent = GetComponent<NavMeshAgent>();

        curEndurance = stats.endurance;
        curWounds = stats.wounds;        
        agent.speed = stats.speed;

        weapon.GetComponent<Renderer>().material.SetColor("_Color", Color.black);

        stateMachine.ChangeState(new EnemySearchState(this));
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();     
        
        DetectWound();
        DetectDamage();

        if (curWounds <= 0)
        {
            Death();
        }

    }

    void Death()
    {
        Destroy(gameObject);
        hud.WriteToLog(gameObject.name + " defeated!");
    }

    public void AttackCheck()
    {
        curTarget = FindObjectOfType<GameStateControl>().activePlayer.transform;
        damageTimer -= Time.deltaTime;

        if (damageTimer < 0)
        {
            weapon.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            StartCoroutine(DealDamage());
        }       
    }

    IEnumerator DealDamage()
    {        
        if (curTarget != null)
        {
            facing = Vector3.Dot((curTarget.position - transform.position).normalized, transform.TransformDirection(Vector3.forward));

            if (Vector3.Distance(curTarget.position, transform.position) <= 3 && facing >= 0.7f)
            {
                damageTimer = damageTime;
                yield return new WaitForSeconds(0.25f);

                weapon.GetComponent<Renderer>().material.SetColor("_Color", Color.black);

                hitChance = Random.Range(5, 96);
                int baseToHit = (stats.weaponSkill * 5) + 50;
                toHit = baseToHit - (curTarget.GetComponent<Stats>().avoidance * 5);

                if (hitChance <= toHit)
                {

                    if (curTarget.GetComponent<PlayerControl>().curEndurance > 0)
                    {
                        int damage = Random.Range(1, 7);
                        curTarget.GetComponent<Stats>().damage = damage;
                        hud.WriteToLog("Enemy hits: " + damage + " damage!");

                    }
                    else
                    {
                        curTarget.GetComponent<Stats>().wound = 1;
                        hud.WriteToLog("Enemy hits: 1 wound!");
                    }
                }                
            }
        }        
    }

    public void FindTarget()
    {
        GameObject tempTarget = null;
        float tempDistance = 0;

        if (FindObjectOfType<GameStateControl>().curParty[0] != null)
        {
            foreach (GameObject player in FindObjectOfType<GameStateControl>().curParty)
            {
                float dis = Vector3.Distance(transform.position, player.transform.position);

                if (dis < tempDistance || tempDistance == 0)
                {
                    tempDistance = dis;
                    tempTarget = player.gameObject;
                }
            }
            curTarget = tempTarget.transform;
        }
        else
        {
            curTarget = null;
        }
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
            curEndurance -= stats.damage;
            stats.damage = 0;
            if (curEndurance < 0)
                curEndurance = 0;
        }
    }
}
