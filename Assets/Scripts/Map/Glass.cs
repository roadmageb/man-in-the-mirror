using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glass : Wall, IBulletInteractor, IBreakable
{
    [Space(15)]
    public GameObject scatteredGlass;

    public void Break()
    {
        Instantiate(scatteredGlass, transform.position, transform.rotation);
        MapManager.inst.currentMap.RemoveWall(this.mapPos);
    }

    public void Interact(Bullet bullet)
    {
        if (bullet is FakeBullet)
        {
            // Change Glass to NormalWall
            MapManager.inst.currentMap.ChangeWall(mapPos, WallType.Normal);
        }
    }
}
