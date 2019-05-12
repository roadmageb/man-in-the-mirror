using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 dragOrigin;
    public float dragSpeed;
    Vector3 previousPos;
    bool isZooming = false;
    /// <summary>
    /// Move camera.
    /// </summary>
    void CameraMove()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        transform.position += new Vector3(verticalInput + horizontalInput, 0, verticalInput - horizontalInput);
    }

    /// <summary>
    /// Rotate camera with mouse right click.
    /// </summary>
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
        transform.LookAt(new Vector3(0, 1, 0));
        dragOrigin = Input.mousePosition;
    }
    /// <summary>
    /// Zoom in at player.
    /// </summary>
    /// <param name="player">Player to be zoomed in.</param>
    /// <returns></returns>
    public IEnumerator ZoomInAtPlayer(Player player)
    {
        float startTime = Time.time;
        Vector3 posDiff = (player.head.transform.position - transform.position) / 50;
        Vector3 angleDiff = (new Vector3(0, transform.eulerAngles.y, 0) - transform.eulerAngles) / 50;
        previousPos = transform.position;
        Camera.main.orthographic = false;
        for (int i = 0; i < 50; i++)
        {
            yield return null;
            transform.position += posDiff;
            transform.eulerAngles += angleDiff;
        }
        transform.position = player.head.transform.position;
    }
    /// <summary>
    /// Zoom out from player.
    /// </summary>
    /// <returns></returns>
    public IEnumerator ZoomOutFromPlayer()
    {
        float startTime = Time.time;
        Vector3 posDiff = (previousPos - transform.position) / 50;
        Vector3 angleDiff = (new Vector3(30, transform.eulerAngles.y, transform.eulerAngles.z) - transform.eulerAngles) / 50;
        for (int i = 0; i < 50; i++)
        {
            yield return null;
            transform.position += posDiff;
            transform.eulerAngles += angleDiff;
        }
        transform.position = previousPos;
        Camera.main.orthographic = true;
        PlayerController.inst.isPlayerShooting = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }



    // Update is called once per frame
    void Update()
    {
        if (!PlayerController.inst.isPlayerShooting)
        {
            CameraMove();
            CameraDrag();
        }
    }

}
