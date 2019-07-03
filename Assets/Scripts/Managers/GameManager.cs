﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonBehaviour<GameManager>
{
    public ClearUIGenerator uiGenerator;

    public int[] clearIndex = new int[9];
    public int clearCounter = 0;
    public static int nFloor, nTurret, nCase, nPlayer, aFloor, aTurret, aCase, white, black;

    /// <summary>
    /// The index of the current stage.
    /// </summary>
    public int currentStage;
    /// <summary>
    /// Max number of stages.
    /// </summary>
    public int totalStageCount;

    public void ResetClearIndex()
    {
        //Reset clear index to -1.
        for (int i = 0; i < clearIndex.Length; i++) clearIndex[i] = -1;
        nFloor = nTurret = nCase = nPlayer = aFloor = aTurret = aCase = white = black = -1;
    }

    //Find and set the index of clear conditions of the map to clear type.
    public void SetClearIndex(Map map)
    {
        foreach (var child in map.clearConditions)
        {
            clearIndex[(int)child.type] = map.clearConditions.IndexOf(child);
            clearCounter++;
        }
        nFloor = clearIndex[(int)ClearType.NFloor];
        nTurret = clearIndex[(int)ClearType.NTurret];
        nCase = clearIndex[(int)ClearType.NCase];
        nPlayer = clearIndex[(int)ClearType.NPlayer];
        aFloor = clearIndex[(int)ClearType.AllFloor];
        aTurret = clearIndex[(int)ClearType.AllTurret];
        aCase = clearIndex[(int)ClearType.AllCase];
        white = clearIndex[(int)ClearType.White];
        black = clearIndex[(int)ClearType.Black];
    }

    public void StartStage(int _stageIndex)
    {
        currentStage = _stageIndex;
        SceneManager.LoadScene("PlayStage");
        MapManager.inst.LoadMap(MapManager.inst.stage[currentStage]);
    }

    public void ClearStage()
    {
        Debug.Log("Stage Clear!");
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");
        StopAllCoroutines();
        foreach (GameObject child in MapManager.inst.players)
            Destroy(child);
        Destroy(MapManager.inst.currentMap.gameObject);
    }

    public IEnumerator RestartStage()
    {
        Debug.Log("Game Restart!");
        yield return new WaitForSeconds(0.5f);
        GameOver();
        MapManager.inst.LoadMap(MapManager.inst.stage[currentStage]);
    }

    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        SetStatic();
        /*if (MapManager.inst.isMapEditingOn)
        {
            //Reset clear index to -1.
            ResetClearIndex();
        }*/
    }
}
