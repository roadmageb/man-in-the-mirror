using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class MapEditor : SingletonBehaviour<MapEditor>
{
    public Floor floor;
    public Wall wall;
    public Map currentMap;
    public Map[] stage;
    public MapEditorTile tile;
    public enum TileMode { None, Floor, Wall };
    TileMode currentMode;
    public Text modeSign;
    public GameObject clickSign;
    public GameObject startSign;
    public GameObject mapSizeSetter;
    public GameObject mapEditorTiles;

    Vector2Int[] wallInputFloors;
    bool isWallClicked;
    bool isEditorStarted;
    bool isCreateMode;
    bool isStartPositionSetter;

    public void LoadMap(Map _newMap)
    {
        if (currentMap != null)
            Destroy(currentMap.gameObject);
        currentMap = Instantiate(_newMap);
        currentMap.transform.position = new Vector3(0, 0, 0);
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
        else if(currentMap.startFloor == null)
            Debug.Log("There is no start floor.");
        else
        {
            PrefabUtility.SaveAsPrefabAsset(_newMap.gameObject, localPath);
            Debug.Log("Map saved at " + localPath);
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
        if (isStartPositionSetter)
            modeSign.text = "Set start position";
        else if (isCreateMode)
            modeSign.text = sign + " Create";
        else
            modeSign.text = sign + " Destroy";
    }
    public void SetStartPosition()
    {
        if(!isStartPositionSetter)
            isStartPositionSetter = true;
        else
            isStartPositionSetter = false;
        SetModeSign();
    }

    private void Awake()
    {
        MapManager.inst.isMapEditingOn = true;
        clickSign.SetActive(false);
        startSign.SetActive(false);
        isEditorStarted = false;
        isCreateMode = true;
        isStartPositionSetter = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadMap(stage[0]);
        wallInputFloors = new Vector2Int[2];
        isWallClicked = false;
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
                Vector2Int clickedPos = new Vector2Int((int)hit.transform.position.x, (int)hit.transform.position.z);
                if (isStartPositionSetter)
                {
                    currentMap.startFloor = currentMap.GetFloorAtPos(clickedPos);
                    startSign.SetActive(true);
                    startSign.transform.position = currentMap.startFloor.transform.position + new Vector3(0, 1, 0);
                    isStartPositionSetter = false;
                    SetModeSign();
                }
                else if(currentMode == TileMode.Floor)
                {
                    if (isCreateMode)
                        currentMap.CreateFloor(clickedPos);
                    else
                        currentMap.RemoveFloor(clickedPos);
                }
                else if(currentMode == TileMode.Wall)
                {
                    if (!isWallClicked)
                    {
                        clickSign.SetActive(true);
                        wallInputFloors[0] = clickedPos;
                        clickSign.transform.position = new Vector3(clickedPos.x, 1, clickedPos.y);
                        isWallClicked = true;
                    }
                    else
                    {
                        wallInputFloors[1] = clickedPos;
                        if (isCreateMode)
                            currentMap.CreateWall(currentMap.GetFloorAtPos(wallInputFloors[0]), currentMap.GetFloorAtPos(wallInputFloors[1]));
                        else
                            currentMap.RemoveWall(currentMap.GetFloorAtPos(wallInputFloors[0]), currentMap.GetFloorAtPos(wallInputFloors[1]));
                        clickSign.SetActive(false);
                        isWallClicked = false;
                    }
                }
            }
        }
    }
}
