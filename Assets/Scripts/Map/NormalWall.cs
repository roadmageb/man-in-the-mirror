﻿using System.Collections;
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
            MapManager.inst.currentMap.ChangeToMirror(mapPos);
        }
    }
}
