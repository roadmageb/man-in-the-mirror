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
        if (GameManager.aTurret >= 0) MapManager.inst.currentMap.clearConditions[GameManager.aTurret].IsDone(0, 1);
    }

    public void Break()
    {
        MapManager.inst.currentMap.RemoveObject(Position);
    }

    public void Interact(Vector2Int pos)
    {
        if(!GameManager.inst.isGameOver && PlayerController.inst.currentPlayer != null)
        {
            if (Position.IsInAdjacentArea(pos, 1) && MapManager.inst.currentMap.GetWallAtPos((Vector2)(Position + pos) / 2) == null)
            {
                GameManager.inst.isGameOver = true;
                StartCoroutine(GameManager.inst.RestartStage());
                GameManager.inst.uiGenerator.ResetAllClearUIs();
                //TODO : Restart Level
            }
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

    private void OnDestroy()
    {
        if(FindObjectOfType<PlayerController>() != null) PlayerController.inst.OnPlayerMove -= Interact;
    }
}
