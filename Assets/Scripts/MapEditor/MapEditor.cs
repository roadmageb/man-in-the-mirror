using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapEditor : SingletonBehaviour<MapEditor>
{
    public class ObjectData
    {
        public TileMode tag;
        public float xPos, yPos;
        public int angle;
        public ObjectData(TileMode _tag, Vector2 _pos, int _angle)
        {
            tag = _tag; xPos = _pos.x; yPos = _pos.y; angle = _angle;
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
        public List<BulletCode> bullets;
        public string comments = null, mapName = "";
        public float centerPosX = 0, centerPosY = 0;
        public MapSaveData()
        {
            objects = new List<ObjectData>();
            clears = new List<ClearData>();
            bullets = new List<BulletCode>();
        }
        public void AddObject(TileMode _tag, Vector2 _pos, int _angle = 0)
        {
            objects.Add(new ObjectData(_tag, _pos, _angle));
        }
        public void AddClears(ClearType _type, int _goal)
        {
            clears.Add(new ClearData(_type, _goal));
        }
    }

    public bool isLoaded = false;
    public GameObject[] tiles;
    public Image[] bulletTiles;
    Transform walls, floors, objects, jacksons, bullets;
    public GameObject stageSelectButton;
    public GameObject centerPosSetter;
    GameObject currentTile = null, controlPanel, stageSelectPanel, saveMapPanel, commentPanel, clearConditionPanel;
    GameObject stageSelectContent, debugText, commentInputField;
    public GameObject[] clearConditionButtons;
    public bool isPanelOn = false;
    bool isFloat = false, isAtPoint = false;
    string comment = "", mapName;
    TileMode tileMode = 0;
    Coroutine debugTextCoroutine;
    Vector3 prevMousePoint;
    private int minX = 0, minY = 0, maxX = 0, maxY = 0;

    public void PrintDebugText(string text)
    {
        if (debugTextCoroutine != null) StopCoroutine(debugTextCoroutine);
        debugTextCoroutine = StartCoroutine(DebugCoroutine(text));
        Debug.Log(text);
    }
    IEnumerator DebugCoroutine(string text)
    {
        debugText.GetComponent<Text>().text = text;
        debugText.GetComponent<Text>().color = new Color(0, 0, 0, 1);
        yield return new WaitForSeconds(1);
        for (float timer = 0; timer < 1; timer += Time.deltaTime)
        {
            yield return null;
            debugText.GetComponent<Text>().color = new Color(0, 0, 0, 1 - timer);
        }
    }

    Vector3 GetMousePoint()
    {
        Vector3 mousePoint, originPos = Camera.main.ScreenPointToRay(Input.mousePosition).origin;
        if (isFloat)
        {
            if (!isAtPoint)
            {
                if (Mathf.Abs(Mathf.Round(originPos.x) - originPos.x) < Mathf.Abs(Mathf.Round(originPos.y) - originPos.y))
                    mousePoint = new Vector3(Mathf.Round(originPos.x), 0, Mathf.Round(originPos.z - 0.5f) + 0.5f);
                else mousePoint = new Vector3(Mathf.Round(originPos.x - 0.5f) + 0.5f, 0, Mathf.Round(originPos.z));
            }
            else mousePoint = new Vector3(Mathf.Round(originPos.x - 0.5f) + 0.5f, 0, Mathf.Round(originPos.z - 0.5f) + 0.5f);
        }
        else mousePoint = new Vector3(Mathf.Round(originPos.x), 0, Mathf.Round(originPos.z));
        return mousePoint;
    }
    
    public void DeleteAll()
    {
        for (int i = 0; i < walls.childCount; i++) Destroy(walls.GetChild(i).gameObject);
        for (int i = 0; i < floors.childCount; i++) Destroy(floors.GetChild(i).gameObject);
        for (int i = 0; i < jacksons.childCount; i++) Destroy(jacksons.GetChild(i).gameObject);
        for (int i = 0; i < objects.childCount; i++) Destroy(objects.GetChild(i).gameObject);
        for (int i = 0; i < bullets.childCount; i++) Destroy(bullets.GetChild(i).gameObject);
        AddComment("");
        for (int i = 0; i < clearConditionButtons.Length; i++) AddClearCondition(i, -1);
        PrintDebugText("Reset stage");
    }
    
    public void AddBullet(int newBullet)
    {
        if (newBullet != (int)BulletCode.NULL)
            Instantiate(bulletTiles[newBullet - 1], new Vector3(50 + bullets.childCount * 50, 50, 0), Quaternion.identity, bullets);
        else if(bullets.childCount > 0)
        {
            Destroy(bullets.GetChild(0).gameObject);
            for (int i = 0; i < bullets.childCount; i++) bullets.GetChild(i).transform.position -= new Vector3(50, 0, 0);
        }
    }

    public void AddComment(string text)
    {
        comment = text;
        commentInputField.GetComponent<InputField>().text = text;
        PrintDebugText("Comment saved");
    }
    public void AddComment(InputField input)
    {
        AddComment(input.text);
    }

    public void AddClearCondition(int index, int goal)
    {
        if(index == (int)ClearType.AllCase || index == (int)ClearType.AllFloor || index == (int)ClearType.AllTurret)
            clearConditionButtons[index].transform.Find("Toggle").GetComponent<Toggle>().isOn = goal == -1 ? false : true;
        else clearConditionButtons[index].transform.Find("InputField").GetComponent<InputField>().text = goal.ToString();
        PrintDebugText("Added clear condition " + (ClearType)index + " " + goal);
    }

    public void ChangeTileMode(int _tileMode)
    {
        tileMode = (TileMode)_tileMode;
        if (currentTile != null && currentTile != centerPosSetter) currentTile.SetActive(false);
        if (tileMode != TileMode.None)
        {
            currentTile = tileMode == TileMode.CenterPos ? centerPosSetter : tiles[_tileMode - 1];
            currentTile.SetActive(true);
            if ((TileMode)_tileMode == TileMode.NormalWall || (TileMode)_tileMode == TileMode.Mirror || (TileMode)_tileMode == TileMode.Glass
                || (TileMode)_tileMode == TileMode.LightPole || (TileMode)_tileMode == TileMode.LightGetter)
            {
                isFloat = true;
                isAtPoint = false;
            }
            else
            {
                isFloat = false;
                isAtPoint = false;
            }
        }
        else currentTile = null;
        PrintDebugText("Tile mode changed to " + (TileMode)_tileMode);
    }
    
    public MapEditorTile CheckFloor(int x, int y)
    {
        for (int i = 0; i < floors.childCount; i++)
        {
            MapEditorTile temp = floors.GetChild(i).GetComponent<MapEditorTile>();
            if (temp.mapPos.x == x && temp.mapPos.y == y) return temp;
        }
        return null;
    }
    public MapEditorTile CheckJackson(int x, int y)
    {
        for (int i = 0; i < jacksons.childCount; i++)
        {
            MapEditorTile temp = jacksons.GetChild(i).GetComponent<MapEditorTile>();
            if (temp.mapPos.x == x && temp.mapPos.y == y) return temp;
        }
        return null;
    }
    public MapEditorTile CheckWall(float x, float y)
    {
        for (int i = 0; i < walls.childCount; i++)
        {
            MapEditorTile temp = walls.GetChild(i).GetComponent<MapEditorTile>();
            if (temp.mapPos.x == x && temp.mapPos.y == y) return temp;
        }
        return null;
    }
    public MapEditorTile CheckObject(float x, float y)
    {
        for (int i = 0; i < objects.childCount; i++)
        {
            MapEditorTile temp = objects.GetChild(i).GetComponent<MapEditorTile>();
            if (temp.mapPos.x == x && temp.mapPos.y == y) return temp;
        }
        return null;
    }

    public void DeleteMap(Text text)
    {
        string localPath = "Assets/Resources/Stages/" + text.text + ".json";
        string localPathMeta = "Assets/Resources/Stages/" + text.text + ".json.meta";
        if (File.Exists(localPath))
        {
            File.Delete(localPath);
            File.Delete(localPathMeta);
            PrintDebugText("Deleted " + text.text);
            CancelMapLoad();
            CheckLoadableStages();
        }
        else
        {
            PrintDebugText("There is no stage named " + text.text);
        }
    }

    public MapSaveData SerializeMap()
    {
        if (jacksons.childCount == 0)
        {
            PrintDebugText("There is no start floor");
            return null;
        }
        else
        {
            MapSaveData mapSaveData = new MapSaveData();
            mapSaveData.AddObject(TileMode.None, new Vector2((Mathf.Max(maxX - minX, maxY - minY) + 1) * 5, 0));

            for (int i = 0; i < walls.childCount; i++)
            {
                MapEditorTile temp = walls.GetChild(i).GetComponent<MapEditorTile>();
                mapSaveData.AddObject(temp.thisTile, temp.mapPos, (int)temp.transform.eulerAngles.y);
            }
            for (int i = 0; i < floors.childCount; i++)
            {
                MapEditorTile temp = floors.GetChild(i).GetComponent<MapEditorTile>();
                mapSaveData.AddObject(temp.thisTile, temp.mapPos);
            }
            for (int i = 0; i < jacksons.childCount; i++)
            {
                MapEditorTile temp = jacksons.GetChild(i).GetComponent<MapEditorTile>();
                mapSaveData.AddObject(TileMode.StartFloor, temp.mapPos);
            }
            for (int i = 0; i < objects.childCount; i++)
            {
                MapEditorTile temp = objects.GetChild(i).GetComponent<MapEditorTile>();
                mapSaveData.AddObject(temp.thisTile, temp.mapPos, (int)temp.transform.eulerAngles.y);
            }

            for (int i = 0; i < clearConditionButtons.Length; i++)
            {
                int goal = -1;
                if (i == (int)ClearType.AllCase || i == (int)ClearType.AllFloor || i == (int)ClearType.AllTurret)
                    goal = clearConditionButtons[i].transform.Find("Toggle").GetComponent<Toggle>().isOn ? 0 : goal;
                else goal = int.Parse(clearConditionButtons[i].transform.Find("InputField").GetComponent<InputField>().text);
                if (goal != -1) mapSaveData.AddClears((ClearType)i, goal);
            }
            for (int i = 0; i < bullets.childCount; i++) mapSaveData.bullets.Add(bullets.GetChild(i).GetComponent<MapEditorTile>().bulletCode);
            mapSaveData.comments = comment;
            mapSaveData.mapName = mapName;
            mapSaveData.centerPosX = centerPosSetter.transform.position.x;
            mapSaveData.centerPosY = centerPosSetter.transform.position.z;
            return mapSaveData;
        }
    }

    public void SaveMap(InputField input)
    {
        mapName = input.text;
        string localPath = "Assets/Resources/Stages/stage" + mapName + ".json";
        string localPathMeta = "Assets/Resources/Stages/" + mapName + ".json.meta";
        MapSaveData mapSaveData = SerializeMap();
        if(mapSaveData != null)
        {
            if (File.Exists(localPath))
            {
                Debug.Log("File Exists");
                File.Delete(localPath);
                File.Delete(localPathMeta);
            }

            File.WriteAllText(localPath, JsonConvert.SerializeObject(mapSaveData));
            PrintDebugText("Map saved at " + localPath);
        }
    }

    public void CheckLoadableStages()
    {
        TextAsset[] newMaps = Resources.LoadAll<TextAsset>("Stages");
        RectTransform rt = stageSelectContent.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, Mathf.Max(375, newMaps.Length * 90));
        float y = rt.rect.height / 2 - 50;
        for (int i = 0; i < newMaps.Length; i++)
        {
            GameObject temp = Instantiate(stageSelectButton, stageSelectContent.transform);
            temp.transform.Find("StageSelect").transform.Find("Text").GetComponent<Text>().text = newMaps[i].name;
            temp.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, y - 90 * i);
        }
    }
    public void CancelMapLoad()
    {
        for (int i = 0; i < stageSelectContent.transform.childCount; i++) Destroy(stageSelectContent.transform.GetChild(i).gameObject);
    }

    public void InstantiateMap(MapSaveData loadedMapData)
    {
        for (int i = 0; i < loadedMapData.clears.Count; i++)
        {
            AddClearCondition((int)loadedMapData.clears[i].type, loadedMapData.clears[i].goal);
        }
        for (int i = 1; i < loadedMapData.objects.Count; i++)
        {
            ObjectData temp = loadedMapData.objects[i];
            ChangeTileMode((int)temp.tag);
            Vector3 tilePos = new Vector3(temp.xPos, 0, temp.yPos);
            GameObject newTile;
            if (temp.tag == TileMode.Floor || temp.tag == TileMode.GoalFloor)
            {
                newTile = Instantiate(currentTile, tilePos, Quaternion.identity, floors);
                if (floors.childCount == 1 || tilePos.x < minX) minX = (int)tilePos.x;
                if (floors.childCount == 1 || tilePos.x > maxX) maxX = (int)tilePos.x;
                if (floors.childCount == 1 || tilePos.z < minY) minY = (int)tilePos.z;
                if (floors.childCount == 1 || tilePos.z > maxY) maxY = (int)tilePos.z;
            }
            else if (temp.tag == TileMode.NormalWall || temp.tag == TileMode.Mirror || temp.tag == TileMode.Glass) newTile = Instantiate(currentTile, tilePos, Quaternion.Euler(0, temp.angle, 0), walls);
            else if (temp.tag == TileMode.StartFloor) newTile = Instantiate(currentTile, tilePos + new Vector3(0, 1, 0), Quaternion.identity, jacksons);
            else newTile = Instantiate(currentTile, tilePos + new Vector3(0, 1, 0), Quaternion.Euler(0, temp.angle, 0), objects);
            newTile.GetComponent<MapEditorTile>().mapPos = new Vector2(tilePos.x, tilePos.z);
            newTile.GetComponent<BoxCollider>().enabled = true;
        }
        for (int i = 0; i < loadedMapData.bullets.Count; i++) AddBullet((int)loadedMapData.bullets[i]);
        if (loadedMapData.comments != null) AddComment(loadedMapData.comments);
        mapName = loadedMapData.mapName;
    }

    public void SetNameInputField(InputField input)
    {
        input.text = mapName;
    }

    public void LoadMap(Text text)
    {
        TextAsset newMap = Resources.Load("Stages/" + text.text) as TextAsset;
        if (newMap != null)
        {
            DeleteAll();
            InstantiateMap(JsonConvert.DeserializeObject<MapSaveData>(newMap.ToString()));
            PrintDebugText("Loaded " + text.text);
        }
        else PrintDebugText("There is no map named " + text.text);

        isPanelOn = false;
    }

    public void TestMap()
    {
        StageInfo.inst.testMap = SerializeMap();
        if(StageInfo.inst.testMap != null)
        {
            StageInfo.inst.selectedStage = "0_0";
            StageInfo.inst.nextStage = "0_0";
            DontDestroyOnLoad(StageInfo.inst);
            SceneManager.LoadScene("PlayStage");
        }
    }

    private void Awake()
    {
        controlPanel = GameObject.Find("ControlPanel");
        stageSelectContent = GameObject.Find("StageSelectContent");
        stageSelectPanel = GameObject.Find("StageSelectPanel");
        saveMapPanel = GameObject.Find("SaveMapPanel");
        commentPanel = GameObject.Find("CommentPanel");
        clearConditionPanel = GameObject.Find("ClearConditionPanel");
        debugText = GameObject.Find("DebugText");
        commentInputField = GameObject.Find("CommentInputField");
        walls = GameObject.Find("Walls").transform;
        floors = GameObject.Find("Floors").transform;
        objects = GameObject.Find("Objects").transform;
        jacksons = GameObject.Find("Jacksons").transform;
        bullets = GameObject.Find("Bullets").transform;
        StageInfo.inst.isMapEditor = true;

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = Instantiate(tiles[i]);
            tiles[i].SetActive(false);
        }
        for (int i = 0; i < clearConditionButtons.Length; i++) AddClearCondition(i, -1);
        stageSelectPanel.SetActive(false);
        saveMapPanel.SetActive(false);
        commentPanel.SetActive(false);
        clearConditionPanel.SetActive(false);
    }

    private void Start()
    {
        if(StageInfo.inst.testMap != null) InstantiateMap(StageInfo.inst.testMap);
        prevMousePoint = new Vector3(0, -1, 0);
    }


    // Update is called once per frame
    void Update()
    {
        bool isValid;
        if (!isPanelOn)
        {
            Vector3 mousePoint = GetMousePoint();
            if (currentTile != null && tileMode != 0)
            {
                isValid = false;
                currentTile.transform.position = mousePoint + new Vector3(0, tileMode == TileMode.CenterPos ? 5 : 0, 0);
                if (isFloat) currentTile.transform.rotation = Quaternion.Euler(0, (int)mousePoint.x == mousePoint.x ? 0 : 90, 0);
                if(prevMousePoint != mousePoint)
                {
                    if (Input.GetMouseButton(0))
                    {
                        if (tileMode == TileMode.LightPole && CheckObject(mousePoint.x, mousePoint.z) && CheckObject(mousePoint.x, mousePoint.z).thisTile == TileMode.LightPole)
                            CheckObject(mousePoint.x, mousePoint.z).transform.rotation = Quaternion.Euler(0, CheckObject(mousePoint.x, mousePoint.z).transform.eulerAngles.y + 90, 0);
                        else
                        {
                            if(tileMode == TileMode.CenterPos)
                            {
                                isValid = mousePoint.x >= minX && mousePoint.x <= maxX && mousePoint.z >= minY && mousePoint.z <= maxY;
                            }
                            else if (tileMode == TileMode.Floor || tileMode == TileMode.GoalFloor)
                            {
                                isValid = true;
                            }
                            else
                            {
                                if (isFloat)
                                {
                                    if (isAtPoint && CheckFloor((int)(mousePoint.x + 0.5f), (int)(mousePoint.z + 0.5f)) || CheckFloor((int)(mousePoint.x + 0.5f), (int)(mousePoint.z - 0.5f)) ||
                                            CheckFloor((int)(mousePoint.x - 0.5f), (int)(mousePoint.z + 0.5f)) || CheckFloor((int)(mousePoint.x - 0.5f), (int)(mousePoint.z - 0.5f)))
                                        isValid = true;
                                    else
                                    {
                                        if ((int)mousePoint.x != mousePoint.x && CheckFloor((int)(mousePoint.x + 0.5f), (int)mousePoint.z) || CheckFloor((int)(mousePoint.x + 0.5f), (int)mousePoint.z))
                                            isValid = true;
                                        else if (CheckFloor((int)mousePoint.x, (int)(mousePoint.z + 0.5f)) || CheckFloor((int)mousePoint.x, (int)(mousePoint.z - 0.5f)))
                                            isValid = true;
                                    }
                                }
                                else if (CheckFloor((int)mousePoint.x, (int)mousePoint.z)) isValid = true;
                            }

                            if (tileMode == TileMode.Floor || tileMode == TileMode.GoalFloor)
                            {
                                if (CheckFloor((int)mousePoint.x, (int)mousePoint.z)) Destroy(CheckFloor((int)mousePoint.x, (int)mousePoint.z).gameObject);
                            }
                            else if (tileMode == TileMode.NormalWall || tileMode == TileMode.Mirror || tileMode == TileMode.Glass || tileMode == TileMode.LightPole || tileMode == TileMode.LightGetter)
                            {
                                if (CheckWall(mousePoint.x, mousePoint.z)) Destroy(CheckWall(mousePoint.x, mousePoint.z).gameObject);
                                else if (CheckObject(mousePoint.x, mousePoint.z)) Destroy(CheckObject(mousePoint.x, mousePoint.z).gameObject);
                            }
                            else
                            {
                                if (CheckJackson((int)mousePoint.x, (int)mousePoint.z)) Destroy(CheckJackson((int)mousePoint.x, (int)mousePoint.z).gameObject);
                                else if (CheckObject(mousePoint.x, mousePoint.z)) Destroy(CheckObject(mousePoint.x, mousePoint.z).gameObject);
                            }
                            if (isValid)
                            {
                                if(tileMode == TileMode.CenterPos)
                                {
                                    ChangeTileMode((int)TileMode.None);
                                }
                                else
                                {
                                    GameObject newTile;
                                    if (tileMode == TileMode.Floor || tileMode == TileMode.GoalFloor)
                                    {
                                        newTile = Instantiate(currentTile, mousePoint, currentTile.transform.rotation, floors);
                                        if (floors.childCount == 1 || mousePoint.x < minX) minX = (int)mousePoint.x;
                                        if (floors.childCount == 1 || mousePoint.x > maxX) maxX = (int)mousePoint.x;
                                        if (floors.childCount == 1 || mousePoint.z < minY) minY = (int)mousePoint.z;
                                        if (floors.childCount == 1 || mousePoint.z > maxY) maxY = (int)mousePoint.z;
                                    }
                                    else if (tileMode == TileMode.NormalWall || tileMode == TileMode.Mirror || tileMode == TileMode.Glass)
                                        newTile = Instantiate(currentTile, mousePoint, currentTile.transform.rotation, walls);
                                    else if (tileMode == TileMode.StartFloor) newTile = Instantiate(currentTile, mousePoint + new Vector3(0, 1, 0), Quaternion.identity, jacksons);
                                    else if (tileMode == TileMode.LightPole) newTile = Instantiate(currentTile, mousePoint + new Vector3(0, 1, 0), Quaternion.Euler(0, 45, 0), objects);
                                    else newTile = Instantiate(currentTile, mousePoint + new Vector3(0, 1, 0), currentTile.transform.rotation, objects);
                                    newTile.GetComponent<MapEditorTile>().mapPos = new Vector2(mousePoint.x, mousePoint.z);
                                    newTile.GetComponent<BoxCollider>().enabled = true;
                                }
                            }
                            else PrintDebugText("Invalid position");
                        }
                        prevMousePoint = mousePoint;
                    }
                }
            }

            if (prevMousePoint != mousePoint && Input.GetMouseButton(1))
            {
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(mouseRay, out hit) && hit.transform.gameObject != currentTile) Destroy(hit.transform.gameObject);
                prevMousePoint = mousePoint;
            }

            if (Input.GetKeyDown(KeyCode.Z)) ChangeTileMode((int)TileMode.Floor);
            else if (Input.GetKeyDown(KeyCode.X)) ChangeTileMode((int)TileMode.NormalWall);
            else if (Input.GetKeyDown(KeyCode.C)) ChangeTileMode((int)TileMode.Mirror);
            else if (Input.GetKeyDown(KeyCode.V)) ChangeTileMode((int)TileMode.Glass);
            else if (Input.GetKeyDown(KeyCode.Q)) ChangeTileMode((int)TileMode.StartFloor);
            else if (Input.GetKeyDown(KeyCode.E)) ChangeTileMode((int)TileMode.GoalFloor);
            else if (Input.GetKeyDown(KeyCode.R)) ChangeTileMode((int)TileMode.CenterPos);
            else if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeTileMode((int)TileMode.TrueCase);
            else if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeTileMode((int)TileMode.FalseCase);
            else if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeTileMode((int)TileMode.MirrorCase);
            else if (Input.GetKeyDown(KeyCode.Alpha4)) ChangeTileMode((int)TileMode.NullCase);
            else if (Input.GetKeyDown(KeyCode.Alpha5)) ChangeTileMode((int)TileMode.Camera);
            else if (Input.GetKeyDown(KeyCode.Alpha6)) ChangeTileMode((int)TileMode.WMannequin);
            else if (Input.GetKeyDown(KeyCode.Alpha7)) ChangeTileMode((int)TileMode.BMannequin);
            else if (Input.GetKeyDown(KeyCode.Alpha8)) ChangeTileMode((int)TileMode.LightPole);
            else if (Input.GetKeyDown(KeyCode.Alpha9)) ChangeTileMode((int)TileMode.LightGetter);

        }
        if (Input.GetKeyDown(KeyCode.Space)) Camera.main.transform.position = new Vector3(0, 10, 0);
        if (Input.GetKeyDown(KeyCode.Tab)) isPanelOn = !isPanelOn;
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) prevMousePoint = new Vector3(0, -1, 0);
        controlPanel.SetActive(isPanelOn);
    }
}
