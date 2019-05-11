using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    Coroutine playerArrivalCheck;

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
    IEnumerator CheckIfPlayerArrived(Vector3 destination)
	{
		while (Mathf.Abs(transform.position.x - destination.x) > 0.01f || Mathf.Abs(transform.position.z - destination.z) > 0.01f)
		{
			yield return null;
		}
		transform.position = new Vector3(destination.x, transform.position.y, destination.z);
        PlayerController.inst.isPlayerMoving = false;
    }
    public IEnumerator ZoomInAtPlayer(float startTime)
    {
        Ray mouseRay;
        RaycastHit hit;
        while(Time.time - startTime <= 2)
        {
            yield return null;
            mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(mouseRay, out hit);
            if (!hit.collider.gameObject.tag.Equals("Player"))
                break;
        }
        if (Time.time - startTime <= 2) ;
        else Debug.Log("asdf");
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
