using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public class MapEditor : SingletonBehaviour<MapEditor>
{
    public Floor floor;
    public Wall wall;
    public Map currentMap;
    public Map[] stage;
    public MapEditorTile tile;

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
        else
        {
            PrefabUtility.SaveAsPrefabAsset(_newMap.gameObject, localPath);
            Debug.Log("Map saved at " + localPath);
        }
    }

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        LoadMap(stage[0]);
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
                Instantiate(tile, new Vector3(i, 0, j), Quaternion.identity).GetComponent<MapEditorTile>().mapPos = new Vector2(i, j);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(mouseRay, out hit))
            {
                Debug.Log(hit.collider.gameObject.transform.position);
            }
        }
    }
}
