using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    public Vector2 pos
    {
        get { return new Vector2(currentFloor.mapPos.x, currentFloor.mapPos.y); }
    }

    Coroutine playerArrivalCheck;
    public GameObject head;
    public Transform shootingArm;
    public GameObject shootingFinger;
    public Quaternion armRotation;
    public Animator anim;

    public Floor currentFloor;

    /// <summary>
    /// Set this player as the current player.
    /// </summary>
    /// <returns></returns>
    public IEnumerator SetCurrentPlayer()
    {
        GetComponent<NavMeshObstacle>().enabled = false;
        yield return null;
        GetComponent<NavMeshAgent>().enabled = true;
        StartCoroutine(MapManager.inst.Rebaker());
    }
    /// <summary>
    /// Reset this player from the current player.
    /// </summary>
    /// <returns></returns>
    public void ResetCurrentPlayer()
    {
        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<NavMeshObstacle>().enabled = true;
        StartCoroutine(MapManager.inst.Rebaker());
        PlayerController.inst.currentPlayer = null;
    }
    /// <summary>
    /// Move player to the destination.
    /// </summary>
    /// <param name="destination">Destination of the player.</param>
    public void MovePlayer(Vector3 destination)
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        NavMeshPath path = new NavMeshPath();
        if(playerArrivalCheck != null)
            StopCoroutine(playerArrivalCheck);
		agent.CalculatePath(destination, path);
        if(path.status == NavMeshPathStatus.PathComplete)
        {
		    PlayerController.inst.isPlayerMoving = true;
            playerArrivalCheck = StartCoroutine(CheckIfPlayerArrived(destination));
            GetComponent<NavMeshAgent>().SetDestination(destination);
        }
        else
            Debug.Log("Destination is not reachable.");
    }
    /// <summary>
    /// Check if player is arrived at the destination.
    /// </summary>
    /// <param name="destination">Destination of the player.</param>
    /// <returns></returns>
    IEnumerator CheckIfPlayerArrived(Vector3 destination)
    {
        anim.SetBool("isWalking", true);
        while (Mathf.Abs(transform.position.x - destination.x) > 0.01f || Mathf.Abs(transform.position.z - destination.z) > 0.01f)
			yield return null;
        transform.position = new Vector3(destination.x, transform.position.y, destination.z);
        currentFloor = MapManager.inst.currentMap.GetFloorAtPos(new Vector2Int((int)destination.x, (int)destination.z));
        PlayerController.inst.CheckCurrentFloors();
        anim.SetBool("isWalking", false);
        PlayerController.inst.isPlayerMoving = false;
    }
    /// <summary>
    /// Count 2 second to make player in shooting mode.
    /// </summary>
    /// <param name="startTime">Start time of the timer.</param>
    /// <returns></returns>
    public IEnumerator CountPlayerClick(float startTime)
    {
        float time = Time.time;
        float endTime = startTime + 2;
        while (time <= endTime)
        {
            yield return null;
            time = Time.time;
            if (!Input.GetMouseButton(0))
                break;
        }
        if (time > endTime)
        {
            PlayerController.inst.isPlayerShooting = true;
            StartCoroutine(Camera.main.GetComponent<CameraController>().ZoomInAtPlayer(this));
        }
    }

    public void Shoot(BulletCode bulletCode)
    {
        Bullet newBullet = MapManager.inst.bulletFactory.MakeBullet(bulletCode);
        newBullet.transform.position = shootingFinger.transform.position;
        newBullet.transform.LookAt(shootingArm.transform.forward + newBullet.transform.position);
        newBullet.Init(shootingArm.transform.forward);
        PlayerController.inst.bulletList.RemoveAt(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IPlayerInteractor>() != null)
        {
            other.GetComponent<IPlayerInteractor>().Interact(currentFloor.mapPos);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        armRotation = shootingArm.rotation;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
