using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 dragOrigin;
    public float dragSpeed;

    void CameraMove()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        transform.position += new Vector3(verticalInput + horizontalInput, 0, verticalInput - horizontalInput);
    }

    void CameraDrag()
    {

        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(1)) return;

        float deg = Mathf.Atan2(transform.position.z, transform.position.x);
        float dis = Vector3.Distance(Vector3.zero, transform.position - new Vector3(0, transform.position.y, 0));

        float dif = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin).x * dragSpeed;

        transform.position = new Vector3(Mathf.Cos(deg + dif) * dis, transform.position.y, Mathf.Sin(deg + dif) * dis);
        transform.LookAt(new Vector3(0, 0, 0));
        dragOrigin = Input.mousePosition;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }



    // Update is called once per frame
    void Update()
    {
        CameraMove();
        CameraDrag();
    }

}
