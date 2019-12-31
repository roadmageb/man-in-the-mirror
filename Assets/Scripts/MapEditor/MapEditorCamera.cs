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
        if(mouseWheel > 0 && Camera.main.orthographicSize < 10 || mouseWheel < 0 && Camera.main.orthographicSize > 2) 
        Camera.main.orthographicSize += mouseWheel * 2;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 2, 10);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!MapEditor.inst.isPanelOn)
        {
            CameraMove();
            CameraZoom();
        }
    }
}
