using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorBullet : Bullet
{
    protected override void OnTriggerEnter(Collider other)
    {
		if (other.GetComponent<IBulletInteractor>() != null)
		{
			other.GetComponent<IBulletInteractor>().Interact(this);
		}
		Destroy(gameObject);
	}
}