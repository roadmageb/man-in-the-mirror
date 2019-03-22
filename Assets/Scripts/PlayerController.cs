using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(mouseRay, out hit) && hit.collider.gameObject.tag.Equals("floor"))
            {
                GetComponent<NavMeshAgent>().SetDestination(hit.collider.gameObject.transform.position);
                Debug.Log(hit.collider.gameObject.transform.position);
                Debug.Log(hit.collider.gameObject.tag);
            }
        }
    }
}
