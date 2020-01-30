using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : SingletonBehaviour<PlayerController>
{
    public Player currentPlayer;
    public List<BulletCode> bulletList = new List<BulletCode>();
    public float radius = 0.4f;
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

    public Coroutine zoomReady = null;

    public GameObject CreatePlayer(Floor floor)
    {
        foreach (var obj in MapManager.inst.players)
        {
            if (obj.GetComponent<Player>().currentFloor == floor)
            {
                Debug.Log("Player already exists on that floor.");
                return null;
            }
        }
        GameObject player = Instantiate(MapManager.inst.player, floor.transform.position + new Vector3(0, 0.1f, 0), Quaternion.identity);
        player.GetComponent<Player>().currentFloor = floor;
        floor.isPlayerOn = true;
        MapManager.inst.players.Add(player);
        if (GameManager.nPlayer >= 0)
        {
            MapManager.inst.currentMap.clearConditions[GameManager.nPlayer].count = MapManager.inst.players.Count;
            MapManager.inst.currentMap.clearConditions[GameManager.nPlayer].IsDone();
        }
        CheckCurrentFloors();
        return player;
    }
    public void CreatePlayer(Vector2Int floorPos, Vector2Int originPos, bool dir)
    {
        Floor originFloor = MapManager.inst.currentMap.GetFloorAtPos(originPos);
        Quaternion mirroredRotation = Quaternion.identity;
        foreach (var obj in MapManager.inst.players)
        {
            if (obj.GetComponent<Player>().currentFloor == originFloor)
            {
                mirroredRotation = obj.transform.rotation;
                break;
            }
        }
        mirroredRotation.y *= -1;
        if (dir) { mirroredRotation.x *= -1; mirroredRotation = Quaternion.Euler(mirroredRotation.eulerAngles + new Vector3(0, 180, 0)); }
        else mirroredRotation.z *= -1;

        if (MapManager.inst.currentMap.floorGrid.TryGetValue(floorPos, out Floor floor))
        {
            GameObject player = CreatePlayer(floor);
            player.transform.rotation = mirroredRotation;
        }
        else
            Debug.Log("there are no floor");
    }

    public void RemovePlayer(Floor floor)
    {
        if (!floor)
        {
            Debug.Log("there are no floor");
            return;
        }

        List<GameObject> copyPlayers = new List<GameObject>(MapManager.inst.players);
        foreach (var obj in copyPlayers)
        {
            if (obj.GetComponent<Player>().currentFloor == floor)
            {
                floor.isPlayerOn = false;
                MapManager.inst.players.Remove(obj);
                if (GameManager.nPlayer >= 0)
                {
                    MapManager.inst.currentMap.clearConditions[GameManager.nPlayer].count = MapManager.inst.players.Count;
                    MapManager.inst.currentMap.clearConditions[GameManager.nPlayer].IsDone();
                }
                Destroy(obj);
                CheckCurrentFloors();
                return;
            }
        }
    }
    public void RemovePlayer(Vector2Int floorPos)
    {
        if (MapManager.inst.currentMap.floorGrid.TryGetValue(floorPos, out Floor floor))
        {
            RemovePlayer(floor);
        }
        else
        {
            Debug.Log("there are no floor");
        }
    }

    public void CheckCurrentFloors()
    {
        int goalFloorCount = 0;
        foreach (GameObject child in MapManager.inst.players)
        {
            OnPlayerMove(Vector2Int.RoundToInt(new Vector2(child.transform.position.x, child.transform.position.z)));
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

    public void AddBullet(BulletCode newBullet)
    {
        bulletList.Add(newBullet);
        GameManager.inst.bulletUIGenerator.GenerateBulletUI(newBullet);
    }

    //For test
    public string GetCurrentBullet()
    {
        return bulletList.Count > 0 ? bulletList[0].ToString() : null;
    }

    // Start is called before the first frame update
    void Start()
    {
        prePos = MapPos;
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameManager.inst.isGameOver)
        {
            if (prePos != MapPos)
            {
                //Debug.Log(MapPos);
                OnPlayerMove?.Invoke(MapPos);
                prePos = MapPos;
            }

            //Control player only if camera is not zooming in to or out from the current player
            if (!GameManager.inst.isZooming && !GameManager.inst.isBulletFlying)
            {
                if (Input.GetMouseButtonDown(0) && zoomReady == null)
                {
                    //Move the current player.
                    if (!GameManager.inst.isPlayerMoving && !GameManager.inst.isPlayerShooting)
                    {
                        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                        int layerMask = (-1) - (1 << LayerMask.NameToLayer("Scattered"));
                        RaycastHit hit;
                        if (Physics.Raycast(mouseRay, out hit, float.MaxValue, layerMask) && hit.collider.gameObject.tag.Equals("Player"))
                        {
                            if (currentPlayer != null)
                                currentPlayer.ResetCurrentPlayer();
                            currentPlayer = hit.transform.gameObject.GetComponent<Player>();
                            StartCoroutine(currentPlayer.SetCurrentPlayer());
                            zoomReady = StartCoroutine(currentPlayer.CountPlayerClick(Time.time));
                            //Debug.Log(hit.collider.gameObject.tag);
                        }
                        else if (Physics.Raycast(mouseRay, out hit, float.MaxValue, layerMask) && hit.collider.gameObject.tag.Equals("floor"))
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
                    else if (GameManager.inst.isPlayerShooting && currentPlayer.laser.activeSelf)
                    {
                        if (bulletList.Count > 0 && currentPlayer.canShoot)
                        {
                            currentPlayer.Shoot(bulletList[0]);
                        }
                    }
                    else if (GameManager.inst.isPlayerMoving && !GameManager.inst.isFast)
                    {
                        currentPlayer.GetComponent<NavMeshAgent>().speed *= 5;
                        currentPlayer.anim.speed *= 5;
                        GameManager.inst.isFast = true;
                    }
                }
                else if (Input.GetMouseButtonDown(1) && GameManager.inst.isPlayerShooting)
                {
                    StartCoroutine(Camera.main.GetComponent<CameraController>().ZoomOutFromPlayer(currentPlayer));
                    currentPlayer.OffAllOutline();
                    currentPlayer.shootingArm.rotation = currentPlayer.armRotation;
                }
            }
        }
    }

    void LateUpdate()
    {
        if(currentPlayer != null)
        {
            if (currentPlayer.GetComponent<NavMeshAgent>().velocity.magnitude > 0)
                transform.rotation = Quaternion.LookRotation(currentPlayer.GetComponent<NavMeshAgent>().velocity.normalized);
            if (GameManager.inst.isPlayerShooting)
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
