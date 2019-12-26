using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.SceneManagement;

public class TempMapEditor : SingletonBehaviour<MapEditor>
{
    public class ObjectData
    {
        public TileMode tag;
        public float xPos, yPos;
        public ObjectData(TileMode _tag, Vector2 _pos)
        {
            tag = _tag; xPos = _pos.x; yPos = _pos.y;
        }
    }
    public class ClearData
    {
        public ClearType type;
        public int goal;
        public ClearData(ClearType _type, int _goal)
        {
            type = _type; goal = _goal;
        }
    }
    public class MapSaveData
    {
        public List<ObjectData> objects;
        public List<ClearData> clears;
        public List<BulletCode> cases;
        public List<BulletCode> bullets;
        public string comments = null;
        public MapSaveData()
        {
            objects = new List<ObjectData>();
            clears = new List<ClearData>();
            cases = new List<BulletCode>();
            bullets = new List<BulletCode>();
        }
        public void AddObject(TileMode _tag, Vector2 _pos)
        {
            objects.Add(new ObjectData(_tag, _pos));
        }
        public void AddClears(ClearType _type, int _goal)
        {
            clears.Add(new ClearData(_type, _goal));
        }
    }
    public Map currentMap;
    public MapEditorTile tile;
    TileMode currentMode;
    BulletCode bulletMode;
    public Text modeSign;
    public GameObject startSign, goalSign, mapSizeSetter, saveMapSelector, loadMapSelector, mapEditorTiles;
    public Dropdown goalDropdown;
    public InputField goalInputField;
    public Dictionary<Floor, GameObject> startSigns, goalSigns;

    public Material editNormalMat;
    
    public bool isEditorStarted;
    bool isCreateMode;

    public string mapName;

