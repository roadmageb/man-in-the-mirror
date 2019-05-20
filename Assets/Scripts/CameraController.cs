using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 dragOrigin;
    public float dragSpeed;
    Vector3 previousPos;
    Vector3 previousAngle;
    float shootingFov = 60f;
    float mapFov = 40f;

    Vector3 centerPos = new Vector3(0, 0, 0);
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

        float deg = Mathf.Atan2(transform.position.z - centerPos.z, transform.position.x - centerPos.x);
        float dis = Vector3.Distance(centerPos, transform.position - new Vector3(0, transform.position.y - centerPos.y, 0));

        float dif = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin).x * dragSpeed;

        transform.position = new Vector3(Mathf.Cos(deg - dif) * dis + centerPos.x, transform.position.y, Mathf.Sin(deg - dif) * dis + centerPos.z);
        transform.LookAt(centerPos);
        dragOrigin = Input.mousePosition;
        transform.eulerAngles = new Vector3(30, transform.eulerAngles.y, transform.eulerAngles.z);
    }
    /// <summary>
    /// Zoom in at player.
    /// </summary>
    /// <param name="player">Player to be zoomed in.</param>
    /// <returns></returns>
    public IEnumerator ZoomInAtPlayer(Player player)
    {
        float startTime = Time.time;
        Vector3 posDiff = (player.head.transform.position - transform.position) / 40;
        Vector3 angleDiff = (player.head.transform.eulerAngles - transform.eulerAngles) / 40;
        float fovDiff = (shootingFov - mapFov) / 40f;
        previousPos = transform.position;
        previousAngle = transform.eulerAngles;
        for (int i = 0; i < 40; i++)
        {
            yield return null;
            transform.position += posDiff;
            transform.eulerAngles += angleDiff;
            Camera.main.fieldOfView += fovDiff;
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
        Vector3 posDiff = (previousPos - transform.position) / 40;
        Vector3 angleDiff = (previousAngle - transform.eulerAngles) / 40;
        float fovDiff = (mapFov - shootingFov) / 40f;
        for (int i = 0; i < 40; i++)
        {
            yield return null;
            transform.position += posDiff;
            transform.eulerAngles += angleDiff;
            Camera.main.fieldOfView += fovDiff;
        }
        transform.position = previousPos;
        PlayerController.inst.isPlayerShooting = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        Camera.main.fieldOfView = mapFov;
        transform.eulerAngles = new Vector3(30, transform.eulerAngles.y, transform.eulerAngles.z);
    }



    // Update is called once per frame
    void Update()
    {
        if (!PlayerController.inst.isPlayerShooting)
        {
            //CameraMove();
            CameraDrag();
        }
    }

}
