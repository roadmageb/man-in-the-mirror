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
    GameObject GetObject();
    Vector2 GetPos();
    ObjType GetType();
}
