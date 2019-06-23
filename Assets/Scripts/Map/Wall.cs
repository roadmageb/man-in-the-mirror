using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WallType
{
    NULL,
    Normal,
    Mirror
}

public class Wall : MonoBehaviour
{
    /// <summary>
    /// Position of this floor at the map.
    /// </summary>
    public Vector2 mapPos;
    public Vector2 ldPos // left down pos
    {
        get { return new Vector2(dir ? mapPos.x - 0.5f : mapPos.x , !dir ? mapPos.y - 0.5f : mapPos.y); }
    }
    public Vector2 rdPos // right down pos
    {
        get { return ldPos + (dir ? new Vector2(len, 0) : new Vector2(0, len)); }
    }
    public bool dir // false: ver, true: hor
    {
        get { return (int)(transform.rotation.eulerAngles.y / 90) % 2 != 1; }
    }
    public int len = 1; // length of wall
    public WallType type;
}
