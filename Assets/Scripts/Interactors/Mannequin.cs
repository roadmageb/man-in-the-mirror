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
        if (bullet is TruthBullet)
        {
            Color = Color.white;
        }
        if (bullet is FakeBullet)
        {
			Color = Color.black;
        }
    }

    public void Init(Floor floor)
    {
        this.floor = floor;
        transform.Rotate(new Vector3(0, Random.Range(0, 4) * 90, 0));
    }

    public void SetColor(bool isWhite)
    {
        Color = isWhite ? Color.white : Color.black;
        this.isWhite = isWhite;
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
