using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObject
{
    void Init(Floor floor);
    GameObject GetObject();
    Vector2 GetPos();
    ObjType GetType();
    float GetRadius();
}
