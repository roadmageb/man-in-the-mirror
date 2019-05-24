using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Map : MonoBehaviour
{
    public int testInputSizeX, testInputSizeY;
    public int maxMapSize;
    public Dictionary<Vector2Int, Floor> floorGrid;
    public Dictionary<Vector2, Wall> wallGrid;
    public Dictionary<Vector2Int, IObject> objectGrid;
    public GameObject floors;
    public GameObject walls;
    public GameObject objects;
    public List<Floor> startFloors;

    /// <summary>
    /// Get floor at position.
    /// </summary>
    /// <param name="pos">Position of floor.</param>
    /// <returns></returns>
    public Floor GetFloorAtPos(Vector2Int pos)
    {
        return floorGrid.ContainsKey(pos) ? floorGrid[pos] : null;
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
    public void CreateFloor(Vector2Int pos)
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
    public void CreateObject(Vector2Int pos, ObjType objType)
    {
        if ((pos.x >= 0 ? (pos.x > maxMapSize / 2) : (pos.x < -maxMapSize / 2)) || (pos.y >= 0 ? (pos.y > maxMapSize / 2) : (pos.y < -maxMapSize / 2)))
        {
            Debug.Log("Input size exceeds map's max size.");
            return;
        }
        if (!objectGrid.ContainsKey(pos))
        {
            objectGrid.Add(pos, Instantiate(MapManager.inst.objects[(int)objType - 1], new Vector3(pos.x, 0, pos.y), Quaternion.identity, objects.transform).GetComponent<IObject>());
            objectGrid[pos].Init(GetFloorAtPos(pos));
            StartCoroutine(MapManager.inst.Rebaker());
        }
        else
            Debug.Log("Object already exists at : (" + pos.x + ", " + pos.y + ")");
    }
    /// <summary>
    /// Remove Object at position.
    /// </summary>
    /// <param name="pos">Position of object.</param>
    public void RemoveObject(Vector2Int pos)
    {
        if (objectGrid.ContainsKey(pos))
        {
            Destroy(objectGrid[pos].GetObject());
            objectGrid.Remove(pos);
            StartCoroutine(MapManager.inst.Rebaker());
        }
        else
            Debug.Log("Object doesn't exists between : " + pos);
    }

    private void LoadObjects()
    {
        floorGrid = new Dictionary<Vector2Int, Floor>();
        wallGrid = new Dictionary<Vector2, Wall>();
        objectGrid = new Dictionary<Vector2Int, IObject>();
        for (int i = 0; i < floors.transform.childCount; i++)
        {
            Floor floor = floors.transform.GetChild(i).GetComponent<Floor>();
            floorGrid.Add(floor.mapPos, floor);
        }
        for (int i = 0; i < walls.transform.childCount; i++)
        {
            Wall wall = walls.transform.GetChild(i).GetComponent<Wall>();
            wallGrid.Add(wall.mapPos, wall);
        }
        for (int i = 0; i < objects.transform.childCount; i++)
        {
            IObject iObject = objects.transform.GetChild(i).GetComponent<IObject>();
            objectGrid.Add(iObject.GetPos(), iObject);
        }
    }

    public void InitiateMap()
    {
        floorGrid = new Dictionary<Vector2Int, Floor>();
        wallGrid = new Dictionary<Vector2, Wall>();
        objectGrid = new Dictionary<Vector2Int, IObject>();
        startFloors = new List<Floor>();
    }

    private void Awake()
    {
        LoadObjects();
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
