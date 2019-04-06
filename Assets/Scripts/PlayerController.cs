using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : SingletonBehaviour<PlayerController>
{
    private Vector2Int prePos;
    public Vector2Int MapPos
    {
        get
        {
            Vector2Int pos = Vector2Int.zero;
            pos.x = Mathf.RoundToInt(transform.position.x);
            pos.y = Mathf.RoundToInt(transform.position.y);
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

        if (Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(mouseRay, out hit) && hit.collider.gameObject.tag.Equals("floor"))
            {
                GetComponent<NavMeshAgent>().SetDestination(hit.collider.gameObject.transform.position);
                //Debug.Log(hit.collider.gameObject.GetComponent<Floor>().mapPos);
                //Debug.Log(hit.collider.gameObject.tag);
            }
        }
    }
}
