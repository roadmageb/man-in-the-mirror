using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Briefcase : MonoBehaviour, IObject, IPlayerInteractor
{
    [SerializeField]
    public Vector2 position;
    public float radius = 0.5f;
    public BulletCode dropBullet;
    public GameObject table;

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

    #region IPlayerInteractor
    public void Interact(Vector2Int position)
	{
        if(!GameManager.inst.isGameOver)
        {
            //Debug.Log(Position + " " + position);
            if (this.position == position)
            {
                if (dropBullet != BulletCode.NULL)
                    PlayerController.inst.AddBullet(dropBullet);
                if (GameManager.nCase >= 0)
                    MapManager.inst.currentMap.clearConditions[GameManager.nCase].IsDone(1);
                MapManager.inst.currentMap.RemoveObject(position);
            }
        }
	}

    #endregion

    private void OnDestroy()
    {
        if (FindObjectOfType<PlayerController>() != null) PlayerController.inst.OnPlayerMove -= Interact;
    }

    #region IObject
    /// <param name="additonal">
    /// <br/>0: (BulletCode) bullet type to drop
    /// </param>
    public void Init(Vector2 pos, params object[] additonal)
    {
        if (GameManager.aCase >= 0)
        {
            MapManager.inst.currentMap.clearConditions[GameManager.aCase].IsDone(0, 1);
            //Debug.Log("init brief");
        }
        position = pos;
        SetBullet((BulletCode)additonal[0]);
        PlayerController.inst.OnPlayerMove += Interact;
    }

    public object[] GetAdditionals()
    {
        return new object[]
        {
            dropBullet
        };
    }

    public GameObject GetObject()
    {
        return gameObject;
    }

    public Vector2 GetPos()
    {
        return position;
    }

    ObjType IObject.GetType()
    {
        return ObjType.Briefcase;
    }

    public float GetRadius()
    {
        return radius;
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, radius);
    }   
}
