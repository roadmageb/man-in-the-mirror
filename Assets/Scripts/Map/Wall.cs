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
    public Vector2Int ldPos // left down pos
    {
        get { return new Vector2Int((int)mapPos.x, (int)mapPos.y); }
    }
    public Vector2Int rdPos // right down pos
    {
        get { return ldPos + (dir ? new Vector2Int(len, 0) : new Vector2Int(0, len)); }
    }
    public bool dir // false: ver, true: hor
    {
        get { return (int)(transform.rotation.eulerAngles.y / 90) % 2 != 1; }
    }
    public int len = 1; // length of wall
    public WallType type;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
