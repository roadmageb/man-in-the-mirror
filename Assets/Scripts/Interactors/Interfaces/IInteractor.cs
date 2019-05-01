using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBulletInteractor
{
    void Interact(Bullet bullet);
}

public interface IPlayerInteractor
{
    void Interact(Vector2Int position);
}