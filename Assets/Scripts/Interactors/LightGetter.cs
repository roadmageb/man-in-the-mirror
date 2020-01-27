using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightGetter : MonoBehaviour, IObject
{
    public Vector2 position;
    public float radius;

    public bool isTurnedOn = true;
    public Action<bool> OnStatusChanged;
    public Animation rotateAnim;

    [Space(15)]
    public Material turnOnMat;
    public Material turnOffMat;

    private void Awake()
    {
        SetReceived(false);
    }

    public void SetReceived(bool isReceived)
    {
        if (isTurnedOn != isReceived)
        {
            if (isReceived)
            { // turn on
                rotateAnim.GetComponent<MeshRenderer>().material = turnOnMat;
                rotateAnim.Play();
                isTurnedOn = true;
                OnStatusChanged?.Invoke(isTurnedOn);
            }
            else
            { // turn off
                rotateAnim.GetComponent<MeshRenderer>().material = turnOffMat;
                rotateAnim.Stop();
                isTurnedOn = false;
                OnStatusChanged?.Invoke(isTurnedOn);
            }
        }
    }

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
