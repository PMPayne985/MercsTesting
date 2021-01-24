using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] Transform target;
    public float attackRange;
    public float attackTime;
    float attackTimer;
    public GameObject weapon;
    public float facing;
    Stats stats;
    public int hitChance;
    public int toHit;
    HUDControl hud;

    // Start is called before the first frame update
    void Start()
    {
        stats = GetComponent<Stats>();
        hud = GameObject.Find("HUD").GetComponent<HUDControl>();
    }

    public void CheckAttack()
    {
        attackTimer -= Time.deltaTime;
        FindTarget();

        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
            weapon.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
        }

        if (attackTimer < 0)
        {
            weapon.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
        }

        if (target != null)
        {
            facing = Vector3.Dot((target.position - transform.position).normalized, transform.TransformDirection(Vector3.forward));
        }
    }

    public void Attack()
    {       
        if (target != null)
        {
            hitChance = Random.Range(5, 96);

            float dis = Vector3.Distance(transform.position, target.position);
            int baseToHit = (stats.weaponSkill * 5) + 50;
            toHit = baseToHit - (target.GetComponent<Stats>().avoidance * 5);

            if (dis <= attackRange && attackTimer <= 0 && facing >= 0.7f)
            {
                if (hitChance <= toHit)
                {                   
                    if (target.gameObject.GetComponent<EnemyControl>().curEndurance > 0)
                    {
                        int damage = Random.Range(1, 4);
                        target.gameObject.GetComponent<Stats>().damage += damage;
                        hud.WriteToLog("Player hits: " + damage + " damage!");
                    }
                    else
                    {
                        hud.WriteToLog("Player hits: 1 wound!");
                        target.gameObject.GetComponent<Stats>().wound += 1;
                    }                  
                }
                
                attackTimer = attackTime;
            }
        }       
    }

    public void FindTarget()
    {
        GameStateControl stateCon = FindObjectOfType<GameStateControl>();
        Transform tempTarget = null;
        float tempDistance = 0;
        bool allNull = false;

        foreach (EnemyControl enemy in stateCon.allEnemies)
        {
            if (enemy != null)
            {
                allNull = false;
                break;
            }
            else
            {
                allNull = true;
            }
        }

        if (!allNull)
        {
            foreach (EnemyControl enemy in stateCon.allEnemies)
            {
                if (enemy != null)
                {
                    float dis = Vector3.Distance(transform.position, enemy.transform.position);

                    if (dis < tempDistance || tempDistance == 0)
                    {
                        tempDistance = dis;
                        tempTarget = enemy.transform;
                    }
                }
            }
            target = tempTarget;
        }
    }
}