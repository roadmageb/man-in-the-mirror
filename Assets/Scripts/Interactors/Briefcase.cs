using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Briefcase : MonoBehaviour, IObject, IPlayerInteractor
{
	[SerializeField]
	private Floor floor = null;
    public BulletCode dropBullet;
	public Vector2Int Position { get { return floor != null ? floor.mapPos : throw new UnassignedReferenceException("Floor of Interactor is not assigned"); } }

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
        if (GameManager.aCase >= 0) MapManager.inst.currentMap.clearConditions[GameManager.aCase].IsDone(0, 1);
        this.floor = floor;
        floor.objOnFloor = this;
		PlayerController.inst.OnPlayerMove += Interact;
	}

    public void SetBullet(BulletCode _dropBullet)
    {
        dropBullet = _dropBullet;
    }

	public void Interact(Vector2Int position)
	{
		Debug.Log(Position + " " + position);
        if (Position == position)
        {
            if (dropBullet != BulletCode.None)
                PlayerController.inst.AddBullet(dropBullet);
            if (GameManager.aCase >= 0)
                MapManager.inst.currentMap.clearConditions[GameManager.aCase].IsDone(1);
            if (GameManager.nCase >= 0)
                MapManager.inst.currentMap.clearConditions[GameManager.nCase].IsDone(1);
			Destroy(gameObject);
		}
	}

    ObjType IObject.GetType()
    {
        return ObjType.Briefcase;
    }
}
