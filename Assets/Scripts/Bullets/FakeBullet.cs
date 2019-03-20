using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeBullet : Bullet
{
    protected override void OnTriggerEnter(Collider other)
    {
        //TODO : if mirror, break mirror and make objects
    }
}
