using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestTools : MonoBehaviour
{
    public Text currentBullet, clear;
    public InputField xInput, yInput;

    public void AddFloor()
    {
        MapManager.inst.currentMap.CreateFloor(new Vector2Int(int.Parse(xInput.text), int.Parse(yInput.text)));
    }
    public void RemoveFloor()
    {
        MapManager.inst.currentMap.RemoveFloor(new Vector2Int(int.Parse(xInput.text), int.Parse(yInput.text)));
    }
    public void AddWall()
    {
        MapManager.inst.currentMap.CreateWall(new Vector2(float.Parse(xInput.text), float.Parse(yInput.text)), WallType.Normal);
    }
    public void RemoveWall()
    {
        MapManager.inst.currentMap.RemoveWall(new Vector2(float.Parse(xInput.text), float.Parse(yInput.text)));
    }
    public void AddTurret()
    {
        MapManager.inst.currentMap.CreateObject(new Vector2Int(int.Parse(xInput.text), int.Parse(yInput.text)), ObjType.Camera);
    }
    public void AddCase()
    {
        MapManager.inst.currentMap.CreateObject(new Vector2Int(int.Parse(xInput.text), int.Parse(yInput.text)), ObjType.Briefcase);
    }
    public void AddBlackMannequin()
    {
        MapManager.inst.currentMap.CreateObject(new Vector2Int(int.Parse(xInput.text), int.Parse(yInput.text)), ObjType.Mannequin, false);
    }
    public void AddWhiteMannequin()
    {
        MapManager.inst.currentMap.CreateObject(new Vector2Int(int.Parse(xInput.text), int.Parse(yInput.text)), ObjType.Mannequin, true);
    }
    public void RemoveObject()
    {
        MapManager.inst.currentMap.RemoveObject(new Vector2Int(int.Parse(xInput.text), int.Parse(yInput.text)));
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
