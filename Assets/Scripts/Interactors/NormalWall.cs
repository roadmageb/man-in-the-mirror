using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalWall : MonoBehaviour, IBulletInteractor
{
    public void Interact(Bullet bullet)
    {
        if (bullet is TruthBullet)
        {
            Destroy(gameObject);
        }
        else if (bullet is MirrorBullet)
        {
            gameObject.AddComponent<Mirror>();
            Destroy(this);
        }
    }
}
