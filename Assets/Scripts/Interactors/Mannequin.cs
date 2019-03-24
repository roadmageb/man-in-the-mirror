using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mannequin : MonoBehaviour, IInteractor
{
    // 일단은 이렇게 해두긴 하는데 나중에 마네킹 매니저같은게 생기면 거기에 두고 쓰는게 편할듯
    public Material white;
    public Material black;

    public bool isWhite;

    private SkinnedMeshRenderer[] _mats;

    private void Start()
    {
        _mats = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    //public Color Color {
    //    get
    //    {
    //        return GetComponent<MeshRenderer>().material.color;
    //    }
    //    private set
    //    {
    //        GetComponent<MeshRenderer>().material.color = value;
    //    }
    //}

    public void Interact(Bullet bullet)
    {
        if (bullet is TruthBullet)
        {
            //GetComponent<MeshRenderer>().material.color = Color.white;
            isWhite = true;
            foreach (var mat in _mats)
                mat.material = white;
        }
        if (bullet is FakeBullet)
        {
            //GetComponent<MeshRenderer>().material.color = Color.black;
            isWhite = false;
            foreach (var mat in _mats)
                mat.material = black;
        }
    }

    public void Init(bool isWhite)
    {
        //Color = isWhite ? Color.white : Color.black;
        this.isWhite = isWhite;
        foreach (var mat in _mats)
            mat.material = isWhite ? white : black;
    }
}
