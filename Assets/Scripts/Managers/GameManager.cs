﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : SingletonBehaviour<GameManager>
{
    public Material mirrorMaterial;
    public ClearUIGenerator uiGenerator;

    public int[] clearIndex = new int[9];
    public int clearCounter = 0;
    public static int nFloor, nTurret, nCase, nPlayer, aFloor, aTurret, aCase, white, black;

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

    public void ClearStage()
    {
        Debug.Log("Stage Clear!");
    }

    public IEnumerator GameOver()
    {
        Debug.Log("Game Over!");
        StopAllCoroutines();
        yield return new WaitForSeconds(1);
        foreach (GameObject child in MapManager.inst.players)
            Destroy(child);
        Destroy(MapManager.inst.currentMap.gameObject);
    }

    public void StageRestart()
    {
        Debug.Log("Game Restart!");
        GameOver();
        MapManager.inst.LoadMap(MapManager.inst.stage[0]);

    }

    void Awake()
    {
        //Reset clear index to -1.
        for (int i = 0; i < clearIndex.Length; i++) clearIndex[i] = -1;
        nFloor = nTurret = nCase = nPlayer = aFloor = aTurret = aCase = white = black = -1;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!MapManager.inst.isMapEditingOn)
            MapManager.inst.LoadMap(MapManager.inst.stage[0]);
    }
}
