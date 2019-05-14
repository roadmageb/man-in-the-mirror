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
		PlayerController.inst.OnPlayerMove += Interact;
	}

    public void Break()
    {
        Destroy(gameObject);
    }

    public void Interact(Vector2Int pos)
    {
		if (Position.IsInSquareArea(pos, 1))
		{
			Debug.Log("Stage Restart!");
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
