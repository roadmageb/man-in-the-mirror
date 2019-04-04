using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
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

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        Floor floor = collision.transform.GetComponent<Floor>();
        if (floor != null)
        {
            Debug.Log(floor.mapPos);
        }
    }
}
