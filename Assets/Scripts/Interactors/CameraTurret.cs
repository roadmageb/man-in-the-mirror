using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTurret : MonoBehaviour, IBreakable, IPlayerInteractor
{
    private Vector2Int position;

    public void Init(Vector2Int pos)
    {
        position = pos;
    }

    public void Break()
    {
        Destroy(gameObject);
    }

    public void Interact(Vector2Int pos)
    {

    }
}
