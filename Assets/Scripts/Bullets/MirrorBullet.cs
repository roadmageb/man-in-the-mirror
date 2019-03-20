using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorBullet : Bullet
{
    protected override void OnTriggerEnter(Collider other)
    {
        //If wall, make a new mirror on the wall
    }
}