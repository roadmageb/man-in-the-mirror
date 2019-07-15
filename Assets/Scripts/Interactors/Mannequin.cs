using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mannequin : MonoBehaviour, IObject, IBulletInteractor
{
    [SerializeField] private SkinnedMeshRenderer[] renderers = new SkinnedMeshRenderer[2];
    [SerializeField] private Material[] mannequinMaterial = new Material[2];
    [SerializeField] private Floor floor;
    private Color _color;
    public Color Color {
        get
        {
			return _color;
        }
        private set
        {
			if (value == Color.black)
			{
                foreach (var renderer in renderers)
                {
                    renderer.material = mannequinMaterial[0];
                }
				//Change mesh to black mannequin
			}
			else if (value == Color.white)
			{
                foreach (var renderer in renderers)
                {
                    renderer.material = mannequinMaterial[1];
                }
                //Change mesh to white mannequin
            }
			else
			{
				Debug.LogWarning("Invalid color input");
			}
			_color = value;
		}
    }
    public bool isWhite;

    public void Interact(Bullet bullet)
    {
        Color tempColor = Color;
        if (bullet is TruthBullet && tempColor == Color.black)
        {
            Color = Color.white;
            isWhite = true;
            if (GameManager.white >= 0)
                MapManager.inst.currentMap.clearConditions[GameManager.white].IsDone(1);
            if (GameManager.black >= 0)
                MapManager.inst.currentMap.clearConditions[GameManager.black].IsDone(-1);
        }
        else if (bullet is FakeBullet && tempColor == Color.white)
        {
			Color = Color.black;
            isWhite = false;
            if (GameManager.black >= 0)
                MapManager.inst.currentMap.clearConditions[GameManager.black].IsDone(1);
            if (GameManager.white >= 0)
                MapManager.inst.currentMap.clearConditions[GameManager.white].IsDone(-1);
        }
    }

    public void Init(Floor floor)
    {
        this.floor = floor;
        floor.objOnFloor = this;
        transform.Rotate(new Vector3(0, Random.Range(0, 4) * 90, 0));
        isWhite = true;
        Color = Color.white;
        if (GameManager.white >= 0) MapManager.inst.currentMap.clearConditions[GameManager.white].IsDone(1);
    }

    public void SetColor(bool isWhite)
    {
        Color = isWhite ? Color.white : Color.black;
        this.isWhite = isWhite;
        if (GameManager.black >= 0 && !isWhite)
        {
            MapManager.inst.currentMap.clearConditions[GameManager.black].count++;
            if (GameManager.white >= 0) MapManager.inst.currentMap.clearConditions[GameManager.white].count--;
        }
    }

    #region IObject Override
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
        return ObjType.Mannequin;
    }
    #endregion
}
