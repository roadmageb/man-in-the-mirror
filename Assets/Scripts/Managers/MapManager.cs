using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public class MapManager : SingletonBehaviour<MapManager>
{
    public bool isMapEditingOn;
    public NavMeshSurface surface;
    public Map currentMap;
    [Header("Instances")]
    public Floor floor;
    public NormalWall normalWall;
    public Mirror mirror;
    public GameObject truthBullet, fakeBullet, mirrorBullet;
    [Tooltip("Objects without mannequin.")]
    public GameObject briefCase;
    public GameObject cameraTurret;
    public GameObject[] mannequins;
    public List<GameObject> players;
    public GameObject player;
    public Map[] stage;
    public BulletFactory bulletFactory;

    public void LoadMap(Map _newMap)
    {
        if(currentMap != null)
            Destroy(currentMap.gameObject);
        currentMap = Instantiate(_newMap);
        currentMap.transform.position = new Vector3(0, 0, 0);
        surface.BuildNavMesh();
        GameManager.inst.SetClearIndex(currentMap);
        GameManager.inst.uiGenerator.GenerateAllClearUI();
        for (int i = 0; i < currentMap.startFloors.Count; i++)
            PlayerController.inst.CreatePlayer(currentMap.startFloors[i]);
    }
    public IEnumerator Rebaker()
    {
        yield return null;
        surface.BuildNavMesh();
    }

    private void Awake()
    {
        players = new List<GameObject>();
        bulletFactory = new BulletFactory(truthBullet, fakeBullet, mirrorBullet);
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
