using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearStatusUI : MonoBehaviour
{
    [Header("gameobjects")]
    public ClearCondition assignedCondition;
    public Sprite emptyBox;
    public Sprite fullBox;
    [Header("inside")]
    public Text tooltipText;
    public Text slashText;
    public Text goalText;
    public Text statusText;
    public Image doneImage;

    public void Init(ClearCondition condition, string tooltip)
    {
        assignedCondition = condition;
        tooltipText.text = tooltip;
        if (condition.type == ClearType.AllCase || condition.type == ClearType.AllFloor || condition.type == ClearType.AllTurret)
        {
            goalText.text = "";
            statusText.text = "";
            slashText.text = (assignedCondition.goal - assignedCondition.count).ToString();
        }
        else
        {
            goalText.text = assignedCondition.goal.ToString();
            statusText.text = assignedCondition.count.ToString();
        }
        if (assignedCondition.isDone) doneImage.sprite = fullBox;
        else doneImage.sprite = emptyBox;
    }

    public void RefreshClearCondition()
    {
        if (assignedCondition.type == ClearType.AllCase || assignedCondition.type == ClearType.AllFloor || assignedCondition.type == ClearType.AllTurret)
        {
            goalText.text = "";
            statusText.text = "";
            slashText.text = (assignedCondition.goal - assignedCondition.count).ToString();
        }
        else
        {
            goalText.text = assignedCondition.goal.ToString();
            statusText.text = assignedCondition.count.ToString();
        }
        if (assignedCondition.isDone) doneImage.sprite = fullBox;
        else doneImage.sprite = emptyBox;
    }
}
