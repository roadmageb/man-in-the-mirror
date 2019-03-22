using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    //Remove when singleton added.
    public MapManager mapManager;
    //Remove when singleton added.

    public int maxMapSize;
    public int mapCenterPos;
    public GameObject[,] floorGrid;

    /// <summary>
    /// Get floor at position.
    /// </summary>
    /// <param name="x">X position of floor.</param>
    /// <param name="y">Y position of floor.</param>
    /// <returns></returns>
    public GameObject GetFloorAtPos(int x, int y)
    {
        if (floorGrid[mapCenterPos + x, mapCenterPos + y] != null)
            return floorGrid[mapCenterPos + x, mapCenterPos + y];
        else
            return null;
    }
    /// <summary>
    /// Get floor at position.
    /// </summary>
    /// <param name="pos">Position of floor.</param>
    /// <returns></returns>
    public GameObject GetFloorAtPos(Vector2Int pos)
    {
        return GetFloorAtPos(pos.x, pos.y);
    }
    /// <summary>
    /// Create floor at position.
    /// </summary>
    /// <param name="x">X position of floor.</param>
    /// <param name="y">Y position of floor.</param>
    public void CreateFloor(int x, int y)
    {
        if(floorGrid[mapCenterPos + x, mapCenterPos + y] == null)
        {
            floorGrid[mapCenterPos + x, mapCenterPos + y] = Instantiate(mapManager.floor, new Vector3(mapCenterPos + x, 0, mapCenterPos + y), Quaternion.identity, transform);
        }
        else
        {
            Debug.Log("Floor already exists in : (" + x + ", " + y + ")");
        }
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
    /// Remove floor at position.
    /// </summary>
    /// <param name="x">X position of floor.</param>
    /// <param name="y">Y position of floor.</param>
    public void RemoveFloor(int x, int y)
    {
        if (floorGrid[mapCenterPos + x, mapCenterPos + y] != null)
        {
            Destroy(floorGrid[mapCenterPos + x, mapCenterPos + y].gameObject);
        }
        else
        {
            Debug.Log("Floor doesn't exists in : (" + x + ", " + y + ")");
        }
    }
    /// <summary>
    /// Remove floor at position.
    /// </summary>
    /// <param name="pos">Position of floor.</param>
    public void RemoveFloor(Vector2Int pos)
    {
        RemoveFloor(pos.x, pos.y);
    }

    public void CreateWall(GameObject cube1, GameObject cube2)
    {

    }







    public void Rebaker()
    {
        surface.BuildNavMesh();
    }
    /// <summary>
    /// Create wall between two cubes.
    /// </summary>
    /// <param name="cube1">Cube 1</param>
    /// <param name="cube2">Cube 2</param>
    public void CreateWall(GameObject cube1, GameObject cube2)
    {
        Vector3 wallPos = (cube1.transform.position + cube2.transform.position) / 2;
        GameObject abc = Instantiate(wall, wallPos, Quaternion.identity, transform);
        abc.transform.LookAt(cube1.transform);
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
