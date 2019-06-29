using System.Collections;
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

    public void SetClearIndex(Map map)
    {
        for (int i = 0; i < 9; i++) clearIndex[i] = -1;
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

    // Start is called before the first frame update
    void Start()
    {
        if (!MapManager.inst.isMapEditingOn)
            MapManager.inst.LoadMap(MapManager.inst.stage[0]);
        else
        {
            for (int i = 0; i < 9; i++) clearIndex[i] = -1;
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
    }
}
