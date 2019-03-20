using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletCode
{
    True,
    False,
    Mirror
}

public static class BulletFactory
{
    /// <summary>
    /// Returns new bullet gameobject with bullet script on it
    /// </summary>
    /// <param name="bulletCode">Type of bullet that wants to make</param>
    /// <returns></returns>
    public static GameObject MakeBullet(BulletCode bulletCode)
    {
        switch (bulletCode)
        {
            case BulletCode.True:
                break;
            case BulletCode.False:
                break;
            case BulletCode.Mirror:
                break;
            default:
                break;
        }
        return null;
    }
}
