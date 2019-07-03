using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTurret : MonoBehaviour, IObject, IBreakable, IPlayerInteractor
{
	[SerializeField]
	private Floor floor = null;
	public Vector2Int Position { get { return floor != null ? floor.mapPos : throw new UnassignedReferenceException("Floor of Interactor is not assigned"); } }

	public void Init(Floor floor)
    {
		this.floor = floor;
        floor.objOnFloor = this;
		PlayerController.inst.OnPlayerMove += Interact;
	}

    public void Break()
    {
        if (GameManager.aTurret >= 0)
            MapManager.inst.currentMap.clearConditions[GameManager.aTurret].IsDone(1);
        if (GameManager.nTurret >= 0)
            MapManager.inst.currentMap.clearConditions[GameManager.nTurret].IsDone(1);
        MapManager.inst.currentMap.RemoveObject(Position);
    }
    
    private void OnDestroy()
    {
        PlayerController.inst.OnPlayerMove -= Interact;
    }

    public void Interact(Vector2Int pos)
    {
		if (Position.IsInAdjacentArea(pos, 1) && MapManager.inst.currentMap.GetWallAtPos((Vector2)(Position + pos) / 2) == null)
		{
            StartCoroutine(GameManager.inst.RestartStage());
            
            //TODO : Restart Level
        }
    }

    public GameObject GetObject()
    {
        return gameObject;
    }

    public Vector2Int GetPos()
    {
        return new Vector2Int((int)transform.position.x, (int)transform.position.z);
    }

    ObjType IObject.GetType()
    {
        return ObjType.Camera;
    }
}
