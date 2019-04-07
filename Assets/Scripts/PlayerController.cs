using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : SingletonBehaviour<PlayerController>
{
    public Player currentPlayer;
    public bool isPlayerMoving;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isPlayerMoving)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(mouseRay, out hit) && hit.collider.gameObject.tag.Equals("Player"))
            {
                if(currentPlayer != null)
                    currentPlayer.ResetCurrentPlayer();
                currentPlayer = hit.transform.gameObject.GetComponent<Player>();
                StartCoroutine(currentPlayer.SetCurrentPlayer());
                Debug.Log(hit.collider.gameObject.tag);
            }
            else if (Physics.Raycast(mouseRay, out hit) && hit.collider.gameObject.tag.Equals("floor"))
            {
                if (currentPlayer != null)
                    currentPlayer.MovePlayer(hit.collider.gameObject.transform.position);
                Debug.Log(hit.collider.gameObject.tag);
            }
            else if(hit.collider == null)
            {
                if (currentPlayer != null)
                    currentPlayer.ResetCurrentPlayer();
            }
        }
    }
}
