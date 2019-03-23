using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Map : MonoBehaviour
{
    public int testInputSizeX, testInputSizeY;
    public int maxMapSize;
    private Dictionary<Vector2Int, Floor> floorGrid;
    private Dictionary<Vector2, Wall> wallGrid;
    public GameObject floors;
    public GameObject walls;
    public Floor startFloor;

    /// <summary>
    /// Get floor at position.
    /// </summary>
    /// <param name="x">X position of floor.</param>
    /// <param name="y">Y position of floor.</param>
    /// <returns></returns>
    public Floor GetFloorAtPos(int x, int y)
    {
        if ((x >= 0 ? x > maxMapSize / 2 - 1 : x < maxMapSize / 2) || (y >= 0 ? y > maxMapSize / 2 - 1 : y < maxMapSize / 2))
        {
            Debug.Log("Input size exceeds map's max size.");
            return null;
        }
        Vector2Int floorPos = new Vector2Int(x, y);
        return floorGrid.ContainsKey(floorPos) ? floorGrid[floorPos] : null;
    }
    /// <summary>
    /// Get floor at position.
    /// </summary>
    /// <param name="pos">Position of floor.</param>
    /// <returns></returns>
    public Floor GetFloorAtPos(Vector2Int pos)
    {
        return GetFloorAtPos(pos.x, pos.y);
    }
    /// <summary>
    /// Get floor at position.
    /// </summary>
    /// <param name="x">X position of floor.</param>
    /// <param name="y">Y position of floor.</param>
    /// <returns></returns>
    public Wall GetWallAtPos(Floor floor1, Floor floor2)
    {
        Vector2 wallPos = (Vector2)(floor1.mapPos + floor2.mapPos) / 2;
        return wallGrid.ContainsKey(wallPos) ? wallGrid[wallPos] : null;
    }
    /// <summary>
    /// Create floor at position.
    /// </summary>
    /// <param name="x">X position of floor.</param>
    /// <param name="y">Y position of floor.</param>
    public void CreateFloor(int x, int y)
    {
        if ((x >= 0 ? (x > maxMapSize / 2 - 1) : (x < -maxMapSize / 2)) || (y >= 0 ? (y > maxMapSize / 2 - 1) : (y < -maxMapSize / 2)))
        {
            Debug.Log("Input size exceeds map's max size.");
            return;
        }
        Vector2Int floorPos = new Vector2Int(x, y);
        if (!floorGrid.ContainsKey(floorPos))
        {
            floorGrid.Add(floorPos, Instantiate(MapManager.inst.floor, new Vector3(floorPos.x, 0, floorPos.y), Quaternion.identity, floors.transform).GetComponent<Floor>());
            floorGrid[floorPos].SetmapPos(floorPos);
            StartCoroutine(MapManager.inst.Rebaker());
        }
        else
            Debug.Log("Floor already exists at : (" + x + ", " + y + ")");
    }
    /// <summary>
    /// Create floor at position.
    /// </summary>
    /// <param name="pos">Position of floor.</param>
    public void CreateFloor(Vector2Int pos)
    {
        CreateFloor(pos.x, pos.y);
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
                CreateFloor(i, j);
    }
    /// <summary>
    /// Remove floor at position.
    /// </summary>
    /// <param name="x">X position of floor.</param>
    /// <param name="y">Y position of floor.</param>
    public void RemoveFloor(int x, int y)
    {
        if ((x >= 0 ? x > maxMapSize / 2 - 1 : x < maxMapSize / 2) || (y >= 0 ? y > maxMapSize / 2 - 1 : y < maxMapSize / 2))
        {
            Debug.Log("Input size exceeds map's max size.");
            return;
        }
        Vector2Int floorPos = new Vector2Int(x, y);
        if (floorGrid.ContainsKey(floorPos))
        {
            Destroy(floorGrid[floorPos].gameObject);
            floorGrid.Remove(floorPos);
            StartCoroutine(MapManager.inst.Rebaker());
        }
        else
            Debug.Log("Floor doesn't exists at : (" + x + ", " + y + ")");
    }
    /// <summary>
    /// Remove floor at position.
    /// </summary>
    /// <param name="pos">Position of floor.</param>
    public void RemoveFloor(Vector2Int pos)
    {
        RemoveFloor(pos.x, pos.y);
    }
    /// <summary>
    /// Create wall between two floors.
    /// </summary>
    /// <param name="floor1"></param>
    /// <param name="floor2"></param>
    public void CreateWall(Floor floor1, Floor floor2)
    {
        Vector2 wallPos = (Vector2)(floor1.mapPos + floor2.mapPos) / 2;
        if (!wallGrid.ContainsKey(wallPos))
        {
            wallGrid.Add(wallPos, Instantiate(MapManager.inst.wall, new Vector3(wallPos.x, 0, wallPos.y), Quaternion.identity, walls.transform).GetComponent<Wall>());
            wallGrid[wallPos].SetmapPos(wallPos);
            wallGrid[wallPos].transform.LookAt(floor1.transform);
            StartCoroutine(MapManager.inst.Rebaker());
        }
        else
            Debug.Log("Wall already exists between : " + floor1.mapPos + ", " + floor2.mapPos);
    }
    /// <summary>
    /// Create walls from two floors, toward dir's direction. 
    /// </summary>
    /// <param name="floor1"></param>
    /// <param name="floor2"></param>
    /// <param name="dir">Direction you want to create walls.</param>
    /// <param name="length">Amount of walls you want to create.</param>
    public void CreateWall(Floor floor1, Floor floor2, Vector2 dir, int length)
    {
        Vector2Int floor1Pos = floor1.mapPos;
        Vector2Int floor2Pos = floor2.mapPos;
        for (int i = 0; i < length; i++)
        {
            if(GetFloorAtPos(floor1Pos) == null || GetFloorAtPos(floor2Pos) == null)
            {
                Debug.Log("Floor doesn't exists.\nMaybe length you input exceeded current floors' length.");
                return;
            }
            CreateWall(GetFloorAtPos(floor1Pos), GetFloorAtPos(floor2Pos));
            floor1Pos += new Vector2Int((int)dir.x, (int)dir.y);
            floor2Pos += new Vector2Int((int)dir.x, (int)dir.y);
        }
    }
    /// <summary>
    /// Remove wall between two floors.
    /// </summary>
    /// <param name="floor1"></param>
    /// <param name="floor2"></param>
    public void RemoveWall(Floor floor1, Floor floor2)
    {
        Vector2 wallPos = (Vector2)(floor1.mapPos + floor2.mapPos) / 2;
        if (wallGrid.ContainsKey(wallPos))
        {
            Destroy(wallGrid[wallPos].gameObject);
            wallGrid.Remove(wallPos);
            StartCoroutine(MapManager.inst.Rebaker());
        }
        else
            Debug.Log("Wall doesn't exists between : " + floor1.mapPos + ", " + floor2.mapPos);
    }

    private void LoadObjects()
    {
        Debug.Log(floors.transform.childCount);
        for(int i = 0; i < floors.transform.childCount; i++)
        {
            Floor floor = floors.transform.GetChild(i).GetComponent<Floor>();
            floorGrid.Add(floor.mapPos, floor);
        }
        for (int i = 0; i < walls.transform.childCount; i++)
        {
            Wall wall = walls.transform.GetChild(i).GetComponent<Wall>();
            wallGrid.Add(wall.mapPos, wall);
        }
    }

    private void Awake()
    {
        floorGrid = new Dictionary<Vector2Int, Floor>();
        wallGrid = new Dictionary<Vector2, Wall>();
        LoadObjects();
        maxMapSize = 5 * Mathf.Max(testInputSizeX, testInputSizeY);
        //CreateFloor(new Vector2Int(0, 0), new Vector2Int(9, 9));
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
