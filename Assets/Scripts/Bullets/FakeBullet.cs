﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeBullet : Bullet
{
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Scattered") && other.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            if (other.GetComponent<IBulletInteractor>() != null)
            {
                other.GetComponent<IBulletInteractor>().Interact(this);
            }
            Destroy(gameObject);
        }
	}
}
