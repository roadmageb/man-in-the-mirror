using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorCamera : MonoBehaviour
{
    void CameraMove()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        transform.position += new Vector3(horizontalInput / 4, 0, verticalInput / 4);
    }
    void CameraZoom()
    {
        float mouseWheel = -Input.GetAxis("Mouse ScrollWheel");
        Camera.main.orthographicSize += mouseWheel * 2;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CameraMove();
        CameraZoom();
    }
}
