using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Map : MonoBehaviour
{
    [Header("Map Data")]
    public int maxMapSize;
    public Vector2Int maxBorder, minBorder;
    public Dictionary<Vector2Int, Floor> floorGrid;
    public Dictionary<Vector2, Wall> wallGrid;
    public Dictionary<Vector2, IObject> objectGrid;
    public GameObject floors, walls, objects;

    [Header("Stage Data")]
    public List<Floor> startFloors;
    public List<BulletCode> initialBullets;
    public string comments;
    public List<ClearCondition> clearConditions;

    Vector2 ConvertVector2(Vector2Int vector) { return new Vector2(vector.x, vector.y); }
    Vector2Int ConvertVector2(Vector2 vector) { return new Vector2Int((int)vector.x, (int)vector.y); }

    /// <summary>
    /// Get floor at position.
    /// </summary>
    /// <param name="pos">Position of floor.</param>
    /// <returns></returns>
    public Floor GetFloorAtPos(Vector2Int pos)
    {
        return floorGrid.ContainsKey(pos) ? floorGrid[pos] : null;
    }
    public Floor GetFloorAtPos(int x, int y)
    {
        return GetFloorAtPos(new Vector2Int(x, y));
    }
    /// <summary>
    /// Get floor at position.
    /// </summary>
    /// <param name="pos">Position of wall.</param>
    /// <returns></returns>
    public Wall GetWallAtPos(Vector2 pos)
    {
        return wallGrid.ContainsKey(pos) ? wallGrid[pos] : null;
    }
    /// <summary>
    /// Get object at position.
    /// </summary>
    /// <param name="pos">Position of object.</param>
    /// <returns></returns>
    public IObject GetObjectAtPos(Vector2 pos)
    {
        return objectGrid.ContainsKey(pos) ? objectGrid[pos] : null;
    }
    /// <summary>
    /// Create floor at position.
    /// </summary>
    /// <param name="pos">Position of floor.</param>
    public void CreateFloor(Vector2Int pos, bool isGoal = false)
    {
        if ((pos.x >= 0 ? (pos.x > maxMapSize / 2) : (pos.x < -maxMapSize / 2)) || (pos.y >= 0 ? (pos.y > maxMapSize / 2) : (pos.y < -maxMapSize / 2)))
        {
            Debug.Log("Input size exceeds map's max size.");
            return;
        }
        if (!floorGrid.ContainsKey(pos))
        {
            floorGrid.Add(pos, Instantiate(MapManager.inst.floor, new Vector3(pos.x, 0, pos.y), Quaternion.identity, floors.transform).GetComponent<Floor>());
            floorGrid[pos].mapPos = pos;
            floorGrid[pos].isGoalFloor = isGoal;
            floorGrid[pos].RefreshGoal();
            if (pos.x > maxBorder.x) maxBorder.x = pos.x;
            else if (pos.x < minBorder.x) minBorder.x = pos.x;
            if (pos.y > maxBorder.y) maxBorder.y = pos.y;
            else if (pos.y < minBorder.y) minBorder.y = pos.y;
            if (GameManager.aFloor >= 0 && isGoal)
                clearConditions[GameManager.aFloor].IsDone(0, 1);
            StartCoroutine(MapManager.inst.Rebaker());
        }
        else
        {
            //Debug.Log("Floor already exists at : (" + pos.x + ", " + pos.y + ")");
            floorGrid[pos].isGoalFloor = isGoal;
            floorGrid[pos].RefreshGoal();   
        }
    }

    /// <summary>
    /// Remove floor at position.
    /// </summary>
    /// <param name="pos">Position of floor.</param>
    public void RemoveFloor(Vector2Int pos)
    {
        if ((pos.x >= 0 ? (pos.x > maxMapSize / 2) : (pos.x < -maxMapSize / 2)) || (pos.y >= 0 ? (pos.y > maxMapSize / 2) : (pos.y < -maxMapSize / 2)))
        {
            Debug.Log("Input size exceeds map's max size.");
            return;
        }
        if (floorGrid.ContainsKey(pos))
        {
            if (GameManager.aFloor >= 0 && floorGrid[pos].isGoalFloor)
                clearConditions[GameManager.aFloor].IsDone(0, -1);
            Destroy(floorGrid[pos].gameObject);
            floorGrid.Remove(pos);
            StartCoroutine(MapManager.inst.Rebaker());
        }
        else
            Debug.Log("Floor doesn't exists at : (" + pos.x + ", " + pos.y + ")");
    }
    /// <summary>
    /// Create wall at position.
    /// </summary>
    /// <param name="pos">Position of wall.</param>
    /// <param name="wallType">Type of wall.</param>
    public void CreateWall(Vector2 pos, WallType wallType, bool isBreak = true)
    {
        if (((int)pos.x >= 0 ? ((int)pos.x > maxMapSize / 2) : ((int)pos.x < -maxMapSize / 2)) || ((int)pos.y >= 0 ? ((int)pos.y > maxMapSize / 2) : ((int)pos.y < -maxMapSize / 2)))
        {
            Debug.Log("Input size exceeds map's max size.");
            return;
        }
        if (Mathf.Abs(pos.x * 10) % 5 != 0 || Mathf.Abs(pos.y * 10) % 5 != 0 || (Mathf.Abs(pos.x * 10) % 10 == 5 && Mathf.Abs(pos.y * 10) % 10 == 5) || (Mathf.Abs(pos.x * 10) % 10 != 5 && Mathf.Abs(pos.y * 10) % 10 != 5))
        {
            Debug.Log("Inappropriate position of wall.");
            return;
        }
        if (!wallGrid.ContainsKey(pos))
        {
            if (wallType == WallType.Normal)
                wallGrid.Add(pos, Instantiate(MapManager.inst.normalWall, new Vector3(pos.x, 0, pos.y), Quaternion.identity, walls.transform).GetComponent<Wall>());
            else if (wallType == WallType.Mirror)
                wallGrid.Add(pos, Instantiate(MapManager.inst.mirror, new Vector3(pos.x, 0, pos.y), Quaternion.identity, walls.transform).GetComponent<Wall>());
            wallGrid[pos].mapPos = pos;
            wallGrid[pos].type = wallType;
            if (Mathf.Abs(pos.x * 10) % 10 == 5)
                wallGrid[pos].transform.eulerAngles = new Vector3(0, 90, 0);
            else if (Mathf.Abs(pos.y * 10) % 10 == 5)
                wallGrid[pos].transform.eulerAngles = new Vector3(0, 0, 0);
            StartCoroutine(MapManager.inst.Rebaker());
        }
        else
        {
            Debug.Log("Wall already exists at : " + pos);
            if (wallGrid[pos].type == WallType.Normal && wallType == WallType.Mirror) // change to Mirror
            {
                MapManager.inst.currentMap.ChangeWall(pos, WallType.Mirror, isBreak);
            }
            else if (wallGrid[pos].type == WallType.Mirror && wallType == WallType.Normal)
            {
                RemoveWall(pos);
                CreateWall(pos, WallType.Normal);
            }
        }
    }
    /// <summary>
    /// Create walls from two floors, toward dir's direction. 
    /// </summary>
    /// <param name="pos">Start position of wall.</param>
    /// <param name="dir">Direction of walls.</param>
    /// <param name="length">Amount of walls.</param>
    /// <param name="wallType">Type of walls.</param>
    public void CreateWall(Vector2 pos, Vector2 dir, int length, WallType wallType)
    {
        Vector2 wallPos = pos;
        for (int i = 0; i < length; i++)
        {
            CreateWall(wallPos, wallType);
            wallPos += new Vector2((int)dir.x, (int)dir.y);
        }
    }
    /// <summary>
    /// Change normal wall at position to wall having type.
    /// </summary>
    /// <param name="pos">Position of wall.</param>
    public void ChangeWall(Vector2 pos, WallType type, bool isBreak = true)
    {
        if (((int)pos.x >= 0 ? ((int)pos.x > maxMapSize / 2) : ((int)pos.x < -maxMapSize / 2)) || ((int)pos.y >= 0 ? ((int)pos.y > maxMapSize / 2) : ((int)pos.y < -maxMapSize / 2)))
        {
            Debug.Log("Input size exceeds map's max size.");
            return;
        }
        if (Mathf.Abs(pos.x * 10) % 5 != 0 || Mathf.Abs(pos.y * 10) % 5 != 0 || (Mathf.Abs(pos.x * 10) % 10 == 5 && Mathf.Abs(pos.y * 10) % 10 == 5) || (Mathf.Abs(pos.x * 10) % 10 != 5 && Mathf.Abs(pos.y * 10) % 10 != 5))
        {
            Debug.Log("Inappropriate position of wall.");
            return;
        }
        if (wallGrid.ContainsKey(pos))
        {
            if (isBreak) (wallGrid[pos] as IBreakable).Break();
            RemoveWall(pos);
            Wall wallObj;
            switch(type)
            {
                case WallType.Mirror:
                    wallObj = Instantiate(MapManager.inst.mirror, new Vector3(pos.x, 0, pos.y), Quaternion.identity, walls.transform).GetComponent<Wall>();
                    break;
                case WallType.Normal:
                    wallObj = Instantiate(MapManager.inst.normalWall, new Vector3(pos.x, 0, pos.y), Quaternion.identity, walls.transform).GetComponent<Wall>();
                    break;
                case WallType.Glass:
                    wallObj = Instantiate(MapManager.inst.glass, new Vector3(pos.x, 0, pos.y), Quaternion.identity, walls.transform).GetComponent<Wall>();
                    break;
                default:
                    Debug.LogError("Unpredictable wall type");
                    return;
            }
            wallGrid.Add(pos, wallObj);
            wallGrid[pos].mapPos = pos;
            wallGrid[pos].type = type;
            if (Mathf.Abs(pos.x * 10) % 10 == 5)
                wallGrid[pos].transform.eulerAngles = new Vector3(0, 90, 0);
            else if (Mathf.Abs(pos.y * 10) % 10 == 5)
                wallGrid[pos].transform.eulerAngles = new Vector3(0, 0, 0);
            StartCoroutine(MapManager.inst.Rebaker());
        }
        else Debug.Log("Wall already exists at : " + pos);
    }
    /// <summary>
    /// Remove wall at position.
    /// </summary>
    /// <param name="pos">Position of wall.</param>
    public void RemoveWall(Vector2 pos)
    {
        if (wallGrid.ContainsKey(pos))
        {
            Destroy(wallGrid[pos].gameObject);
            wallGrid.Remove(pos);
            StartCoroutine(MapManager.inst.Rebaker());
        }
        else Debug.Log("Wall doesn't exists between : " + pos);
    }
    /// <summary>
    /// Create object at position.
    /// </summary>
    /// <param name="pos">Position of object.</param>
    /// <param name="objType">Type of object.</param>
    public void CreateObject(Vector2 pos, ObjType objType, float angle, bool isWhite = true)
    {
        if ((pos.x >= 0 ? (pos.x > maxMapSize / 2) : (pos.x < -maxMapSize / 2)) || (pos.y >= 0 ? (pos.y > maxMapSize / 2) : (pos.y < -maxMapSize / 2)))
        {
            Debug.Log("Input size exceeds map's max size.");
            return;
        }
        if (!objectGrid.ContainsKey(pos))
        {
            switch (objType)
            {
                case ObjType.Briefcase:
                    objectGrid.Add(pos, Instantiate(MapManager.inst.briefCase, new Vector3(pos.x, 0.5f, pos.y), Quaternion.identity, objects.transform).GetComponent<IObject>());
                    (objectGrid[pos] as Briefcase).Init(pos);
                    break;
                case ObjType.Camera:
                    objectGrid.Add(pos, Instantiate(MapManager.inst.cameraTurret, new Vector3(pos.x, 0, pos.y), Quaternion.identity, objects.transform).GetComponent<IObject>());
                    objectGrid[pos].Init(pos);
                    break;
                case ObjType.Mannequin:
                    objectGrid.Add(pos, Instantiate(MapManager.inst.mannequin, new Vector3(pos.x, 0, pos.y), Quaternion.identity, objects.transform).GetComponent<IObject>());
                    (objectGrid[pos] as Mannequin).Init(pos, isWhite);
                    break;
            }
            StartCoroutine(MapManager.inst.Rebaker());
        }
        else Debug.Log("Object already exists at : (" + pos.x + ", " + pos.y + ")");
    }

    public void CreateObject(Vector2 pos, ObjType objType, float angle, BulletCode _dropBullet)
    {
        CreateObject(pos, objType, angle);
        GetObjectAtPos(pos).GetObject().GetComponent<Briefcase>().SetBullet(_dropBullet);
    }
    /// <summary>
    /// Remove Object at position.
    /// </summary>
    /// <param name="pos">Position of object.</param>
    public void RemoveObject(Vector2 pos)
    {
        if (objectGrid.ContainsKey(pos))
        {
            switch (objectGrid[pos].GetType())
            {
                case ObjType.Camera:
                    if (GameManager.aTurret >= 0)
                        clearConditions[GameManager.aTurret].IsDone(0, -1);
                    PlayerController.inst.OnPlayerMove -= objectGrid[pos].GetObject().GetComponent<IPlayerInteractor>().Interact;
                    break;
                case ObjType.Mannequin:
                    if (objectGrid[pos].GetObject().GetComponent<Mannequin>().isWhite && GameManager.white >= 0)
                        clearConditions[GameManager.white].IsDone(-1);
                    else if (!objectGrid[pos].GetObject().GetComponent<Mannequin>().isWhite && GameManager.black >= 0)
                        clearConditions[GameManager.black].IsDone(-1);
                    break;
                case ObjType.Briefcase:
                    if (GameManager.aCase >= 0)
                        clearConditions[GameManager.aCase].IsDone(0, -1);
                    PlayerController.inst.OnPlayerMove -= objectGrid[pos].GetObject().GetComponent<IPlayerInteractor>().Interact;
                    break;
                default:
                    Debug.Log("[ERR] 병신아");
                    break;
            }
            Destroy(objectGrid[pos].GetObject());
            objectGrid.Remove(pos);
            floorGrid[ConvertVector2(pos)].objOnFloor = null;
            StartCoroutine(MapManager.inst.Rebaker());
        }
        else Debug.Log("Object doesn't exists between : " + pos);
    }

    public void InitiateMap()
    {
        floorGrid = new Dictionary<Vector2Int, Floor>();
        wallGrid = new Dictionary<Vector2, Wall>();
        objectGrid = new Dictionary<Vector2, IObject>();
        startFloors = new List<Floor>();
    }
}
