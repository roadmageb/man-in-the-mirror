using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalWall : Wall, IBulletInteractor, IBreakable
{
    [Space(15)]
    public GameObject scatteredWall;

    public void Break()
    {
        Instantiate(scatteredWall, transform.position + new Vector3(0, 0.3f), transform.rotation);
    }

    public void Interact(Bullet bullet)
    {
        if (bullet is MirrorBullet)
        {
            MapManager.inst.currentMap.ChangeWall(mapPos, WallType.Mirror);
        }
    }
}
