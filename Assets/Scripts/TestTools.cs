using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestTools : MonoBehaviour
{
    public InputField floorXInput, floorYInput;

    public InputField wall1XInput, wall1YInput, wall2XInput, wall2YInput;

    public void AddFloor()
    {
        MapManager.inst.currentMap.CreateFloor(int.Parse(floorXInput.text), int.Parse(floorYInput.text));
    }
    public void RemoveFloor()
    {
        MapManager.inst.currentMap.RemoveFloor(int.Parse(floorXInput.text), int.Parse(floorYInput.text));
    }
    public void AddWall()
    {
        MapManager.inst.currentMap.CreateWall(
            MapManager.inst.currentMap.GetFloorAtPos(int.Parse(wall1XInput.text), int.Parse(wall1YInput.text)),
            MapManager.inst.currentMap.GetFloorAtPos(int.Parse(wall2XInput.text), int.Parse(wall2YInput.text)));
    }
    public void RemoveWall()
    {
        MapManager.inst.currentMap.RemoveWall(
            MapManager.inst.currentMap.GetFloorAtPos(int.Parse(wall1XInput.text), int.Parse(wall1YInput.text)),
            MapManager.inst.currentMap.GetFloorAtPos(int.Parse(wall2XInput.text), int.Parse(wall2YInput.text)));
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
        
    }
}
