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

    public Dictionary<Vector2, IObject> adjacentObject;

    [Header("Goal Floor Settings")]
    public SpriteRenderer spriteRenderer;
    public Sprite goalSpriteOn;
    public Sprite goalSpriteOff;

    public void RefreshGoal()
    {
        spriteRenderer.gameObject.SetActive(isGoalFloor);
    }

    // Update is called once per frame
    void Update()
    {
        if (isGoalFloor)
        {
            if (isPlayerOn) spriteRenderer.sprite = goalSpriteOn;
            else spriteRenderer.sprite = goalSpriteOff;
        }
    }
}
