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
    float mapFov = 20f;
    float rotationX = 0;
    float rotationY = 0;
    float sensitivity = 30;

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
        float fovDiff = (shootingFov - mapFov) / 40f;
        float angleDiff = -30f / 40f;
        PlayerController.inst.isZooming = true;
        previousPos = transform.position;
        previousAngle = new Vector3(transform.eulerAngles.x > 180 ? transform.eulerAngles.x - 360 : transform.eulerAngles.x,
            transform.eulerAngles.y > 180 ? transform.eulerAngles.y - 360 : transform.eulerAngles.y,
            transform.eulerAngles.z > 180 ? transform.eulerAngles.z - 360 : transform.eulerAngles.z);
        for (int i = 0; i < 40; i++)
        {
            yield return null;
            transform.position += posDiff;
            transform.eulerAngles += new Vector3(angleDiff, 0, 0);
            Camera.main.fieldOfView += fovDiff;
        }
        player.transform.eulerAngles = new Vector3(player.transform.eulerAngles.x, transform.eulerAngles.y, player.transform.eulerAngles.z);
        transform.position = player.head.transform.position;
        rotationX = transform.eulerAngles.y;
        rotationY = transform.eulerAngles.x;
        PlayerController.inst.isZooming = false;
        PlayerController.inst.currentPlayer.GetComponent<Animator>().SetBool("isShooting", true);
        PlayerController.inst.currentPlayer.GetComponent<Player>().head.SetActive(false);

    }
    /// <summary>
    /// Zoom out from player.
    /// </summary>
    /// <returns></returns>
    public IEnumerator ZoomOutFromPlayer()
    {
        float startTime = Time.time;
        Vector3 posDiff = (previousPos - transform.position) / 40;
        float fovDiff = (mapFov - shootingFov) / 40f;
        PlayerController.inst.isZooming = true;
        PlayerController.inst.currentPlayer.GetComponent<Animator>().SetBool("isShooting", false);
        PlayerController.inst.currentPlayer.GetComponent<Player>().head.SetActive(true);
        Vector3 tempAngle = new Vector3(transform.eulerAngles.x > 180 ? transform.eulerAngles.x - 360 : transform.eulerAngles.x,
            transform.eulerAngles.y > 180 ? transform.eulerAngles.y - 360 : transform.eulerAngles.y,
            transform.eulerAngles.z > 180 ? transform.eulerAngles.z - 360 : transform.eulerAngles.z);
        Vector3 angleDiff = (previousAngle - tempAngle) / 40;
        angleDiff = new Vector3(angleDiff.x > 180 ? 360 - angleDiff.x : angleDiff.x,
            angleDiff.y > 180 ? 360 - angleDiff.y : angleDiff.y,
            angleDiff.z > 180 ? 360 - angleDiff.z : angleDiff.z);
        for (int i = 0; i < 40; i++)
        {
            yield return null;
            transform.position += posDiff;
            transform.eulerAngles += angleDiff;
            Camera.main.fieldOfView += fovDiff;
        }
        transform.position = previousPos;
        PlayerController.inst.isPlayerShooting = false;
        PlayerController.inst.isZooming = false;
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
        if (!PlayerController.inst.isZooming)
        {
            if (!PlayerController.inst.isPlayerShooting)
            {
                //CameraMove();
                CameraDrag();
            }
            else
            {
                float mouseMoveValueX = Input.GetAxis("Mouse X");
                float mouseMoveValueY = Input.GetAxis("Mouse Y");
                rotationX += mouseMoveValueX * sensitivity * Time.deltaTime;
                rotationY += mouseMoveValueY * sensitivity * Time.deltaTime;
                rotationY = Mathf.Clamp(rotationY, -20, 10);
                transform.eulerAngles = new Vector3(-rotationY, rotationX, 0);
            }
        }
    }

}
