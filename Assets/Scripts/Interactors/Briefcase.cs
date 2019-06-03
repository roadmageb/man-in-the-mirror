using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Briefcase : MonoBehaviour, IObject, IPlayerInteractor
{
	[SerializeField]
	private Floor floor = null;
	public Vector2Int Position { get { return floor != null ? floor.mapPos : throw new UnassignedReferenceException("Floor of Interactor is not assigned"); } }
    private int aCase;
    private int nCase;

    public GameObject GetObject()
    {
        return gameObject;
    }

    public Vector2Int GetPos()
    {
        return new Vector2Int((int)transform.position.x, (int)transform.position.z);
    }

    public void Init(Floor floor)
	{
        aCase = GameManager.inst.clearIndex[(int)ClearType.AllCase];
        nCase = GameManager.inst.clearIndex[(int)ClearType.NCase];
        if (aCase >= 0) MapManager.inst.currentMap.clearConditions[aCase].goal++;
        this.floor = floor;
		PlayerController.inst.OnPlayerMove += Interact;
	}

	public void Interact(Vector2Int position)
	{
		Debug.Log(Position + " " + position);
		if (Position == position)
		{
            if (aCase >= 0)
            {
                MapManager.inst.currentMap.clearConditions[aCase].count++;
                MapManager.inst.currentMap.clearConditions[aCase].IsDone();
            }
            if (nCase >= 0)
            {
                MapManager.inst.currentMap.clearConditions[nCase].count++;
                MapManager.inst.currentMap.clearConditions[nCase].IsDone();
            }
			Destroy(gameObject);
		}
	}

    ObjType IObject.GetType()
    {
        return ObjType.Briefcase;
    }
}