    public void StartMap(Map _newMap)
    {
        currentMap = Instantiate(_newMap);
        currentMap.transform.position = new Vector3(0, 0, 0);
        currentMap.InitiateMap();
    }
    /// <summary>
    /// Saves map to Assets folder.
    /// </summary>
    /// <param name="_newMap"></param>
    public void SaveMap()
    {
        saveMapSelector.SetActive(false);
        Map _newMap = currentMap;
        /* 맵 저장 시 반드시 승리 조건 작성할 것
         * 목표가 '모든'일 경우 승리 목표는 초기 맵 기준으로 작성
         */
        var x = saveMapSelector.transform.Find("x").GetComponent<InputField>();
        var y = saveMapSelector.transform.Find("y").GetComponent<InputField>();
        mapName = x.text + "_" + y.text;
        //System.DateTime time = System.DateTime.Now;
        //string localPath = "Assets/" + time.ToShortDateString() + "-" + time.Hour + "-" + time.Minute + "-" + time.Second + ".json";
        string localPath = "Assets/Resources/Stages/stage" + mapName + ".json";
        if (currentMap.startFloors.Count == 0)
            Debug.Log("There is no start floor.");
        else
        {
            MapSaveData mapSaveData = new MapSaveData();
            mapSaveData.AddObject(TileMode.None, new Vector2(currentMap.maxMapSize, 0));
            foreach(Transform child in currentMap.walls.transform)
            {
                Wall temp = child.GetComponent<Wall>();
                if (temp is NormalWall)
                    mapSaveData.AddObject(TileMode.NormalWall, temp.mapPos);
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
                {
                    mapSaveData.cases.Add(temp.GetObject().GetComponent<Briefcase>().dropBullet);
                    //mapSaveData.AddObject(TileMode.Briefcase, temp.GetPos());
                }
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
            for (int i = 0; i < currentMap.initialBullets.Count; i++)
                mapSaveData.bullets.Add(currentMap.initialBullets[i]);

            mapSaveData.comments = currentMap.comments;

            if (File.Exists(localPath))
            {
                Debug.Log("File Exists");
                File.Delete(localPath);
            }

            File.WriteAllText(localPath, JsonConvert.SerializeObject(mapSaveData));
            Debug.Log("Map saved at " + localPath);}
    }

    public void LoadMap()
    {
        loadMapSelector.SetActive(false);
        var x = loadMapSelector.transform.Find("x").GetComponent<InputField>();
        var y = loadMapSelector.transform.Find("y").GetComponent<InputField>();
        mapName = x.text + "_" + y.text;
        TextAsset _newMap = Resources.Load("Stages/stage" + mapName) as TextAsset;
        if(_newMap != null)
        {
            var loadedMapData = JsonConvert.DeserializeObject<MapEditor.MapSaveData>(_newMap.ToString());

            InputField xInput = mapSizeSetter.transform.Find("x").GetComponent<InputField>();
            InputField yInput = mapSizeSetter.transform.Find("y").GetComponent<InputField>();
            xInput.text = loadedMapData.objects[0].xPos.ToString();
            yInput.text = loadedMapData.objects[0].xPos.ToString();
            SetMapSize();

            for (int i = 0; i < loadedMapData.clears.Count; i++)
            {
                var temp = loadedMapData.clears[i];
                currentMap.clearConditions.Add(new ClearCondition(temp.type, temp.goal));
            }
            int casesIndex = 0;
            for (int i = 1; i < loadedMapData.objects.Count; i++)
            {
                var temp = loadedMapData.objects[i];
                switch (temp.tag)
                {
                    case TileMode.Floor:
                        currentMap.CreateFloor(new Vector2Int((int)temp.xPos, (int)temp.yPos));
                        break;
                    case TileMode.NormalWall:
                        currentMap.CreateWall(new Vector2(temp.xPos, temp.yPos), WallType.Normal);
                        if (currentMap.GetWallAtPos(new Vector2(temp.xPos, temp.yPos)) != null && currentMap.GetWallAtPos(new Vector2(temp.xPos, temp.yPos)).GetComponent<Wall>() is NormalWall)
                            currentMap.GetWallAtPos(new Vector2(temp.xPos, temp.yPos)).gameObject.GetComponent<MeshRenderer>().material = editNormalMat;
                        break;
                    case TileMode.Mirror:
                        currentMap.CreateWall(new Vector2(temp.xPos, temp.yPos), WallType.Mirror);
                        break;
                    case TileMode.StartFloor:
                        currentMap.startFloors.Add(currentMap.GetFloorAtPos(new Vector2Int((int)temp.xPos, (int)temp.yPos)));
                        startSigns.Add(currentMap.GetFloorAtPos(new Vector2Int((int)temp.xPos, (int)temp.yPos)), Instantiate(startSign));
                        startSigns[currentMap.GetFloorAtPos(new Vector2Int((int)temp.xPos, (int)temp.yPos))].transform.position = new Vector3(temp.xPos, 2, temp.yPos);
                        break;
                    /*case TileMode.Briefcase:
                        currentMap.CreateObject(new Vector2Int((int)temp.xPos, (int)temp.yPos), ObjType.Briefcase, loadedMapData.cases[casesIndex++]);
                        break;*/
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
                        currentMap.SetGoalFloor(new Vector2Int((int)temp.xPos, (int)temp.yPos));
                        goalSigns.Add(currentMap.GetFloorAtPos(new Vector2Int((int)temp.xPos, (int)temp.yPos)), Instantiate(goalSign));
                        goalSigns[currentMap.GetFloorAtPos(new Vector2Int((int)temp.xPos, (int)temp.yPos))].transform.position = new Vector3(temp.xPos, 2, temp.yPos);
                        break;
                    default:
                        break;
                }
            }
            for (int i = 0; i < loadedMapData.bullets.Count; i++) currentMap.initialBullets.Add(loadedMapData.bullets[i]);
            if (loadedMapData.comments != null) currentMap.comments = loadedMapData.comments;
            xInput.text = (currentMap.maxBorder.x - currentMap.minBorder.x + 1).ToString();
            yInput.text = (currentMap.maxBorder.y - currentMap.minBorder.y + 1).ToString();
            SetMapSize();
        }
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
        saveMapSelector.SetActive(false);
        loadMapSelector.SetActive(false);
        isEditorStarted = false;
    }
    public void LoadMapButton()
    {
        mapSizeSetter.SetActive(false);
        saveMapSelector.SetActive(false);
        loadMapSelector.SetActive(true);
        isEditorStarted = false;
    }
    public void SaveMapButton()
    {
        mapSizeSetter.SetActive(false);
        saveMapSelector.SetActive(true);
        loadMapSelector.SetActive(false);
        isEditorStarted = false;
    }
    public void SwitchMode(int _tileMode)
    {
        currentMode = (TileMode)_tileMode;
        //if (currentMode != TileMode.Briefcase) SwitchBulletMode(3);
        SetModeSign();
    }
    public void SwitchBulletMode(int _bulletMode)
    {
        bulletMode = (BulletCode)_bulletMode;
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
        if (bulletMode < 0) currentMap.initialBullets.Clear();
        else currentMap.initialBullets.Add((BulletCode)bulletMode);
    }

    public void TestMapStart()
    {
        if(mapName != "")
        {
            StageSelector.selectedStage = mapName;
            SceneManager.LoadScene("PlayStage");
        }
        else
        {
            Debug.Log("Save Your Map!");
        }
    }

    public void AddClearCondition()
    {
        ClearType c = (ClearType)System.Enum.Parse(typeof(ClearType), goalDropdown.options[goalDropdown.value].text);
        int n = int.Parse(goalInputField.text);
        if(n >= 0)
        {
            currentMap.clearConditions.Add(new ClearCondition(c, n));
        }
    }

    public void ClearClearCondition()
    {
        currentMap.clearConditions.Clear();
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
        GameManager.inst.ResetClearIndex();
        StartMap(currentMap);
        SwitchMode(0);
        SwitchBulletMode((int)BulletCode.None);
        goalDropdown.options.Clear();
        foreach(ClearType c in (ClearType[])System.Enum.GetValues(typeof(ClearType)))
        {
            goalDropdown.options.Add(new Dropdown.OptionData(c.ToString()));
        }
        goalDropdown.value = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isEditorStarted && Input.GetMouseButton(0) && Input.mousePosition.x > 250 && Input.mousePosition.x < 1550)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool isWall = false;
            if (Physics.Raycast(mouseRay, out hit))
            {
                Debug.Log(hit.transform.position);
                Vector2Int clickedPos = Vector2Int.zero;
                Vector2 wallPos = Vector2.zero;
                if (hit.transform.tag == "wallSign")
                {
                    wallPos = new Vector2(hit.transform.position.x, hit.transform.position.z);
                    isWall = true;
                }
                else
                {
                    clickedPos = new Vector2Int((int)hit.transform.position.x, (int)hit.transform.position.z);
                    isWall = false;
                }
            
                if(currentMode == TileMode.Floor && !isWall)
                {
                    if (isCreateMode)
                        currentMap.CreateFloor(clickedPos);
                    else
                        currentMap.RemoveFloor(clickedPos);
                }
                else if(currentMode == TileMode.NormalWall || currentMode == TileMode.Mirror && isWall)
                {
                    if (isCreateMode)
                    {
                        currentMap.CreateWall(wallPos, (WallType)((int)currentMode - 1));
                        if (currentMap.GetWallAtPos(wallPos) != null && currentMap.GetWallAtPos(wallPos).GetComponent<Wall>() is NormalWall)
                            currentMap.GetWallAtPos(wallPos).gameObject.GetComponent<MeshRenderer>().material = editNormalMat;
                    }
                    else
                        currentMap.RemoveWall(wallPos);
                }
                else if(currentMode == TileMode.StartFloor && !isWall)
                {
                    if (isCreateMode)
                    {
                        if(currentMap.startFloors.Contains(currentMap.GetFloorAtPos(clickedPos)))
                            Debug.Log("Start floor already exists at : (" + clickedPos.x + ", " + clickedPos.y + ")");
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
                            Debug.Log("Start floor doesn't exist at : (" + clickedPos.x + ", " + clickedPos.y + ")");
                        else
                        {
                            currentMap.startFloors.Remove(currentMap.GetFloorAtPos(clickedPos));
                            Destroy(startSigns[currentMap.GetFloorAtPos(clickedPos)].gameObject);
                            startSigns.Remove(currentMap.GetFloorAtPos(clickedPos));
                        }
                    }
                }
                else if(currentMode == TileMode.goalFloor && isWall)
                {
                    if (isCreateMode)
                    {
                        if (currentMap.GetFloorAtPos(clickedPos).isGoalFloor)
                            Debug.Log("Goal floor already exists at : (" + clickedPos.x + ", " + clickedPos.y + ")");
                        else
                        {
                            currentMap.GetFloorAtPos(clickedPos).isGoalFloor = true;
                            goalSigns.Add(currentMap.GetFloorAtPos(clickedPos), Instantiate(goalSign));
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
                else if((int)currentMode >= 5 && (int)currentMode <= 8 && isWall)
                {
                    if (isCreateMode)
                    {
                        Debug.Log(wallPos);
                        if(currentMode == TileMode.BMannequin)
                            currentMap.CreateObject(clickedPos, ObjType.Mannequin, false);
                        /*else if(currentMode == TileMode.Briefcase)
                            currentMap.CreateObject(clickedPos, ObjType.Briefcase, bulletMode);*/
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
