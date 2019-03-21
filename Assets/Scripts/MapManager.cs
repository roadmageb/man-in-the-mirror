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
    public GameObject[,] mapGrid;
    public NavMeshSurface surface;

    public InputField xInput, yInput;
    public void Rebaker()
    {
        surface.BuildNavMesh();
    }
    public void RemoveTile()
    {
        if (mapGrid[int.Parse(xInput.text), int.Parse(yInput.text)] != null)
        {
            Destroy(mapGrid[int.Parse(xInput.text), int.Parse(yInput.text)].gameObject);
            surface.BuildNavMesh();
        }
        else
            Debug.Log("Tile doesn't exists");
    }
    public void AddTile()
    {
        if (mapGrid[int.Parse(xInput.text), int.Parse(yInput.text)] == null)
        {
            mapGrid[int.Parse(xInput.text), int.Parse(yInput.text)] = Instantiate(floor, new Vector3(int.Parse(xInput.text), 0, int.Parse(yInput.text)), Quaternion.identity, transform);
            surface.BuildNavMesh();
        }
        else
            Debug.Log("Tile already exists");
    }

    // Start is called before the first frame update
    void Start()
    {
        mapGrid = new GameObject[100, 100];
        for(int i = 0; i < x; i++)
            for(int j = 0; j < y; j++)
                mapGrid[i, j] = Instantiate(floor, new Vector3(i, 0, j), Quaternion.identity, transform);
        Instantiate(wall, new Vector3(2, 1, 2), Quaternion.identity, transform);
        Instantiate(wall, new Vector3(3, 1, 2), Quaternion.identity, transform);
        Instantiate(wall, new Vector3(3, 1, 3), Quaternion.identity, transform);
        surface.BuildNavMesh();
        player.transform.position = mapGrid[0, 0].transform.position + new Vector3(0, 1.5f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
