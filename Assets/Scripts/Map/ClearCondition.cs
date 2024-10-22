﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ClearCondition
{
    public ClearType type;
    public int count;
    public int goal;
    public bool isDone = false;
    public ClearStatusUI assignedClearUI;

    public ClearCondition(ClearType _type, int _goal)
    {
        type = _type;
        goal = _goal;
        count = 0;
    }

    public void IsDone(int _count = 0, int _goal = 0)
    {
        count += _count;
        goal += _goal;
        if (((type == ClearType.White || type == ClearType.Black || type == ClearType.NPlayer) ? goal == count : goal <= count) && !isDone)
        {
            GameManager.inst.clearCounter--;
            isDone = true;
            if (GameManager.inst.clearCounter == 0)
                GameManager.inst.StartCoroutine(GameManager.inst.ClearStage());
        }
        else if (((type == ClearType.White || type == ClearType.Black || type == ClearType.NPlayer) ? goal != count : goal > count) && isDone)
        {
            GameManager.inst.clearCounter++;
            isDone = false;
        }
        assignedClearUI.RefreshClearCondition();
    }
}
