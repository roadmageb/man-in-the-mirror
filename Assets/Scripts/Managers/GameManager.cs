using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : SingletonBehaviour<GameManager>
{
    public Material mirrorMaterial;

    public int[] clearIndex = new int[9];
    public int clearCounter = 0;

    private void GetClearIndex(Map map)
    {
        for (int i = 0; i < 9; i++) clearIndex[i] = -1;
        foreach (var child in map.clearConditions)
        {
            clearIndex[(int)child.type] = map.clearConditions.IndexOf(child);
            clearCounter++;
        }
    }
}
