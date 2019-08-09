using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearStatusUI : MonoBehaviour
{
    [Header("gameobjects")]
    public ClearCondition assignedCondition;
    public Sprite notCleared;
    public Color notClearedTooltipColor;
    public Color notClearedColor;
    public Sprite whenCleared;
    public Color clearColor;
    [Header("inside")]
    public Image condImage;
    public Text tooltipText;
    public Text checkerText;
    public Text counterText;

    public void Init(ClearCondition condition, string tooltip)
    {
        assignedCondition = condition;
        tooltipText.text = tooltip;
        if (condition.type == ClearType.AllCase || condition.type == ClearType.AllFloor || condition.type == ClearType.AllTurret)
        {
            checkerText.text = "잔여";
            counterText.text = (condition.goal - condition.count).ToString();
        }
        else
        {
            checkerText.text = "현재";
            counterText.text = condition.count.ToString();
        }
        if (assignedCondition.isDone)
        {
            condImage.sprite = whenCleared;
            tooltipText.color = clearColor;
            checkerText.color = clearColor;
            counterText.color = clearColor;
        }
        else
        {
            condImage.sprite = notCleared;
            tooltipText.color = notClearedTooltipColor;
            counterText.color = notClearedColor;
            checkerText.color = notClearedColor;
        }
    }

    public void RefreshClearCondition()
    {
        if (assignedCondition.type == ClearType.AllCase || assignedCondition.type == ClearType.AllFloor || assignedCondition.type == ClearType.AllTurret)
        {
            counterText.text = (assignedCondition.goal - assignedCondition.count).ToString();
        }
        else
        {
            counterText.text = assignedCondition.count.ToString();
        }
        if (assignedCondition.isDone)
        {
            condImage.sprite = whenCleared;
            tooltipText.color = clearColor;
            checkerText.color = clearColor;
            counterText.color = clearColor;
        }
        else
        {
            condImage.sprite = notCleared;
            tooltipText.color = notClearedTooltipColor;
            counterText.color = notClearedColor;
            checkerText.color = notClearedColor;
        }
    }
}
