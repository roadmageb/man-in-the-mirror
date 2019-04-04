using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mannequin : MonoBehaviour, IBulletInteractor
{
    public Color Color {
        get
        {
            return GetComponent<MeshRenderer>().material.color;
        }
        private set
        {
            GetComponent<MeshRenderer>().material.color = value;
        }
    }

    public void Interact(Bullet bullet)
    {
        if (bullet is TruthBullet)
        {
            GetComponent<MeshRenderer>().material.color = Color.white;
        }
        if (bullet is FakeBullet)
        {
            GetComponent<MeshRenderer>().material.color = Color.black;
        }
    }

    public void Init(bool isWhite)
    {
        Color = isWhite ? Color.white : Color.black;
    }
}
