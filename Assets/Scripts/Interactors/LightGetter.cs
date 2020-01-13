using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightGetter : MonoBehaviour, IObject
{
    public Vector2 position;
    public float radius;

    #region IObject
    public GameObject GetObject()
    {
        return gameObject;
    }

    public Vector2 GetPos()
    {
        return position;
    }

    public float GetRadius()
    {
        return radius;
    }

    /// <param name="additonal">
    /// <br/>No additional data
    /// </param>
    public void Init(Vector2 pos, params object[] additonal)
    {
        position = pos;
    }

    public object[] GetAdditionals()
    {
        return new object[0];
    }

    ObjType IObject.GetType()
    {
        return ObjType.LightGetter;
    }
    #endregion
}
