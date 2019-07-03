using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelector : MonoBehaviour
{
    public Text stageIndex;

    public void ChangeStage(int i)
    {
        if(i < 0 && GameManager.inst.currentStage == 0) GameManager.inst.currentStage = GameManager.inst.totalStageCount - 1;
        else GameManager.inst.currentStage = (GameManager.inst.currentStage + i) % GameManager.inst.totalStageCount;
        stageIndex.text = "Stage : " + GameManager.inst.currentStage;
    }

    public void StartSelectedStage()
    {
        GameManager.inst.StartStage(GameManager.inst.currentStage);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
