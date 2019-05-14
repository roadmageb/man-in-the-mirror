using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalWall : Wall, IBulletInteractor
{
    public void Interact(Bullet bullet)
    {
        if (bullet is TruthBullet)
        {
            Destroy(gameObject);
        }
        else if (bullet is MirrorBullet)
        {
            Mirror mirror = gameObject.AddComponent<Mirror>();
            GetComponent<Renderer>().material = GameManager.inst.mirrorMaterial;
            mirror.mapPos = mapPos;
            mirror.len = len;
            mirror.type = WallType.Mirror;
            Destroy(this);
        }
    }
}
