using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : SingletonBehaviour<GameManager>
{
    private List<IPlayerInteractor> playerInteractors; 

    public void UpdateInteractorList()
    {
        playerInteractors = FindObjectsOfType<MonoBehaviour>().OfType<IPlayerInteractor>().ToList();
    }
}
