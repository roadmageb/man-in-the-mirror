using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTest : MonoBehaviour
{
    Animator anim;
    public Transform aimObject;
    public Transform target;
    Quaternion aimRotation;
    bool isShooting = false;

    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        aimRotation = aimObject.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
            anim.SetBool("isWalking", true);
        else if (Input.GetKeyDown(KeyCode.S))
            anim.SetBool("isWalking", false);
        if (Input.GetKeyDown(KeyCode.Q))
        {
            anim.SetBool("isShooting", true);
            isShooting = true;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            anim.SetBool("isShooting", false);
            isShooting = false;
        }
        if (Input.GetKeyDown(KeyCode.T))
            anim.SetTrigger("shoot");

        if (isShooting)
        {
            aimObject.LookAt(target, new Vector3(0,1,0));
        }
        else
        {
            aimObject.rotation = aimRotation;
        }
    }
}
