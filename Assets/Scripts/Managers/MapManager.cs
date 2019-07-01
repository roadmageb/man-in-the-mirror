using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Newtonsoft.Json;

public class MapManager : SingletonBehaviour<MapManager>
{
    public bool isMapEditingOn;
    public NavMeshSurface surface;
    public Map currentMap, emptyMap;
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
    public TextAsset[] stage;
    public BulletFactory bulletFactory;

    /// <summary>
    /// Load and make a map by map data json file.
    /// </summary>
    /// <param name="_newMap">The json file of the map data to be created.</param>
    public void LoadMap(TextAsset _newMap)
    {
        if (currentMap != null)
            Destroy(currentMap.gameObject);
        var loadedMapData = JsonConvert.DeserializeObject<MapEditor.MapSaveData>(_newMap.ToString());
        currentMap = Instantiate(emptyMap, new Vector3(0, 0, 0), Quaternion.identity);
        GameManager.inst.ResetClearIndex();
        currentMap.InitiateMap();
        currentMap.maxMapSize = (int)loadedMapData.objects[0].xPos;
        for(int i = 1; i < loadedMapData.objects.Count; i++)
        {
            var temp = loadedMapData.objects[i];
            switch (temp.tag)
            {
                case TileMode.Floor:
                    currentMap.CreateFloor(new Vector2Int((int)temp.xPos, (int)temp.yPos));
                    break;
                case TileMode.Normal:
                    currentMap.CreateWall(new Vector2(temp.xPos, temp.yPos), WallType.Normal);
                    break;
                case TileMode.Mirror:
                    currentMap.CreateWall(new Vector2(temp.xPos, temp.yPos), WallType.Mirror);
                    break;
                case TileMode.StartFloor:
                    currentMap.startFloors.Add(currentMap.GetFloorAtPos(new Vector2Int((int)temp.xPos, (int)temp.yPos)));
                    break;
                case TileMode.Briefcase:
                    currentMap.CreateObject(new Vector2Int((int)temp.xPos, (int)temp.yPos), ObjType.Briefcase);
                    break;
                case TileMode.Camera:
                    currentMap.CreateObject(new Vector2Int((int)temp.xPos, (int)temp.yPos), ObjType.Camera);
                    break;
                case TileMode.WMannequin:
                    currentMap.CreateObject(new Vector2Int((int)temp.xPos, (int)temp.yPos), ObjType.Mannequin, true);
                    break;
                case TileMode.BMannequin:
                    currentMap.CreateObject(new Vector2Int((int)temp.xPos, (int)temp.yPos), ObjType.Mannequin, false);
                    break;
                case TileMode.goalFloor:
                    currentMap.GetFloorAtPos(new Vector2Int((int)temp.xPos, (int)temp.yPos)).isGoalFloor = true;
                    break;
                default:
                    break;
            }
        }
        for (int i = 0; i < loadedMapData.clears.Count; i++)
        {
            var temp = loadedMapData.clears[i];
            currentMap.clearConditions.Add(new ClearCondition(temp.type, temp.goal));
        }
        GameManager.inst.SetClearIndex(currentMap);
        surface.BuildNavMesh();
        GameManager.inst.uiGenerator.GenerateAllClearUI();
        for (int i = 0; i < currentMap.startFloors.Count; i++)
            PlayerController.inst.CreatePlayer(currentMap.startFloors[i]);
        for (int i = 0; i < currentMap.initialBullets.Count; i++)
            PlayerController.inst.bulletList.Add(currentMap.initialBullets[i]);


        /*if (currentMap != null)
            Destroy(currentMap.gameObject);
        currentMap = Instantiate(_newMap);
        currentMap.transform.position = new Vector3(0, 0, 0);
        surface.BuildNavMesh();
        GameManager.inst.SetClearIndex(currentMap);
        GameManager.inst.uiGenerator.GenerateAllClearUI();
        for (int i = 0; i < currentMap.startFloors.Count; i++)
            PlayerController.inst.CreatePlayer(currentMap.startFloors[i]);
        for (int i = 0; i < currentMap.initialBullets.Count; i++)
            PlayerController.inst.bulletList.Add(currentMap.initialBullets[i]);*/
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
