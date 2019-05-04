using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Briefcase : MonoBehaviour, IObject, IPlayerInteractor
{
	[SerializeField]
	private Floor floor = null;
	public Vector2Int Position { get { return floor != null ? floor.mapPos : throw new UnassignedReferenceException("Floor of Interactor is not assigned"); } }

    public GameObject GetObject()
    {
        return gameObject;
    }

    public Vector2 GetPos()
    {
        return new Vector2(transform.position.x, transform.position.z);
    }

    public void Init(Floor floor)
	{
		this.floor = floor;
		PlayerController.inst.OnPlayerMove += Interact;
	}

	public void Interact(Vector2Int position)
	{
		Debug.Log(Position + " " + position);
		if (Position == position)
		{
			IngameManager.inst.BriefcaseCount++;
			Destroy(gameObject);
		}
	}

    ObjType IObject.GetType()
    {
        return ObjType.Briefcase;
    }
}
