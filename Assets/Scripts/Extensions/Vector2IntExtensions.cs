using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2IntExtensions
{
	public static bool IsInSquareArea(this Vector2Int a, Vector2Int b, int size)
	{
        bool xdiff = Mathf.Abs(a.x - b.x) <= size;
        bool ydiff = Mathf.Abs(a.y - b.y) <= size;
        return (xdiff && !ydiff) || (!xdiff && ydiff);
	}

    public static int ManhattanDistance(this Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}