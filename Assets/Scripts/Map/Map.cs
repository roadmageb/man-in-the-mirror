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

    public Vector2Int ConvertVector2(Vector2 vector) { return new Vector2Int((int)vector.x, (int)vector.y); }

    /// <summary>
    /// Check adjacent floors and do things based on FloorChkmode.
    /// </summary>
    /// <param name="pos">Position of the IObject</param>
    /// <param name="iObject">IObject itself</param>
    /// <param name="mode">Check mode</param>
    /// <returns></returns>
    public bool CheckAdjacentFloor(Vector2 pos, IObject iObject, FloorChkMode mode)
    {
        if((int)pos.x == pos.x && (int)pos.y == pos.y)
        {
            if (GetFloorAtPos(ConvertVector2(pos)))
            {
                if (mode == FloorChkMode.Add) GetFloorAtPos(ConvertVector2(pos)).adjacentObject.Add(pos, iObject);
                else if (mode == FloorChkMode.Remove) GetFloorAtPos(ConvertVector2(pos)).adjacentObject.Remove(pos);
                else return true;
            }
        }
        else if ((int)pos.x != pos.x && (int)pos.y != (int)pos.y)
        {
            if (GetFloorAtPos(new Vector2Int((int)(pos.x + 0.5f), (int)(pos.y + 0.5f))))
            {
                if (mode == FloorChkMode.Add) GetFloorAtPos(new Vector2Int((int)(pos.x + 0.5f), (int)(pos.y + 0.5f))).adjacentObject.Add(pos, iObject);
                else if (mode == FloorChkMode.Remove) GetFloorAtPos(new Vector2Int((int)(pos.x + 0.5f), (int)(pos.y + 0.5f))).adjacentObject.Remove(pos);
                else return true;
            }
            if (GetFloorAtPos(new Vector2Int((int)(pos.x + 0.5f), (int)(pos.y - 0.5f))))
            {
                if (mode == FloorChkMode.Add) GetFloorAtPos(new Vector2Int((int)(pos.x + 0.5f), (int)(pos.y - 0.5f))).adjacentObject.Add(pos, iObject);
                else if (mode == FloorChkMode.Remove) GetFloorAtPos(new Vector2Int((int)(pos.x + 0.5f), (int)(pos.y - 0.5f))).adjacentObject.Remove(pos);
                else return true;
            }
            if (GetFloorAtPos(new Vector2Int((int)(pos.x - 0.5f), (int)(pos.y + 0.5f))))
            {
                if (mode == FloorChkMode.Add) GetFloorAtPos(new Vector2Int((int)(pos.x - 0.5f), (int)(pos.y + 0.5f))).adjacentObject.Add(pos, iObject);
                else if (mode == FloorChkMode.Remove) GetFloorAtPos(new Vector2Int((int)(pos.x - 0.5f), (int)(pos.y + 0.5f))).adjacentObject.Remove(pos);
                else return true;
            }
            if (GetFloorAtPos(new Vector2Int((int)(pos.x - 0.5f), (int)(pos.y - 0.5f))))
            {
                if (mode == FloorChkMode.Add) GetFloorAtPos(new Vector2Int((int)(pos.x - 0.5f), (int)(pos.y - 0.5f))).adjacentObject.Add(pos, iObject);
                else if (mode == FloorChkMode.Remove) GetFloorAtPos(new Vector2Int((int)(pos.x - 0.5f), (int)(pos.y - 0.5f))).adjacentObject.Remove(pos);
                else return true;
            }
        }
        else
        {
            if((int)pos.x != pos.x)
            {
                if (GetFloorAtPos(new Vector2Int((int)(pos.x + 0.5f), (int)pos.y)))
                {
                    if (mode == FloorChkMode.Add) GetFloorAtPos(new Vector2Int((int)(pos.x + 0.5f), (int)pos.y)).adjacentObject.Add(pos, iObject);
                    else if (mode == FloorChkMode.Remove) GetFloorAtPos(new Vector2Int((int)(pos.x + 0.5f), (int)pos.y)).adjacentObject.Remove(pos);
                    else return true;
                }
                if (GetFloorAtPos(new Vector2Int((int)(pos.x - 0.5f), (int)pos.y)))
                {
                    if (mode == FloorChkMode.Add) GetFloorAtPos(new Vector2Int((int)(pos.x - 0.5f), (int)pos.y)).adjacentObject.Add(pos, iObject);
                    else if (mode == FloorChkMode.Remove) GetFloorAtPos(new Vector2Int((int)(pos.x - 0.5f), (int)pos.y)).adjacentObject.Remove(pos);
                    else return true;
                }
            }
            else
            {
                if (GetFloorAtPos(new Vector2Int((int)pos.x, (int)(pos.y + 0.5f))))
                {
                    if (mode == FloorChkMode.Add) GetFloorAtPos(new Vector2Int((int)pos.x, (int)(pos.y + 0.5f))).adjacentObject.Add(pos, iObject);
                    else if (mode == FloorChkMode.Remove) GetFloorAtPos(new Vector2Int((int)pos.x, (int)(pos.y + 0.5f))).adjacentObject.Remove(pos);
                    else return true;
                }
                if (GetFloorAtPos(new Vector2Int((int)pos.x, (int)(pos.y - 0.5f))))
                {
                    if (mode == FloorChkMode.Add) GetFloorAtPos(new Vector2Int((int)pos.x, (int)(pos.y - 0.5f))).adjacentObject.Add(pos, iObject);
                    else if (mode == FloorChkMode.Remove) GetFloorAtPos(new Vector2Int((int)pos.x, (int)(pos.y - 0.5f))).adjacentObject.Remove(pos);
                    else return true;
                }
            }
        }
        return false;
    }

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
            Debug.LogError("Input size exceeds map's max size.");
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
            floorGrid[pos].adjacentObject = new Dictionary<Vector2, IObject>();
            StartCoroutine(MapManager.inst.Rebaker());
        }
        else
        {
            //Debug.LogError("Floor already exists at : (" + pos.x + ", " + pos.y + ")");
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
            Debug.LogError("Input size exceeds map's max size.");
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
            Debug.LogError("Floor doesn't exists at : (" + pos.x + ", " + pos.y + ")");
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
            Debug.LogError("Input size exceeds map's max size.");
            return;
        }
        if (!wallGrid.ContainsKey(pos))
        {
            wallGrid.Add(pos, Instantiate(MapManager.inst.walls[(int)wallType], new Vector3(pos.x, 0, pos.y),
                Quaternion.Euler(0, (int)pos.x != pos.x ? 90 : 0, 0), walls.transform).GetComponent<Wall>());
            wallGrid[pos].mapPos = pos;
            wallGrid[pos].type = wallType;
            StartCoroutine(MapManager.inst.Rebaker());
        }
        else
        {
            Debug.LogError("Wall already exists at : " + pos);
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
    /// Change normal wall at position to wall having type.
    /// </summary>
    /// <param name="pos">Position of wall.</param>
    public void ChangeWall(Vector2 pos, WallType type, bool isBreak = true)
    {
        if (((int)pos.x >= 0 ? ((int)pos.x > maxMapSize / 2) : ((int)pos.x < -maxMapSize / 2)) || ((int)pos.y >= 0 ? ((int)pos.y > maxMapSize / 2) : ((int)pos.y < -maxMapSize / 2)))
        {
            Debug.LogError("Input size exceeds map's max size.");
            return;
        }
        if (wallGrid.ContainsKey(pos))
        {
            if (isBreak) (wallGrid[pos] as IBreakable).Break();
            RemoveWall(pos);

            CreateWall(pos, type, isBreak);
            StartCoroutine(MapManager.inst.Rebaker());
        }
        else Debug.LogError("Wall already exists at : " + pos);
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
        else Debug.LogError("Wall doesn't exists between : " + pos);
    }

    public void CreateObject(Vector2 pos, ObjType objType, float angle, params object[] additional)
    {
        if ((pos.x >= 0 ? (pos.x > maxMapSize / 2) : (pos.x < -maxMapSize / 2)) || (pos.y >= 0 ? (pos.y > maxMapSize / 2) : (pos.y < -maxMapSize / 2)))
        {
            Debug.LogError("Input size exceeds map's max size.");
            return;
        }
        if(!CheckAdjacentFloor(pos, null, FloorChkMode.Check))
        {
            Debug.LogError("Object's position " + pos + " is not possible for " + objType + ".");
            return;
        }
        if (!objectGrid.ContainsKey(pos))
        {
            Vector3 objectPos = new Vector3(pos.x, objType == ObjType.Briefcase ? 0.5f : objType == ObjType.Mannequin ? 0.1f : 0, pos.y);
            objectGrid.Add(pos, Instantiate(objType == ObjType.Mannequin ? MapManager.inst.mannequins[Random.Range(0, MapManager.inst.mannequins.Length)] : 
                MapManager.inst.IObjects[(int)objType], objectPos, Quaternion.Euler(0, angle, 0), objects.transform).GetComponent<IObject>());
            if(additional.Length == 0) objectGrid[pos].Init(pos);
            else objectGrid[pos].Init(pos, additional);
            CheckAdjacentFloor(pos, objectGrid[pos], FloorChkMode.Add);
            StartCoroutine(MapManager.inst.Rebaker());
        }
        else Debug.LogError("Object already exists at : (" + pos.x + ", " + pos.y + ")");
    }
    /// <summary>
    /// Remove Object at position.
    /// </summary>
    /// <param name="pos">Position of object.</param>
    public void RemoveObject(Vector2 pos)
    {
        if (objectGrid.ContainsKey(pos))
        {
            //Debug.Log(pos + " Remove Obj, " + objectGrid[pos].GetType());
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
                    Debug.LogError("[ERR] 병신아");
                    break;
            }
            CheckAdjacentFloor(pos, null, FloorChkMode.Remove);
            Destroy(objectGrid[pos].GetObject());
            objectGrid.Remove(pos);
            floorGrid[ConvertVector2(pos)].objOnFloor = null;
            StartCoroutine(MapManager.inst.Rebaker());
        }
        else
            Debug.LogError("Object doesn't exists at : " + pos);
    }

    public void InitiateMap()
    {
        floorGrid = new Dictionary<Vector2Int, Floor>();
        wallGrid = new Dictionary<Vector2, Wall>();
        objectGrid = new Dictionary<Vector2, IObject>();
        startFloors = new List<Floor>();
    }
}
