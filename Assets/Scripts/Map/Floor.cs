using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    /// <summary>
    /// Position of this floor at the map.
    /// </summary>
    public Vector2Int mapPos;
    public bool isGoalFloor = false;
    public IObject objOnFloor = null;
    public bool isPlayerOn = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
