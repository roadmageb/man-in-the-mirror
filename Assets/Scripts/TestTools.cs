using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestTools : MonoBehaviour
{
    public InputField floorXInput, floorYInput;
    public InputField wallXInput, wallYInput;
    public InputField objectXInput, objectYInput;
    public Text currentBullet;

    public void AddFloor()
    {
        MapManager.inst.currentMap.CreateFloor(new Vector2Int(int.Parse(floorXInput.text), int.Parse(floorYInput.text)));
    }
    public void RemoveFloor()
    {
        MapManager.inst.currentMap.RemoveFloor(new Vector2Int(int.Parse(floorXInput.text), int.Parse(floorYInput.text)));
    }
    public void AddWall()
    {
        MapManager.inst.currentMap.CreateWall(new Vector2(float.Parse(wallXInput.text), float.Parse(wallYInput.text)), WallType.Normal);
    }
    public void RemoveWall()
    {
        MapManager.inst.currentMap.RemoveWall(new Vector2(float.Parse(wallXInput.text), float.Parse(wallYInput.text)));
    }
    public void AddCase()
    {
        MapManager.inst.currentMap.CreateObject(new Vector2Int(int.Parse(objectXInput.text), int.Parse(objectYInput.text)), ObjType.Briefcase);
    }
    public void AddTurret()
    {
        MapManager.inst.currentMap.CreateObject(new Vector2Int(int.Parse(objectXInput.text), int.Parse(objectYInput.text)), ObjType.Camera);
    }
    public void AddMannequin()
    {
        MapManager.inst.currentMap.CreateObject(new Vector2Int(int.Parse(objectXInput.text), int.Parse(objectYInput.text)), ObjType.Mannequin);
    }
    public void RemoveObject()
    {
        MapManager.inst.currentMap.RemoveObject(new Vector2Int(int.Parse(objectXInput.text), int.Parse(objectYInput.text)));
    }

    public void SaveMap()
    {
        MapEditor.inst.SaveMap(MapManager.inst.currentMap);
    }
    public void LoadMap()
    {
        MapManager.inst.LoadMap(MapManager.inst.stage[1]);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentBullet.text = "Current Bullet : " + PlayerController.inst.GetCurrentBullet();
    }
}
