using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    CharacterController con;
    Stats stats;
    HUDControl hud;
    Vector3 velocity;
    [HideInInspector] public float speed;
    float gravity = -9.81f;
    float jumpPower;

    float xRotAxis;
    float xRot;

    public GameObject cameraPivot;

    public bool fallDamage;
    public float impactMultiplyer;
    public int impactDamage;

    public float aIFollowRange;

    // Start is called before the first frame update
    public void Awake()
    {
        stats = GetComponent<Stats>();
        con = GetComponent<CharacterController>();
        hud = GameObject.Find("HUD").GetComponent<HUDControl>();
        speed = stats.speed;
        jumpPower = stats.jumpPower;
    }

    void Update()
    {
        if (!stats.dead)
        {
            FallDamage();
        }
    }

    void FallDamage()
    {
        if (con.isGrounded && velocity.y < 0)
        {
            velocity.y = 0;
            if (fallDamage)
            {
                stats.wound = impactDamage;
                fallDamage = false;
                hud.WriteToLog(impactDamage + " damage from falling!");
            }
        }

        if (velocity.y <= (gravity * stats.fallResistance))
        {
            fallDamage = true;

        }
        else
        {
            fallDamage = false;
        }

        impactMultiplyer = (velocity.y / gravity) - stats.fallResistance;
        impactDamage = Mathf.RoundToInt(impactMultiplyer);
        if (impactDamage < 0)
            impactDamage = 0;
    }

    public void BasicControls()
    {
        velocity.y += gravity * Time.deltaTime;
        con.Move(velocity * Time.deltaTime);

        if (con.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y += jumpPower;
            }
        }

        Vector3 move = transform.TransformDirection(Vector3.forward) * Input.GetAxis("Vertical");
        Vector3 straff = transform.TransformDirection(Vector3.right) * Input.GetAxis("Horizontal");

        Vector3 myForward = new Vector3(move.x, -0.1f, move.z);
        Vector3 myRight = new Vector3(straff.x, -0.1f, straff.z);

        con.Move(velocity * Time.deltaTime);

        con.Move(move * Time.deltaTime * speed);
        con.Move(straff * Time.deltaTime * speed);

        con.Move(velocity * Time.deltaTime);
    }

    public void CameraRotate()
    {
        Vector3 angle = cameraPivot.transform.rotation.eulerAngles;
        Vector3 rotate = new Vector3(0, Input.GetAxis("Mouse X"), 0);
        float xRotRaw = angle.x;
        if (xRotRaw >= 120)
        {
            xRot = xRotRaw - 315;
        }
        else
        {
            xRot = xRotRaw + 45;
        }

        if (xRot <= 15)
        {
            xRotAxis = Mathf.Clamp(Input.GetAxis("Mouse Y"), -1, 0);
        }
        else if (xRot >= 75)
        {
            xRotAxis = Mathf.Clamp(Input.GetAxis("Mouse Y"), 0, 1);
        }
        else
        {
            xRotAxis = Input.GetAxis("Mouse Y");
        }

        transform.Rotate(rotate);
        Vector3 pivot = new Vector3(xRotAxis, 0, 0);
        cameraPivot.transform.Rotate(-pivot);
    }

    public void AIControl()
    {
        Transform player = FindObjectOfType<GameStateControl>().activePlayer.transform;
        float chase = Vector3.Distance(transform.position, player.position);
        if (chase > aIFollowRange)
        {
            GetComponent<NavMeshAgent>().SetDestination(player.position);
        }
        else
        {
            GetComponent<NavMeshAgent>().SetDestination(transform.position);
        }
    }
}
