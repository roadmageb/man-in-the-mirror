using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapManager : SingletonBehaviour<MapManager>
{
    public Floor floor;
    public Wall wall;
    public GameObject player;
    public Map currentMap;
    public NavMeshSurface surface;
    public Map[] stage;

    public void LoadMap(Map _newMap)
    {
        currentMap = Instantiate(_newMap);
        currentMap.transform.position = new Vector3(0, 0, 0);
        player.transform.position = currentMap.GetFloorAtPos(0, 0).transform.position + new Vector3(0, 1.5f, 0);
    }
    public IEnumerator Rebaker()
    {
        yield return null;
        surface.BuildNavMesh();
    }

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadMap(stage[0]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
