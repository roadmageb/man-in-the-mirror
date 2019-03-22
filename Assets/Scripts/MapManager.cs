using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public int x, y;
    public GameObject floor;
    public GameObject wall;
    public GameObject player;
    public Map currentMap;
    public static NavMeshSurface surface;
    public Map[] stage;


    public InputField xInput, yInput;


    public void CreateMap(Map _newMap)
    {
        Map newMap = Instantiate(_newMap);
        newMap.transform.position = new Vector3(0, 0, 0);
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


    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        mapMaxSize = 5 * Mathf.Max(x, y);
        mapCenterPos = mapMaxSize / 2;
        mapGrid = new GameObject[mapMaxSize, mapMaxSize];
        for (int i = mapCenterPos; i < mapCenterPos + x; i++)
            for (int j = mapCenterPos; j < mapCenterPos + y; j++)
                mapGrid[i, j] = Instantiate(floor, new Vector3(i, 0, j), Quaternion.identity, transform);
        player.transform.position = GetCubeAtPos(0, 0).transform.position + new Vector3(0, 1.5f, 0);
        CreateWall(GetCubeAtPos(2, 2), GetCubeAtPos(2, 3));
        CreateWall(GetCubeAtPos(3, 2), GetCubeAtPos(2, 2));
        CreateWall(GetCubeAtPos(3, 3), GetCubeAtPos(2, 3));
        surface.BuildNavMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
