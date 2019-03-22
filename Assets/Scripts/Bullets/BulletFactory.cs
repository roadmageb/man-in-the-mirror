using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletCode
{
    True,
    False,
    Mirror
}

public class BulletFactory
{
	public bool isInit = false;
	public static GameObject truthBulletPrefab = null, fakeBulletPrefab = null, mirrorBulletPrefab = null;

	public BulletFactory()
	{
		if (isInit)
			return;
		truthBulletPrefab = Resources.Load<GameObject>("Prefabs/Bullets/TruthBullet");
		fakeBulletPrefab = Resources.Load<GameObject>("Prefabs/Bullets/FakeBullet");
		mirrorBulletPrefab = Resources.Load<GameObject>("Prefabs/Bullets/MirrorBullet");
	}

	/// <summary>
	/// Returns new bullet gameobject with bullet script on it
	/// </summary>
	/// <param name="bulletCode">Type of bullet that wants to make</param>
	/// <returns></returns>
	public static Bullet MakeBullet(BulletCode bulletCode)
    {
        switch (bulletCode)
        {
            case BulletCode.True:
				return GameObject.Instantiate(truthBulletPrefab).GetComponent<Bullet>();
            case BulletCode.False:
				return GameObject.Instantiate(fakeBulletPrefab).GetComponent<Bullet>();
			case BulletCode.Mirror:
				return GameObject.Instantiate(mirrorBulletPrefab).GetComponent<Bullet>();
			default:
				return null;
        }
    }
}
