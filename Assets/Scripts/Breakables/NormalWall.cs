using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalWall : MonoBehaviour, IInteractor
{
    public void Interact(Bullet bullet)
    {
        if (bullet is TruthBullet)
        {
            //Break Wall
        }
        else if (bullet is FakeBullet)
        {
            //Nothing happens
        }
        else if (bullet is MirrorBullet)
        {
            //Nothing happens
        }
    }
}
