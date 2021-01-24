using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : MonoBehaviour
{
    Stats stats;
    PlayerControl player;
    public float armorRate;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        stats = GameObject.FindGameObjectWithTag("Player").GetComponent<Stats>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            stats.armorRate += armorRate;
            player.armor.SetActive(true);
            Destroy(gameObject);
        }
    }
}
