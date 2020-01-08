using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolveMap
{
    public int maxMapSize;
    public Vector2Int maxBorder, minBorder;
    public Dictionary<Vector2Int, bool> floorGrid;
    public Dictionary<Vector2, WallType> wallGrid;
    public Dictionary<Vector2, ObjType> objectGrid;

    public List<KeyValuePair<Vector2Int, bool>> playerFloors;
    public List<BulletCode> initialBullets;
    public List<ClearCondition> clearConditions;

    public SolveMap()
    {
        floorGrid = new Dictionary<Vector2Int, bool>();
        wallGrid = new Dictionary<Vector2, WallType>();
        objectGrid = new Dictionary<Vector2, ObjType>();
        playerFloors = new List<KeyValuePair<Vector2Int, bool>>();
        initialBullets = new List<BulletCode>();
        clearConditions = new List<ClearCondition>();
    }
    public SolveMap(SolveMap svm)
    {
        floorGrid = new Dictionary<Vector2Int, bool>(svm.floorGrid);
        wallGrid = new Dictionary<Vector2, WallType>(svm.wallGrid);
        objectGrid = new Dictionary<Vector2, ObjType>(svm.objectGrid);
        playerFloors = new List<KeyValuePair<Vector2Int, bool>>(svm.playerFloors);
        initialBullets = new List<BulletCode>(svm.initialBullets);
        clearConditions = new List<ClearCondition>(svm.clearConditions);
    }

}
