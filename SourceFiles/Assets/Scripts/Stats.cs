using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public string unitName;
    public int wounds;
    public int endurance;
    public float speed;
    public float combatRange;
    public float jumpPower;
    public float fallResistance;
    [HideInInspector] public bool dead;
    [HideInInspector] public int wound;
    [HideInInspector] public int damage;
    public float sprintSpeed;
    public int sprintCost;
    public float tumbleSuccessRate;
    public int tumbleCost;
    public int avoidance;
    public int weaponSkill;
    public int guardRate;
    public int guardCost;
    public float armorRate;
}
