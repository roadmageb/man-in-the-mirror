using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Map : MonoBehaviour
{
    [Header("Map Data")]
    public int maxMapSize;
    public Dictionary<Vector2Int, Floor> floorGrid;
    public Dictionary<Vector2, Wall> wallGrid;
    public Dictionary<Vector2Int, IObject> objectGrid;
    public GameObject floors;
    public GameObject walls;
    public GameObject objects;
    public List<Floor> startFloors;
    public List<BulletCode> initialBullets;

    public List<ClearCondition> clearConditions;

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
    public IObject GetObjectAtPos(Vector2Int pos)
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
            if (GameManager.aFloor >= 0 && isGoal)
                clearConditions[GameManager.aFloor].IsDone(0, 1);
            StartCoroutine(MapManager.inst.Rebaker());
        }
        else
            Debug.Log("Floor already exists at : (" + pos.x + ", " + pos.y + ")");
    }
    /// <summary>
    /// Create floor in rectangular area between pos1 and pos2. 
    /// </summary>
    /// <param name="pos1"></param>
    /// <param name="pos2"></param>
    public void CreateFloor(Vector2Int pos1, Vector2Int pos2)
    {
        int xMax = Mathf.Max(pos1.x, pos2.x);
        int yMax = Mathf.Max(pos1.y, pos2.y);
        int xMin = Mathf.Min(pos1.x, pos2.x);
        int yMin = Mathf.Min(pos1.y, pos2.y);
        for (int i = xMin; i <= xMax; i++)
            for (int j = yMin; j <= yMax; j++)
                CreateFloor(new Vector2Int(i, j));
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
    public void CreateWall(Vector2 pos, WallType wallType)
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
            if(wallType == WallType.Normal)
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
            Debug.Log("Wall already exists at : " + pos);
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
    /// Change normal wall at position to mirror.
    /// </summary>
    /// <param name="pos">Position of wall.</param>
    public void ChangeToMirror(Vector2 pos)
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
            RemoveWall(pos);
            wallGrid.Add(pos, Instantiate(MapManager.inst.mirror, new Vector3(pos.x, 0, pos.y), Quaternion.identity, walls.transform).GetComponent<Wall>());
            wallGrid[pos].mapPos = pos;
            wallGrid[pos].type = WallType.Mirror;
            if (Mathf.Abs(pos.x * 10) % 10 == 5)
                wallGrid[pos].transform.eulerAngles = new Vector3(0, 90, 0);
            else if (Mathf.Abs(pos.y * 10) % 10 == 5)
                wallGrid[pos].transform.eulerAngles = new Vector3(0, 0, 0);
            StartCoroutine(MapManager.inst.Rebaker());
        }
        else
            Debug.Log("Wall already exists at : " + pos);
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
        else
            Debug.Log("Wall doesn't exists between : " + pos);
    }
    /// <summary>
    /// Create object at position.
    /// </summary>
    /// <param name="pos">Position of object.</param>
    /// <param name="objType">Type of object.</param>
    public void CreateObject(Vector2Int pos, ObjType objType, bool isWhite = true)
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
                    objectGrid.Add(pos, Instantiate(MapManager.inst.briefCase, new Vector3(pos.x, 0.35f, pos.y), Quaternion.identity, objects.transform).GetComponent<IObject>());
                    if (GameManager.aCase >= 0)
                        clearConditions[GameManager.aCase].IsDone(0, 1);
                    break;
                case ObjType.Camera:
                    objectGrid.Add(pos, Instantiate(MapManager.inst.cameraTurret, new Vector3(pos.x, 0, pos.y), Quaternion.identity, objects.transform).GetComponent<IObject>());
                    if (GameManager.aTurret >= 0)
                        clearConditions[GameManager.aTurret].IsDone(0, 1);
                    break;
                case ObjType.Mannequin:
                    objectGrid.Add(pos, Instantiate(MapManager.inst.mannequins[Random.Range(0, 5)], new Vector3(pos.x, 0, pos.y), Quaternion.identity, objects.transform).GetComponent<IObject>());
                    objectGrid[pos].GetObject().GetComponent<Mannequin>().SetColor(isWhite);
                    break;
            }
            objectGrid[pos].Init(GetFloorAtPos(pos));
            StartCoroutine(MapManager.inst.Rebaker());
        }
        else
            Debug.Log("Object already exists at : (" + pos.x + ", " + pos.y + ")");
    }
    public void CreateObject(Vector2Int pos, ObjType objType, BulletCode _dropBullet)
    {
        CreateObject(pos, objType);
        GetObjectAtPos(pos).GetObject().GetComponent<Briefcase>().dropBullet = _dropBullet;
    }
    /// <summary>
    /// Remove Object at position.
    /// </summary>
    /// <param name="pos">Position of object.</param>
    public void RemoveObject(Vector2Int pos)
    {
        if (objectGrid.ContainsKey(pos))
        {
            if(objectGrid[pos].GetType() == ObjType.Briefcase && GameManager.aCase >= 0)
                clearConditions[GameManager.aCase].IsDone(0, -1);
            else if (objectGrid[pos].GetType() == ObjType.Camera && GameManager.aTurret >= 0)
                clearConditions[GameManager.aTurret].IsDone(0, -1);
            else if(objectGrid[pos].GetType() == ObjType.Mannequin)
            {
                if(objectGrid[pos].GetObject().GetComponent<Mannequin>().isWhite && GameManager.white >= 0)
                    clearConditions[GameManager.white].IsDone(0, -1);
                else if (!objectGrid[pos].GetObject().GetComponent<Mannequin>().isWhite && GameManager.black >= 0)
                    clearConditions[GameManager.black].IsDone(0, -1);
            }
            Destroy(objectGrid[pos].GetObject());
            objectGrid.Remove(pos);
            StartCoroutine(MapManager.inst.Rebaker());
        }
        else
            Debug.Log("Object doesn't exists between : " + pos);
    }

    public void InitiateMap()
    {
        floorGrid = new Dictionary<Vector2Int, Floor>();
        wallGrid = new Dictionary<Vector2, Wall>();
        objectGrid = new Dictionary<Vector2Int, IObject>();
        startFloors = new List<Floor>();
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
