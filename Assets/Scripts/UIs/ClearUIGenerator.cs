using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearUIGenerator : MonoBehaviour
{
    public GameObject clearUI;

    private int nextx = -175;
    private int nexty = -100;

    // Start is called before the first frame update
    public void GenerateAllClearUI()
    {
        foreach (ClearCondition cond in MapManager.inst.currentMap.clearConditions)
        {
            ClearStatusUI ui = Instantiate(clearUI, transform).GetComponent<ClearStatusUI>();
            ui.GetComponent<RectTransform>().anchoredPosition = new Vector2(nextx, nexty);
            nexty -= 200;

            string str;
            switch(cond.type)
            {
                case ClearType.NFloor:
                    str = "강조된 바닥에 올라서세요.";
                    break;
                case ClearType.NTurret:
                    str = "카메라터렛을 파괴하세요.";
                    break;
                case ClearType.NCase:
                    str = "서류가방을 획득하세요.";
                    break;
                case ClearType.NPlayer:
                    str = "캐릭터를 일정 수로 만드세요.";
                    break;
                case ClearType.AllFloor:
                    str = "모든 강조된 바닥에 올라서세요.";
                    break;
                case ClearType.AllTurret:
                    str = "모든 카메라터렛을 파괴하세요.";
                    break;
                case ClearType.AllCase:
                    str = "모든 서류가방을 획득하세요";
                    break;
                case ClearType.White:
                    str = "흰 마네킹을 만드세요.";
                    break;
                case ClearType.Black:
                    str = "검은 마네킹을 만드세요.";
                    break;
                default:
                    str = "오류입니다.";
                    break;
            }

            ui.Init(cond, str);
            cond.assignedClearUI = ui;
        }
    }
}
