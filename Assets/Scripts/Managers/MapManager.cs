using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Newtonsoft.Json;

public class MapManager : SingletonBehaviour<MapManager>
{
    public NavMeshSurface surface;
    public Map currentMap;
    public Map emptyMap;
    [Header("Instances")]
    public Floor floor;
    public GameObject truthBullet, fakeBullet, mirrorBullet;
    public GameObject[] walls;
    public GameObject[] IObjects;
    public GameObject[] mannequins;
    public GameObject player;
    [Header("All players")]
    public List<GameObject> players;
    public BulletFactory bulletFactory;

    /// <summary>
    /// Load and make a map by map data json file.
    /// </summary>
    /// <param name="_newMap">The json file of the map data to be created.</param>
    public void LoadMap(MapEditor.MapSaveData loadedMapData)
    {
        currentMap = Instantiate(emptyMap, new Vector3(0, 0, 0), Quaternion.identity);
        currentMap.InitiateMap();
        GameManager.inst.ResetClearIndex();
        PlayerController.inst.bulletList.Clear();
        players.Clear();
        currentMap.maxMapSize = (int)loadedMapData.objects[0].xPos;
        for (int i = 0; i < loadedMapData.clears.Count; i++)
        {
            var temp = loadedMapData.clears[i];
            currentMap.clearConditions.Add(new ClearCondition(temp.type, temp.goal));
        }
        GameManager.inst.SetClearIndex(currentMap);
        GameManager.inst.uiGenerator.GenerateAllClearUI();
        for(int i = 1; i < loadedMapData.objects.Count; i++)
        {
            var temp = loadedMapData.objects[i];
            switch (temp.tag)
            {
                case TileMode.Floor:
                    currentMap.CreateFloor(new Vector2Int((int)temp.xPos, (int)temp.yPos));
                    break;
                case TileMode.NormalWall:
                    currentMap.CreateWall(new Vector2(temp.xPos, temp.yPos), WallType.Normal);
                    break;
                case TileMode.Mirror:
                    currentMap.CreateWall(new Vector2(temp.xPos, temp.yPos), WallType.Mirror);
                    break;
                case TileMode.StartFloor:
                    currentMap.startFloors.Add(currentMap.GetFloorAtPos(new Vector2Int((int)temp.xPos, (int)temp.yPos)));
                    break;
                case TileMode.TrueCase:
                    currentMap.CreateObject(new Vector2(temp.xPos, temp.yPos), ObjType.Briefcase, temp.angle, BulletCode.True);
                    break;
                case TileMode.FalseCase:
                    currentMap.CreateObject(new Vector2(temp.xPos, temp.yPos), ObjType.Briefcase, temp.angle, BulletCode.False);
                    break;
                case TileMode.MirrorCase:
                    currentMap.CreateObject(new Vector2(temp.xPos, temp.yPos), ObjType.Briefcase, temp.angle, BulletCode.Mirror);
                    break;
                case TileMode.NullCase:
                    currentMap.CreateObject(new Vector2(temp.xPos, temp.yPos), ObjType.Briefcase, temp.angle, BulletCode.NULL);
                    break;
                case TileMode.Camera:
                    currentMap.CreateObject(new Vector2(temp.xPos, temp.yPos), ObjType.Camera, temp.angle);
                    break;
                case TileMode.WMannequin:
                    currentMap.CreateObject(new Vector2(temp.xPos, temp.yPos), ObjType.Mannequin, temp.angle, true, true);
                    break;
                case TileMode.BMannequin:
                    currentMap.CreateObject(new Vector2(temp.xPos, temp.yPos), ObjType.Mannequin, temp.angle, false, true);
                    break;
                case TileMode.GoalFloor:
                    currentMap.CreateFloor(new Vector2Int((int)temp.xPos, (int)temp.yPos), true);
                    break;
                case TileMode.Glass:
                    currentMap.CreateWall(new Vector2(temp.xPos, temp.yPos), WallType.Glass);
                    break;
                case TileMode.LightPole:
                    currentMap.CreateObject(new Vector2(temp.xPos, temp.yPos), ObjType.LightPole, temp.angle);
                    break;
                case TileMode.LightGetter:
                    currentMap.CreateObject(new Vector2(temp.xPos, temp.yPos), ObjType.LightGetter, temp.angle);
                    break;
                default:
                    break;
            }
        }
        for (int i = 0; i < currentMap.startFloors.Count; i++)
            PlayerController.inst.CreatePlayer(currentMap.startFloors[i]);
        surface.BuildNavMesh();
        for (int i = 0; i < loadedMapData.bullets.Count; i++)
            PlayerController.inst.AddBullet(loadedMapData.bullets[i]);
        if (loadedMapData.comments != null && loadedMapData.comments != "")
        {
            currentMap.comments = loadedMapData.comments;
            GameManager.inst.commentUIGenerator.SetComment(currentMap.comments);
        }
        Camera.main.GetComponent<CameraController>().centerPos =
            new Vector3((float)(currentMap.maxBorder.x + currentMap.minBorder.x) / 2, 0, (float)(currentMap.maxBorder.y + currentMap.minBorder.y) / 2);
        float fov = (Mathf.Max(currentMap.maxBorder.x - currentMap.minBorder.x, currentMap.maxBorder.y - currentMap.minBorder.y) + 1) + 10;
        Camera.main.fieldOfView = fov;
        Camera.main.GetComponent<CameraController>().minFOV = fov * 0.7f;
        Camera.main.GetComponent<CameraController>().maxFOV = fov * 1.5f;
    }

    public IEnumerator Rebaker()
    {
        yield return null;
        surface.BuildNavMesh();
    }

    private void Awake()
    {
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
