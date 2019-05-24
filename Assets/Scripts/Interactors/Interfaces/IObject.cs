using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjType
{
    NULL,
    Briefcase,
    Camera,
    Mannequin
}

public interface IObject
{
    void Init(Floor floor);
    GameObject GetObject();
    Vector2Int GetPos();
    ObjType GetType();
}
