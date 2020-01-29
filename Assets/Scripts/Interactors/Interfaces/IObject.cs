using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObject
{
    void Init(Vector2 pos, params object[] additonal);
    object[] GetAdditionals();
    GameObject GetObject();
    Vector2 GetPos();
    ObjType GetType();
    float GetRadius();
    int GetMirrorAble();
}
