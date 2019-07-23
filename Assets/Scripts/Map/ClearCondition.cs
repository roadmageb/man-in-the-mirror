using System.Collections;
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
        if (!MapManager.inst.isMapEditingOn)
        {
            count += _count;
            goal += _goal;
            assignedClearUI.RefreshClearCondition();
            if ((type == ClearType.White || type == ClearType.Black) ? goal == count : goal <= count && !isDone)
            {
                GameManager.inst.clearCounter--;
                isDone = true;
                Debug.Log(GameManager.inst.clearCounter);
                if (GameManager.inst.clearCounter == 0)
                    GameManager.inst.StartCoroutine(GameManager.inst.ClearStage());
            }
            else if ((type == ClearType.White || type == ClearType.Black) ? goal != count : goal > count && isDone)
            {
                GameManager.inst.clearCounter++;
                isDone = false;
            }
        }
    }
}
