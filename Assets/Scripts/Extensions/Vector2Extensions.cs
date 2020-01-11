using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2Extensions
{
    public static bool IsInAdjacentArea(this Vector2 a, Vector2 b, int size)
    {
        return (a - b).magnitude <= size;
    }
    public static float ManhattanDistance(this Vector2 a, Vector2 b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}