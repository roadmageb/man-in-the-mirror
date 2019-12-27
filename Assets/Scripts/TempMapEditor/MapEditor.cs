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
        /*Vector3 mousePoint = new Vector3(Mathf.Round(originPos.x - (isFloat ? 0.5f : 0)) + (isFloat ? 0.5f : 0), 0, 
            Mathf.Round(originPos.z - (isFloat ? 0.5f : 0)) + (isFloat ? 0.5f : 0));*/
        Vector3 mousePoint;
        if (isFloat)
        {
            if (!isAtPoint)
            {
                /*if(Mathf.Abs(originPos.x - mousePoint.x) > Mathf.Abs(originPos.z - mousePoint.z)) mousePoint = new Vector3(Mathf.Round(mousePoint.x), 0, mousePoint.z);
                else mousePoint = new Vector3(mousePoint.x, 0, Mathf.Round(mousePoint.z));*/
                if (Mathf.Abs(Mathf.Round(originPos.x) - originPos.x) < Mathf.Abs(Mathf.Round(originPos.y) - originPos.y))
                {
                    mousePoint = new Vector3(Mathf.Round(originPos.x), 0, Mathf.Round(originPos.z - 0.5f) + 0.5f);
                }
                else
                {

                    mousePoint = new Vector3(Mathf.Round(originPos.x - 0.5f) + 0.5f, 0, Mathf.Round(originPos.z));
                }
            }
            else
            {
                mousePoint = new Vector3(Mathf.Round(originPos.x - 0.5f) + 0.5f, 0, Mathf.Round(originPos.z - 0.5f) + 0.5f);
            }
        }
        else mousePoint = new Vector3(Mathf.Round(originPos.x), 0, Mathf.Round(originPos.z));
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
        for (int i = 0; i < floors.childCount; i++)
        {
            MapEditorTile temp = floors.GetChild(i).GetComponent<MapEditorTile>();
            if (temp.mapPos.x == x && temp.mapPos.y == y) return true;
        }
        return false;
    }
    public bool CheckJackson(int x, int y)
    {
        for (int i = 0; i < jacksons.childCount; i++)
        {
            MapEditorTile temp = jacksons.GetChild(i).GetComponent<MapEditorTile>();
            if (temp.mapPos.x == x && temp.mapPos.y == y) return true;
        }
        return false;
    }
    public bool CheckWall(float x, float y)
    {
        for (int i = 0; i < walls.childCount; i++)
        {
            MapEditorTile temp = walls.GetChild(i).GetComponent<MapEditorTile>();
            if (temp.mapPos.x == x && temp.mapPos.y == y) return true;
        }
        return false;
    }
    public bool CheckObject(float x, float y)
    {
        for (int i = 0; i < objects.childCount; i++)
        {
            MapEditorTile temp = objects.GetChild(i).GetComponent<MapEditorTile>();
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
            for (int i = 0; i < floors.childCount; i++)
            {
                MapEditorTile temp = floors.GetChild(i).GetComponent<MapEditorTile>();
                if (temp.mapPos.x < minX) minX = (int)temp.mapPos.x;
                if (temp.mapPos.y < minY) minY = (int)temp.mapPos.y;
                if (temp.mapPos.x > maxX) maxX = (int)temp.mapPos.x;
                if (temp.mapPos.y > maxY) maxY = (int)temp.mapPos.y;
            }
            MapSaveData mapSaveData = new MapSaveData();
            mapSaveData.AddObject(TileMode.None, new Vector2(Mathf.Max(maxX - minX, maxY - minY), 0));

            for (int i = 0; i < walls.childCount; i++)
            {
                MapEditorTile temp = walls.GetChild(i).GetComponent<MapEditorTile>();
                if (temp.thisTile == TileMode.NormalWall) mapSaveData.AddObject(TileMode.NormalWall, temp.mapPos);
                else mapSaveData.AddObject(TileMode.Mirror, temp.mapPos);
            }
            for (int i = 0; i < floors.childCount; i++)
            {
                MapEditorTile temp = floors.GetChild(i).GetComponent<MapEditorTile>();
                mapSaveData.AddObject(TileMode.Floor, temp.mapPos);
                if (temp.thisTile == TileMode.goalFloor) mapSaveData.AddObject(TileMode.goalFloor, temp.mapPos);
            }
            for (int i = 0; i < jacksons.childCount; i++)
            {
                MapEditorTile temp = jacksons.GetChild(i).GetComponent<MapEditorTile>();
                mapSaveData.AddObject(TileMode.StartFloor, temp.mapPos);
            }
            for (int i = 0; i < objects.childCount; i++)
            {
                MapEditorTile temp = objects.GetChild(i).GetComponent<MapEditorTile>();
                switch (temp.thisTile)
                {
                    case TileMode.TrueCase:
                        mapSaveData.AddObject(TileMode.TrueCase, temp.mapPos);
                        break;
                    case TileMode.FalseCase:
                        mapSaveData.AddObject(TileMode.FalseCase, temp.mapPos);
                        break;
                    case TileMode.MirrorCase:
                        mapSaveData.AddObject(TileMode.MirrorCase, temp.mapPos);
                        break;
                    case TileMode.NullCase:
                        mapSaveData.AddObject(TileMode.NullCase, temp.mapPos);
                        break;
                    case TileMode.Camera:
                        mapSaveData.AddObject(TileMode.Camera, temp.mapPos);
                        break;
                    case TileMode.WMannequin:
                        mapSaveData.AddObject(TileMode.WMannequin, temp.mapPos);
                        break;
                    case TileMode.BMannequin:
                        mapSaveData.AddObject(TileMode.BMannequin, temp.mapPos);
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
                if (tileMode != TileMode.Floor && tileMode != TileMode.goalFloor)
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
                else isValid = true;

                if ((tileMode == TileMode.Floor || tileMode == TileMode.goalFloor) && CheckFloor((int)mousePoint.x, (int)mousePoint.z)) isValid = false;
                else if (tileMode == TileMode.StartFloor && CheckJackson((int)mousePoint.x, (int)mousePoint.z)) isValid = false;
                else if ((tileMode == TileMode.NormalWall || tileMode == TileMode.Mirror) && CheckWall(mousePoint.x, mousePoint.z)) isValid = false;
                if (isValid)
                {
                    if (tileMode == TileMode.Floor || tileMode == TileMode.goalFloor)
                        Instantiate(currentTile, mousePoint, Quaternion.identity, floors).
                            GetComponent<MapEditorTile>().mapPos = new Vector2(mousePoint.x, mousePoint.z);
                    else if (tileMode == TileMode.NormalWall || tileMode == TileMode.Mirror)
                        Instantiate(currentTile, mousePoint, Quaternion.Euler(0, (int)mousePoint.x == mousePoint.x ? 0 : 90, 0), walls).
                            GetComponent<MapEditorTile>().mapPos = new Vector2(mousePoint.x, mousePoint.z);
                    else if(tileMode == TileMode.StartFloor)
                        Instantiate(currentTile, mousePoint, Quaternion.identity, jacksons).
                            GetComponent<MapEditorTile>().mapPos = new Vector2(mousePoint.x, mousePoint.z);
                    else
                        Instantiate(currentTile, mousePoint + new Vector3(0, 1, 0), Quaternion.identity, objects).
                            GetComponent<MapEditorTile>().mapPos = new Vector2(mousePoint.x, mousePoint.z);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab)) isPanelOn = !isPanelOn;
        controlPanel.SetActive(isPanelOn);
    }
}
