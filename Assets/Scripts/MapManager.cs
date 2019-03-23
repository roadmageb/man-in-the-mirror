using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public class MapManager : SingletonBehaviour<MapManager>
{
    public Floor floor;
    public Wall wall;
    public GameObject player;
    public Map currentMap;
    public NavMeshSurface surface;
    public Map[] stage;

    public void LoadMap(Map _newMap)
    {
        if(currentMap != null)
            Destroy(currentMap.gameObject);
        currentMap = Instantiate(_newMap);
        currentMap.transform.position = new Vector3(0, 0, 0);
        player.transform.position = currentMap.startFloor.transform.position + new Vector3(0, 1.5f, 0);
    }
    public IEnumerator Rebaker()
    {
        yield return null;
        surface.BuildNavMesh();
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
        player = GameObject.Find("Player");
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadMap(stage[0]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
