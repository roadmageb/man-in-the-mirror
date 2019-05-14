using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class MapEditor : SingletonBehaviour<MapEditor>
{
    public Map currentMap;
    public Map[] stage;
    public MapEditorTile tile;
    public enum TileMode { None, Floor, NormalWall, Mirror, StartFloor };
    TileMode currentMode;
    public Text modeSign;
    public GameObject startSign;
    public Dictionary<Floor, GameObject> startSigns;
    public GameObject mapSizeSetter;
    public GameObject mapEditorTiles;

    public Material editWallMat;
    public Material realWallMat;
    
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
        string localPath = "Assets/SavedMap_" + time.ToShortDateString() + "-" + time.Hour + "-" + time.Minute + "-" + time.Second + ".prefab";
        if (AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject)))
            Debug.Log("Object with same name already exists.");
        else if(currentMap.startFloors.Count == 0)
            Debug.Log("There is no start floor.");
        else
        {
            foreach(Transform child in currentMap.walls.transform)
            {
                child.gameObject.GetComponent<MeshRenderer>().material = realWallMat;
            }
            PrefabUtility.SaveAsPrefabAsset(_newMap.gameObject, localPath);
            Debug.Log("Map saved at " + localPath);
            foreach (Transform child in currentMap.walls.transform)
            {
                child.gameObject.GetComponent<MeshRenderer>().material = editWallMat;
            }
        }
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
        currentMap.maxMapSize = Mathf.Max(x, y);
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

    private void Awake()
    {
        MapManager.inst.isMapEditingOn = true;
        isEditorStarted = false;
        isCreateMode = true;
        startSigns = new Dictionary<Floor, GameObject>();
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
                else if(currentMode == TileMode.NormalWall)
                {
                    if (isCreateMode)
                    {
                        Debug.Log(wallPos);
                        currentMap.CreateWall(wallPos, WallType.Normal);
                        if(currentMap.GetWallAtPos(wallPos) != null)
                            currentMap.GetWallAtPos(wallPos).gameObject.GetComponent<MeshRenderer>().material = editWallMat;
                    }
                    else
                        currentMap.RemoveWall(wallPos);
                }
                else if (currentMode == TileMode.Mirror)
                {
                    if (isCreateMode)
                    {
                        Debug.Log(wallPos);
                        currentMap.CreateWall(wallPos, WallType.Mirror);
                        if (currentMap.GetWallAtPos(wallPos) != null)
                            currentMap.GetWallAtPos(wallPos).gameObject.GetComponent<MeshRenderer>().material = editWallMat;
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
            }
        }
    }
}
