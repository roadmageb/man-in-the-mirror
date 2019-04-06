using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mannequin : MonoBehaviour, IBulletInteractor
{
	[SerializeField]
	private Mesh[] mannequinMesh = new Mesh[2];
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
				GetComponent<MeshFilter>().mesh = mannequinMesh[0];
				//Change mesh to black mannequin
			}
			else if (value == Color.white)
			{
				GetComponent<MeshFilter>().mesh = mannequinMesh[1];
				//Change mesh to white mannequin
			}
			else
			{
				Debug.LogWarning("Invalid color input");
			}
			_color = value;
		}
    }

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

    public void Init(bool isWhite)
    {
        Color = isWhite ? Color.white : Color.black;
    }
}
