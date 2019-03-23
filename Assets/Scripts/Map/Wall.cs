using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    /// <summary>
    /// Position of this floor at the map.
    /// </summary>
    public Vector2 MapPos { get; private set; }

    public void SetMapPos(Vector2 pos)
    {
        MapPos = pos;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
