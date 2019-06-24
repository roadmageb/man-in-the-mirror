using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : SingletonBehaviour<PlayerController>
{
    public Player currentPlayer;
    public bool isPlayerMoving, isPlayerShooting, isZooming;
    private List<BulletCode> bulletList = new List<BulletCode>();
    private int bulletCount = 0;
    private Vector2Int prePos;
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

    public void CreatePlayer(Floor floor)
    {
        GameObject player = Instantiate(MapManager.inst.player, floor.transform.position + new Vector3(0, 0.1f, 0), Quaternion.identity);
        player.GetComponent<Player>().currentFloor = floor;
        MapManager.inst.players.Add(player);
        if (GameManager.nPlayer >= 0)
        {
            MapManager.inst.currentMap.clearConditions[GameManager.nPlayer].count = MapManager.inst.players.Count;
            MapManager.inst.currentMap.clearConditions[GameManager.nPlayer].IsDone();
        }
        CheckCurrentFloors();
    }

    public void CheckCurrentFloors()
    {
        int goalFloorCount = 0;
        foreach (GameObject child in MapManager.inst.players)
        {
            if (child.GetComponent<Player>().currentFloor.isGoalFloor)
                goalFloorCount++;
        }
        if (GameManager.aFloor >= 0)
        {
            MapManager.inst.currentMap.clearConditions[GameManager.aFloor].count = goalFloorCount;
            MapManager.inst.currentMap.clearConditions[GameManager.aFloor].IsDone();
        }
        if (GameManager.nFloor >= 0)
        {
            MapManager.inst.currentMap.clearConditions[GameManager.nFloor].count = goalFloorCount;
            MapManager.inst.currentMap.clearConditions[GameManager.nFloor].IsDone();
        }
    }

    //For test
    public string GetCurrentBullet()
    {
        return bulletList.Count > 0 ? bulletList[bulletCount].ToString() : null;
    }

    // Start is called before the first frame update
    void Start()
    {
        prePos = MapPos;
        bulletList.Add(BulletCode.True);
        bulletList.Add(BulletCode.Mirror);
        bulletList.Add(BulletCode.False);
        bulletList.Add(BulletCode.Mirror);
        bulletList.Add(BulletCode.False);
        bulletList.Add(BulletCode.Mirror);
        bulletList.Add(BulletCode.False);
        bulletList.Add(BulletCode.Mirror);
        bulletList.Add(BulletCode.False);
        bulletList.Add(BulletCode.Mirror);
        bulletList.Add(BulletCode.False);
    }

    // Update is called once per frame
    void Update()
    {
        if (prePos != MapPos)
        {
			//Debug.Log(MapPos);
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
                        //Debug.Log(hit.collider.gameObject.tag);
                    }
                    else if (Physics.Raycast(mouseRay, out hit) && hit.collider.gameObject.tag.Equals("floor"))
                    {
                        if (currentPlayer != null)
                            currentPlayer.MovePlayer(hit.collider.gameObject.transform.position);
                        //Debug.Log(hit.collider.gameObject.tag);
                    }
                    else if (hit.collider == null)
                    {
                        if (currentPlayer != null)
                            currentPlayer.ResetCurrentPlayer();
                    }
                }
                else if (isPlayerShooting)
                {
                    if (bulletList.Count > 0)
                    {
                        currentPlayer.Shoot(bulletList[bulletCount]);
                        bulletList.RemoveAt(bulletCount);
                    }
                }
            }
            else if (Input.GetMouseButtonDown(1) && isPlayerShooting)
            {
                StartCoroutine(Camera.main.GetComponent<CameraController>().ZoomOutFromPlayer(currentPlayer));
                currentPlayer.shootingArm.rotation = currentPlayer.armRotation;
            }
        }
    }

    void LateUpdate()
    {
        if(currentPlayer != null)
        {
            if (currentPlayer.GetComponent<NavMeshAgent>().velocity.magnitude > 0)
                transform.rotation = Quaternion.LookRotation(currentPlayer.GetComponent<NavMeshAgent>().velocity.normalized);
            if (isPlayerShooting)
            {
                Quaternion destinationRotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, currentPlayer.transform.eulerAngles.z));
                currentPlayer.transform.rotation = Quaternion.Lerp(currentPlayer.transform.rotation, destinationRotation, Time.deltaTime * 10);
                currentPlayer.shootingArm.LookAt(Camera.main.transform.forward + Camera.main.transform.position);
            }
            else
                currentPlayer.shootingArm.rotation = currentPlayer.armRotation;
        }
    }
}
