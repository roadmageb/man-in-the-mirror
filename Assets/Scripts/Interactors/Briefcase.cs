using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Briefcase : MIMObject, IPlayerInteractor
{
	[SerializeField]
	private Floor floor = null;
    public BulletCode dropBullet;
    public GameObject table;
	public Vector2Int Position { get { return floor != null ? floor.mapPos : throw new UnassignedReferenceException("Floor of Interactor is not assigned"); } }

    public override void Init()
	{
        base.Init();
        if (GameManager.aCase >= 0)
        {
            MapManager.inst.currentMap.clearConditions[GameManager.aCase].IsDone(0, 1);
            //Debug.Log("init brief");
        }
        //this.floor = floor;
        //floor.objOnFloor = this;
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

    public void Interact(Vector2Int position)
	{
        if(!GameManager.inst.isGameOver)
        {
            //Debug.Log(Position + " " + position);
            if (Position == position)
            {
                if (dropBullet != BulletCode.NULL)
                    PlayerController.inst.AddBullet(dropBullet);
                if (GameManager.nCase >= 0)
                    MapManager.inst.currentMap.clearConditions[GameManager.nCase].IsDone(1);
                floor.objOnFloor = null;
                MapManager.inst.currentMap.RemoveObject(Position);
            }
        }

	}

    private void OnDestroy()
    {
        if (FindObjectOfType<PlayerController>() != null) PlayerController.inst.OnPlayerMove -= Interact;
    }
}
