using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClearType
{
    NFloor,
    NTurret,
    NCase,
    NPlayer,
    AllFloor,
    AllTurret,
    AllCase,
    White,
    Black
}

[System.Serializable]
public class ClearCondition
{
    public ClearType type;
    public int count;
    public int goal;

    public ClearCondition(ClearType _type, int _goal)
    {
        type = _type;
        goal = _goal;
        count = 0;
    }

    public bool IsDone()
    {
        if (goal <= count)
        {
            GameManager.inst.clearCounter--;
            GameManager.inst.clearIndex[(int)type] = -1;
            return true;
        }
        else return false;
    }
}
