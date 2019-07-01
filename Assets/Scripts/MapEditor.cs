using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;

public class MapEditor : SingletonBehaviour<MapEditor>
{
    public class objectData
    {
        public TileMode tag;
        public float xPos, yPos;
        public objectData(TileMode _tag, Vector2 _pos)
        {
            tag = _tag; xPos = _pos.x; yPos = _pos.y;
        }
    }
    public class clearData
    {
        public ClearType type;
        public int goal;
        public clearData(ClearType _type, int _goal)
        {
            type = _type; goal = _goal;
        }
    }
    public class MapSaveData
    {
        public List<objectData> objects;
        public List<clearData> clears;
        public MapSaveData()
        {
            objects = new List<objectData>();
            clears = new List<clearData>();
        }
        public void AddObject(TileMode _tag, Vector2 _pos)
        {
            objects.Add(new objectData(_tag, _pos));
        }
        public void AddClears(ClearType _type, int _goal)
        {
            clears.Add(new clearData(_type, _goal));
        }
    }
    public Map currentMap;
    public Map[] stage;
    public MapEditorTile tile;
    TileMode currentMode;
    public Text modeSign;
    public GameObject startSign, goalSign, mapSizeSetter, mapEditorTiles;
    public Dictionary<Floor, GameObject> startSigns, goalSigns;

    public Material editNormalMat;
    
    bool isEditorStarted;
    bool isCreateMode;

    public void StartMap(Map _newMap)
    {
        if (currentMap != null)
            Destroy(currentMap.gameObject);
        currentMap = Instantiate(_newMap);
        currentMap.transform.position = new Vector3(0, 0, 0);
        currentMap.InitiateMap();
    }
    /// <summary>
    /// Saves map to Assets folder.
    /// </summary>
    /// <param name="_newMap"></param>
    public void SaveMap(Map _newMap)
    {
        System.DateTime time = System.DateTime.Now;
        string localPath = "Assets/" + time.ToShortDateString() + "-" + time.Hour + "-" + time.Minute + "-" + time.Second + ".json";
        if(currentMap.startFloors.Count == 0)
            Debug.Log("There is no start floor.");
        else
        {
            MapSaveData mapSaveData = new MapSaveData();
            mapSaveData.AddObject(TileMode.None, new Vector2(currentMap.maxMapSize, 0));
            foreach(Transform child in currentMap.walls.transform)
            {
                Wall temp = child.GetComponent<Wall>();
                if (temp is NormalWall)
                    mapSaveData.AddObject(TileMode.Normal, temp.mapPos);
                else
                    mapSaveData.AddObject(TileMode.Mirror, temp.mapPos);
            }
            foreach(Transform child in currentMap.floors.transform)
            {
                Floor temp = child.GetComponent<Floor>();
                mapSaveData.AddObject(TileMode.Floor, temp.mapPos);
                if (child.GetComponent<Floor>().isGoalFloor)
                    mapSaveData.AddObject(TileMode.goalFloor, temp.mapPos);
            }
            foreach(Floor child in currentMap.startFloors)
            {
                Floor temp = child.GetComponent<Floor>();
                mapSaveData.AddObject(TileMode.StartFloor, temp.mapPos);
            }
            foreach (Transform child in currentMap.objects.transform)
            {
                IObject temp = child.GetComponent<IObject>();
                if (temp.GetType() == ObjType.Briefcase)
                    mapSaveData.AddObject(TileMode.Briefcase, temp.GetPos());
                else if(temp.GetType() == ObjType.Camera)
                    mapSaveData.AddObject(TileMode.Camera, temp.GetPos());
                else if (temp.GetType() == ObjType.Mannequin)
                {
                    if (temp.GetObject().GetComponent<Mannequin>().isWhite)
                        mapSaveData.AddObject(TileMode.WMannequin, temp.GetPos());
                    else
                        mapSaveData.AddObject(TileMode.BMannequin, temp.GetPos());
                }
            }
            for(int i = 0; i < currentMap.clearConditions.Count; i++)
                mapSaveData.AddClears(currentMap.clearConditions[i].type, currentMap.clearConditions[i].goal);
            File.WriteAllText(localPath, JsonConvert.SerializeObject(mapSaveData));
            Debug.Log("Map saved at " + localPath);}
    }
    public void SaveCurrentMap()
    {
        SaveMap(currentMap);
    }
    public void SetMapSize()
    {
        InputField xInput = mapSizeSetter.transform.Find("x").GetComponent<InputField>();
        InputField yInput = mapSizeSetter.transform.Find("y").GetComponent<InputField>();
        int x = int.Parse(xInput.text);
        int y = int.Parse(yInput.text);
        int xMax = x % 2 == 0 ? x / 2 - 1 : x / 2;
        int yMax = y % 2 == 0 ? y / 2 - 1 : y / 2;
        for(int i = 0; i < mapEditorTiles.transform.childCount; i++)
            Destroy(mapEditorTiles.transform.GetChild(i).gameObject);
        currentMap.maxMapSize = 5 * Mathf.Max(x, y);
        for (int i = -x / 2; i <= xMax; i++)
            for (int j = -y / 2; j <= yMax; j++)
                Instantiate(tile, new Vector3(i, -0.3f, j), Quaternion.identity, mapEditorTiles.transform).GetComponent<MapEditorTile>().mapPos = new Vector2(i, j);
        mapSizeSetter.SetActive(false);
        isEditorStarted = true;
    }
    public void ResizeMap()
    {
        mapSizeSetter.SetActive(true);
        isEditorStarted = false;
    }
    public void SwitchMode(int mode)
    {
        currentMode = (TileMode)mode;
        SetModeSign();
    }
    public void SetCreateMode(bool mode)
    {
        isCreateMode = mode;
        SetModeSign();
    }
    void SetModeSign()
    {
        string sign = "Mode : " + currentMode.ToString();
        if (isCreateMode)
            modeSign.text = sign + " Create";
        else
            modeSign.text = sign + " Destroy";
    }
    public void AddBulletToPlayer(int bulletMode)
    {
        currentMap.initialBullets.Add((BulletCode)bulletMode);
        //PlayerController.inst.bulletList.Add((BulletCode)bulletMode);
    }

