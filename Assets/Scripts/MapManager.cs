using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public int x, y;
    public GameObject cube;
    public GameObject player;
    public GameObject[,] mapGrid;

    // Start is called before the first frame update
    void Start()
    {
        mapGrid = new GameObject[x, y];
        for(int i = 0; i < x; i++)
            for(int j = 0; j < y; j++)
                mapGrid[i, j] = Instantiate(cube, new Vector3(i, 0, j), Quaternion.identity, transform);
        //player.transform.position = mapGrid[0, 0].transform.position + new Vector3(0, 1.5f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
