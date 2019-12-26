using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.SceneManagement;

public class MapEditor : SingletonBehaviour<MapEditor>
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
        public List<BulletCode> bullets;
        public string comments = null;
        public MapSaveData()
        {
            objects = new List<ObjectData>();
            clears = new List<ClearData>();
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

    public GameObject[] tiles;
    public Transform walls, floors, objects, jacksons;
    int startFloors = 0;
    GameObject currentTile = null, controlPanel;
    bool isPanelOn = false;
    bool isFloat = false, isAtPoint = false;
    TileMode tileMode = 0;

    Vector3 GetMousePoint()
    {
        Vector3 originPos = Camera.main.ScreenPointToRay(Input.mousePosition).origin;
        Vector3 mousePoint = new Vector3(Mathf.Round(originPos.x - (isFloat ? 0.5f : 0)) + (isFloat ? 0.5f : 0), 0, 
            Mathf.Round(originPos.z - (isFloat ? 0.5f : 0)) + (isFloat ? 0.5f : 0));
        if (!isAtPoint)
        {
            if(Mathf.Abs(originPos.x - mousePoint.x) > Mathf.Abs(originPos.z - mousePoint.z)) mousePoint = new Vector3(Mathf.Round(mousePoint.x), 0, mousePoint.z);
            else mousePoint = new Vector3(mousePoint.x, 0, Mathf.Round(mousePoint.z));
        }
        return mousePoint;
    }

    public void ChangeTileMode(int _tileMode)
    {
        tileMode = (TileMode)_tileMode;
        if(currentTile != null) currentTile.SetActive(false);
        currentTile = tiles[_tileMode - 1];
        currentTile.SetActive(true);
        if ((TileMode)_tileMode == TileMode.NormalWall || (TileMode)_tileMode == TileMode.Mirror)
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

    public bool CheckFloor(int x, int y)
    {
        for(int i = 0; i < floors.childCount; i++)
        {
            MapEditorTile temp = floors.GetChild(i).GetComponent<MapEditorTile>();
            if (temp.mapPos.x == x && temp.mapPos.y == y) return true;
        }
        return false;
    }
    public bool CheckObject(int x, int y)
    {
        for (int i = 0; i < objects.childCount; i++)
        {
            MapEditorTile temp = floors.GetChild(i).GetComponent<MapEditorTile>();
            if (temp.mapPos.x == x && temp.mapPos.y == y) return true;
        }
        return false;
    }

    public void SaveMap()
    {
        /* 맵 저장 시 반드시 승리 조건 작성할 것
         * 목표가 '모든'일 경우 승리 목표는 초기 맵 기준으로 작성
         */
        string mapName = "abcd";
        string localPath = "Assets/Resources/Stages/stage" + mapName + ".json";
        if (jacksons.childCount == 0) Debug.Log("There is no start floor.");
        else
        {
            int minX = 0, minY = 0, maxX = 0, maxY = 0;
            foreach (MapEditorTile child in floors)
            {
                if (child.mapPos.x < minX) minX = (int)child.mapPos.x;
                if (child.mapPos.y < minY) minY = (int)child.mapPos.y;
                if (child.mapPos.x > maxX) maxX = (int)child.mapPos.x;
                if (child.mapPos.y > maxY) maxY = (int)child.mapPos.y;

            }
            MapSaveData mapSaveData = new MapSaveData();
            mapSaveData.AddObject(TileMode.None, new Vector2(Mathf.Max(maxX - minX, maxY - minY), 0));

            foreach (MapEditorTile child in walls)
            {
                if (child.thisTile == TileMode.NormalWall) mapSaveData.AddObject(TileMode.NormalWall, child.mapPos);
                else mapSaveData.AddObject(TileMode.Mirror, child.mapPos);
            }
            foreach (MapEditorTile child in floors)
            {
                mapSaveData.AddObject(TileMode.Floor, child.mapPos);
                if (child.thisTile == TileMode.goalFloor) mapSaveData.AddObject(TileMode.goalFloor, child.mapPos);
            }
            foreach (MapEditorTile child in jacksons)
            {
                mapSaveData.AddObject(TileMode.StartFloor, child.mapPos);
            }
            foreach (MapEditorTile child in objects)
            {
                switch (child.thisTile)
                {
                    case TileMode.TrueCase:
                        mapSaveData.AddObject(TileMode.TrueCase, child.mapPos);
                        break;
                    case TileMode.FalseCase:
                        mapSaveData.AddObject(TileMode.FalseCase, child.mapPos);
                        break;
                    case TileMode.MirrorCase:
                        mapSaveData.AddObject(TileMode.MirrorCase, child.mapPos);
                        break;
                    case TileMode.NullCase:
                        mapSaveData.AddObject(TileMode.NullCase, child.mapPos);
                        break;
                    case TileMode.Camera:
                        mapSaveData.AddObject(TileMode.Camera, child.mapPos);
                        break;
                    case TileMode.WMannequin:
                        mapSaveData.AddObject(TileMode.WMannequin, child.mapPos);
                        break;
                    case TileMode.BMannequin:
                        mapSaveData.AddObject(TileMode.BMannequin, child.mapPos);
                        break;
                }
            }


            /*for (int i = 0; i < currentMap.clearConditions.Count; i++)
                mapSaveData.AddClears(currentMap.clearConditions[i].type, currentMap.clearConditions[i].goal);
            for (int i = 0; i < currentMap.initialBullets.Count; i++)
                mapSaveData.bullets.Add(currentMap.initialBullets[i]);

            mapSaveData.comments = currentMap.comments;*/

            if (File.Exists(localPath))
            {
                Debug.Log("File Exists");
                File.Delete(localPath);
            }

            File.WriteAllText(localPath, JsonConvert.SerializeObject(mapSaveData));
            Debug.Log("Map saved at " + localPath);
        }
    }




    private void Awake()
    {
        controlPanel = GameObject.Find("ControlPanel");
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = Instantiate(tiles[i]);
            tiles[i].SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool isValid;
    // Update is called once per frame
    void Update()
    {
        if (!isPanelOn && currentTile != null)
        {
            Vector3 mousePoint = GetMousePoint();
            isValid = false;
            currentTile.transform.position = mousePoint;
            if (Input.GetMouseButtonDown(0))
            {
                if (tileMode != TileMode.Floor)
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
                else if (!CheckFloor((int)mousePoint.x, (int)mousePoint.z)) isValid = true;
                if (isValid)
                {
                    if(tileMode == TileMode.Floor || tileMode == TileMode.goalFloor)
                    {
                        Instantiate(currentTile, mousePoint, Quaternion.identity, floors).GetComponent<MapEditorTile>().mapPos = new Vector2(mousePoint.x, mousePoint.z);
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab)) isPanelOn = !isPanelOn;
        controlPanel.SetActive(isPanelOn);
    }
}
