using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : SingletonBehaviour<PlayerController>
{
    public Player currentPlayer;
    public bool isPlayerMoving;
    public bool isPlayerShooting;
    public bool isZooming;
    [SerializeField] private Vector2Int prePos;
    public Vector2Int MapPos
    {
        get
        {
            Vector2Int pos = Vector2Int.zero;
            if (currentPlayer)
            {
                pos.x = Mathf.RoundToInt(currentPlayer.transform.position.x);
                pos.y = Mathf.RoundToInt(currentPlayer.transform.position.z);   
            }
            return pos;
        }
    }

	public event Action<Vector2Int> OnPlayerMove;

    // Start is called before the first frame update
    void Start()
    {
        prePos = MapPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (prePos != MapPos)
        {
			Debug.Log(MapPos);
			OnPlayerMove?.Invoke(MapPos);
			prePos = MapPos;
		}

        //Control player only if camera is not zooming in to or out from the current player
        if (!isZooming)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //Move the current player.
                if(!isPlayerMoving && !isPlayerShooting)
                {
                    Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(mouseRay, out hit) && hit.collider.gameObject.tag.Equals("Player"))
                    {
                        if (currentPlayer != null)
                            currentPlayer.ResetCurrentPlayer();
                        currentPlayer = hit.transform.gameObject.GetComponent<Player>();
                        StartCoroutine(currentPlayer.SetCurrentPlayer());
                        StartCoroutine(currentPlayer.CountPlayerClick(Time.time));
                        Debug.Log(hit.collider.gameObject.tag);
                    }
                    else if (Physics.Raycast(mouseRay, out hit) && hit.collider.gameObject.tag.Equals("floor"))
                    {
                        if (currentPlayer != null)
                            currentPlayer.MovePlayer(hit.collider.gameObject.transform.position);
                        Debug.Log(hit.collider.gameObject.tag);
                    }
                    else if (hit.collider == null)
                    {
                        if (currentPlayer != null)
                            currentPlayer.ResetCurrentPlayer();
                    }
                }
            }
            else if (Input.GetMouseButtonDown(1) && isPlayerShooting)
            {
                StartCoroutine(Camera.main.GetComponent<CameraController>().ZoomOutFromPlayer());
            }
        }
    }

    void LateUpdate()
    {
        if (currentPlayer != null && currentPlayer.GetComponent<NavMeshAgent>().velocity.magnitude > 0)
            transform.rotation = Quaternion.LookRotation(currentPlayer.GetComponent<NavMeshAgent>().velocity.normalized);
        if (isPlayerShooting)
        {
            Quaternion destinationRotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, currentPlayer.transform.eulerAngles.z));
            currentPlayer.transform.rotation = Quaternion.Lerp(currentPlayer.transform.rotation, destinationRotation, Time.deltaTime * 10);
            Debug.Log(currentPlayer.shootingArm.transform.position);
            Debug.Log(currentPlayer.shootingArm.transform.eulerAngles);
            Debug.Log(Camera.main.transform.position);
            Debug.Log(Camera.main.transform.eulerAngles);
        }
    }
}
