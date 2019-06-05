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
    bool isDone = false;

    public ClearCondition(ClearType _type, int _goal)
    {
        type = _type;
        goal = _goal;
        count = 0;
    }

    public void IsDone()
    {
        if (!isDone)
        {
            if (goal <= count)
            {
                GameManager.inst.clearCounter--;
                GameManager.inst.clearIndex[(int)type] = -1;
                isDone = true;
                if (GameManager.inst.clearCounter == 0)
                    GameManager.inst.ClearStage();
            }
        }
    }
}
