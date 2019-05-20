﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTest : MonoBehaviour
{
    Animator anim;

    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
            anim.SetBool("isWalking", true);
        else if (Input.GetKey(KeyCode.S))
            anim.SetBool("isWalking", false);
        else if (Input.GetKeyDown(KeyCode.D))
        {
            anim.SetTrigger("shoot");
        }
        else if (Input.GetKey(KeyCode.F))
            anim.ResetTrigger("shoot");
        else if (Input.GetKey(KeyCode.G))
            anim.SetBool("isShooting", true);
        else if (Input.GetKey(KeyCode.H))
            anim.SetBool("isShooting", false);
    }
}