    private void Awake()
    {
        MapManager.inst.isMapEditingOn = true;
        isEditorStarted = false;
        isCreateMode = true;
        startSigns = new Dictionary<Floor, GameObject>();
        goalSigns = new Dictionary<Floor, GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartMap(stage[0]);
        SwitchMode(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (isEditorStarted && Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(mouseRay, out hit))
            {
                Debug.Log(hit.transform.position);
                Vector2Int clickedPos = Vector2Int.zero;
                Vector2 wallPos = Vector2.zero;
                if (hit.transform.tag == "wallSign")
                    wallPos = new Vector2(hit.transform.position.x, hit.transform.position.z);
                else
                    clickedPos = new Vector2Int((int)hit.transform.position.x, (int)hit.transform.position.z);
                if(currentMode == TileMode.Floor)
                {
                    if (isCreateMode)
                        currentMap.CreateFloor(clickedPos);
                    else
                        currentMap.RemoveFloor(clickedPos);
                }
                else if(currentMode == TileMode.Normal || currentMode == TileMode.Mirror)
                {
                    if (isCreateMode)
                    {
                        Debug.Log(wallPos);
                        currentMap.CreateWall(wallPos, (WallType)((int)currentMode - 1));
                        if (currentMap.GetWallAtPos(wallPos) != null && currentMap.GetWallAtPos(wallPos).GetComponent<Wall>() is NormalWall)
                            currentMap.GetWallAtPos(wallPos).gameObject.GetComponent<MeshRenderer>().material = editNormalMat;
                    }
                    else
                        currentMap.RemoveWall(wallPos);
                }
                else if(currentMode == TileMode.StartFloor)
                {
                    if (isCreateMode)
                    {
                        if(currentMap.startFloors.Contains(currentMap.GetFloorAtPos(clickedPos)))
                        {
                            Debug.Log("Start floor already exists at : (" + clickedPos.x + ", " + clickedPos.y + ")");
                        }
                        else
                        {
                            currentMap.startFloors.Add(currentMap.GetFloorAtPos(clickedPos));
                            startSigns.Add(currentMap.GetFloorAtPos(clickedPos), Instantiate(startSign));
                            startSigns[currentMap.GetFloorAtPos(clickedPos)].transform.position = new Vector3(clickedPos.x, 2, clickedPos.y);
                        }
                    }
                    else
                    {
                        if (!currentMap.startFloors.Contains(currentMap.GetFloorAtPos(clickedPos)))
                        {
                            Debug.Log("Start floor doesn't exist at : (" + clickedPos.x + ", " + clickedPos.y + ")");
                        }
                        else
                        {
                            currentMap.startFloors.Remove(currentMap.GetFloorAtPos(clickedPos));
                            Destroy(startSigns[currentMap.GetFloorAtPos(clickedPos)].gameObject);
                            startSigns.Remove(currentMap.GetFloorAtPos(clickedPos));
                        }
                    }
                }
                else if(currentMode == TileMode.goalFloor)
                {
                    if (isCreateMode)
                    {
                        if (currentMap.GetFloorAtPos(clickedPos).isGoalFloor)
                            Debug.Log("Goal floor already exists at : (" + clickedPos.x + ", " + clickedPos.y + ")");
                        else
                        {
                            currentMap.GetFloorAtPos(clickedPos).isGoalFloor = true;
                            goalSigns.Add(currentMap.GetFloorAtPos(clickedPos), 
                                Instantiate(goalSign));
                            goalSigns[currentMap.GetFloorAtPos(clickedPos)].transform.position = new Vector3(clickedPos.x, 2, clickedPos.y);
                        }
                    }
                    else
                    {
                        if (!currentMap.GetFloorAtPos(clickedPos).isGoalFloor)
                            Debug.Log("Goal floor doesn't exist at : (" + clickedPos.x + ", " + clickedPos.y + ")");
                        else
                        {
                            currentMap.GetFloorAtPos(clickedPos).isGoalFloor = false;
                            Destroy(goalSigns[currentMap.GetFloorAtPos(clickedPos)].gameObject);
                            goalSigns.Remove(currentMap.GetFloorAtPos(clickedPos));
                        }
                    }
                }
                else if((int)currentMode >= 5 && (int)currentMode <= 8)
                {
                    if (isCreateMode)
                    {
                        Debug.Log(wallPos);
                        if(currentMode == TileMode.BMannequin)
                            currentMap.CreateObject(clickedPos, ObjType.Mannequin, false);
                        else
                            currentMap.CreateObject(clickedPos, (ObjType)((int)currentMode - 4));
                    }
                    else
                        currentMap.RemoveObject(clickedPos);
                }
            }
        }
    }
}
