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
	private GameObject truthBulletPrefab = null, fakeBulletPrefab = null, mirrorBulletPrefab = null;

	public BulletFactory(GameObject truthBullet, GameObject fakeBullet, GameObject mirrorBullet)
	{
		truthBulletPrefab = truthBullet;
		fakeBulletPrefab = fakeBullet;
		mirrorBulletPrefab = mirrorBullet;
	}

	/// <summary>
	/// Returns new bullet gameobject with bullet script on it
	/// </summary>
	/// <param name="bulletCode">Type of bullet that wants to make</param>
	/// <returns></returns>
	public Bullet MakeBullet(BulletCode bulletCode)
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
