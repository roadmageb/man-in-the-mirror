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
    public Text goalText;
    public Text statusText;
    public Image doneImage;

    public void Init(ClearCondition condition, string tooltip)
    {
        assignedCondition = condition;
        tooltipText.text = tooltip;
        goalText.text = assignedCondition.goal.ToString();
        statusText.text = assignedCondition.count.ToString();
        if (assignedCondition.isDone) doneImage.sprite = fullBox;
        else doneImage.sprite = emptyBox;
    }

    public void RefreshClearCondition()
    {
        goalText.text = assignedCondition.goal.ToString();
        statusText.text = assignedCondition.count.ToString();
        if (assignedCondition.isDone) doneImage.sprite = fullBox;
        else doneImage.sprite = emptyBox;
    }
}
