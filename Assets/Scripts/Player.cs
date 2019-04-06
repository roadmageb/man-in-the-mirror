using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public void SetCurrentPlayer()
    {
        transform.localScale = new Vector3(1, 2, 1);
    }
    public void ResetCurrentPlayer()
    {
        transform.localScale = new Vector3(1, 1, 1);
        PlayerController.inst.currentPlayer = null;
    }
    public void MovePlayer()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
