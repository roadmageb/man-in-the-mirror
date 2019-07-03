using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Briefcase : MonoBehaviour, IObject, IPlayerInteractor
{
	[SerializeField]
	private Floor floor = null;
    public BulletCode dropBullet;
    public GameObject table;
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
        if (dropBullet == BulletCode.False)
        {
            table.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else if (dropBullet == BulletCode.Mirror)
        {
            table.GetComponent<MeshRenderer>().material.color = Color.gray;
        }
        else if (dropBullet == BulletCode.True)
        {
            table.GetComponent<MeshRenderer>().material.color = Color.green;
        }
        else
        {
            table.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        PlayerController.inst.OnPlayerMove -= Interact;
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
            floor.objOnFloor = null;
			Destroy(gameObject);
		}
	}

    ObjType IObject.GetType()
    {
        return ObjType.Briefcase;
    }
}
