using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruthBullet : Bullet
{
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IBreakable>() != null)
        {
            other.GetComponent<IBreakable>().Break();
        }
        else if (other.GetComponent<IBulletInteractor>() != null)
        {
            other.GetComponent<IBulletInteractor>().Interact(this);
        }
        Destroy(gameObject);
    }
}
