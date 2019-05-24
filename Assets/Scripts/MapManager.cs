using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public class MapManager : SingletonBehaviour<MapManager>
{
    public bool isMapEditingOn;
    public Floor floor;
    public NormalWall normalWall;
    public Mirror mirror;
    public GameObject[] objects;
    public List<GameObject> players;
    public GameObject player;
    public Map currentMap;
    public NavMeshSurface surface;
    public Map[] stage;

    public void LoadMap(Map _newMap)
    {
        if(currentMap != null)
            Destroy(currentMap.gameObject);
        currentMap = Instantiate(_newMap);
        currentMap.transform.position = new Vector3(0, 0, 0);
        surface.BuildNavMesh();
        for (int i = 0; i < currentMap.startFloors.Count; i++)
            players.Add(Instantiate(player, currentMap.startFloors[i].transform.position + new Vector3(0, 0.1f, 0), Quaternion.identity));
    }
    public IEnumerator Rebaker()
    {
        yield return null;
        surface.BuildNavMesh();
    }

    private void Awake()
    {
        players = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(!isMapEditingOn)
            LoadMap(stage[0]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
