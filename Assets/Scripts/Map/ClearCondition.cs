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
    public bool isDone = false;
    public ClearStatusUI assignedClearUI;

    /*public ClearCondition(ClearType _type, int _goal)
    {
        type = _type;
        goal = _goal;
        count = 0;
    }*/

    public void IsDone(int _count = 0, int _goal = 0)
    {
        count += _count;
        goal += _goal;
        if (goal == count)
        {
            GameManager.inst.clearCounter--;
            isDone = true;
            Debug.Log(GameManager.inst.clearCounter);
            assignedClearUI.RefreshClearCondition();
            if (GameManager.inst.clearCounter == 0)
                GameManager.inst.ClearStage();
        }
        else if (goal != count && isDone)
        {
            GameManager.inst.clearCounter++;
            isDone = false;
            assignedClearUI.RefreshClearCondition();
        }
    }
}
