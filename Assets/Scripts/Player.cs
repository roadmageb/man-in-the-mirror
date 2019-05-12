using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    Coroutine playerArrivalCheck;
    public GameObject head;

    public IEnumerator SetCurrentPlayer()
    {
        GetComponent<NavMeshObstacle>().enabled = false;
        yield return null;
        GetComponent<NavMeshAgent>().enabled = true;
    }
    public void ResetCurrentPlayer()
    {
        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<NavMeshObstacle>().enabled = true;
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
		PlayerController.inst.isPlayerMoving = true;
		playerArrivalCheck = StartCoroutine(CheckIfPlayerArrived(destination));
		agent.CalculatePath(destination, path);
        if(path.status == NavMeshPathStatus.PathComplete)
            GetComponent<NavMeshAgent>().SetDestination(destination);
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
		while (Mathf.Abs(transform.position.x - destination.x) > 0.01f || Mathf.Abs(transform.position.z - destination.z) > 0.01f)
		{
			yield return null;
		}
		transform.position = new Vector3(destination.x, transform.position.y, destination.z);
        PlayerController.inst.isPlayerMoving = false;
    }
    public IEnumerator CountPlayerClick(float startTime)
    {
        float time = Time.time;
        float endTime = startTime + 2;
        while(time <= endTime)
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
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
