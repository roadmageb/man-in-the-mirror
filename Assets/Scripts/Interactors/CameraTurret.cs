using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTurret : MonoBehaviour, IObject, IBreakable, IPlayerInteractor
{
	[SerializeField]
	private Floor floor = null;
    public float radius = 0.5f;
	public Vector2Int Position { get { return floor != null ? floor.mapPos : throw new UnassignedReferenceException("Floor of Interactor is not assigned"); } }

    [Space(15)]
    public GameObject scatteredTurret;
    public void Init(Vector2 pos, params object[] additonal)
    {
        floor.objOnFloor = this;
		PlayerController.inst.OnPlayerMove += Interact;
        if (GameManager.aTurret >= 0) MapManager.inst.currentMap.clearConditions[GameManager.aTurret].IsDone(0, 1);
    }

    public void Break()
    {
        Instantiate(scatteredTurret, transform.position + new Vector3(0, 0.3f), transform.rotation);
        if (GameManager.nTurret >= 0) MapManager.inst.currentMap.clearConditions[GameManager.nTurret].IsDone(1);
        MapManager.inst.currentMap.RemoveObject(Position);
    }

    public void Interact(Vector2Int pos)
    {
        if(!GameManager.inst.isGameOver && PlayerController.inst.currentPlayer != null)
        {
            if (Position.IsInAdjacentArea(pos, 1) && MapManager.inst.currentMap.GetWallAtPos((Vector2)(Position + pos) / 2) == null)
            {
                GameManager.inst.GameOver();
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

    public float GetRadius()
    {
        return radius;
    }

    Vector2 IObject.GetPos()
    {
        return Position;
    }
}
