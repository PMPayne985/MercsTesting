using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public Stats stats;
    HUDControl hud;
    public float xRotAxis;
    public float xRot;

    // Start is called before the first frame update
    void Start()
    {
        hud = GameObject.Find("HUD").GetComponent<HUDControl>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!stats.dead)
        {
            Vector3 angle = transform.rotation.eulerAngles;
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

            Vector3 rotate = new Vector3(xRotAxis, 0, 0);
            transform.Rotate(-rotate);
        }
    }
}
