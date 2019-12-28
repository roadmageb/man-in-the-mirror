using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageInfo : SingletonBehaviour<StageInfo>
{
    public string selectedStage;
    public string nextStage;
    public MapEditor.MapSaveData testMap;
    public bool isMapEditor = false;
}
