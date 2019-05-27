using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2IntExtensions
{
    public static bool IsInAdjacentArea(this Vector2Int a, Vector2Int b, int size)
    {
        return (a - b).magnitude <= size;
    }
    public static int ManhattanDistance(this Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}