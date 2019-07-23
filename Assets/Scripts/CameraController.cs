using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 dragOrigin;
    Vector3 moveOrigin;
    public float dragSpeed;
    Vector3 previousPos;
    Vector3 previousAngle;
    float shootingFov = 60f;
    float mapFov = 20f;
    float rotationX = 0;
    float rotationY = 0;
    float sensitivity = 30;

    [SerializeField]
    Vector3 centerPos = new Vector3(0, 0, 0);
    Vector3 distance = new Vector3(0, 0, 0);
    /// <summary>
    /// Move camera.
    /// </summary>
    void CameraMove()
    {
        if (Input.GetMouseButtonDown(2))
        {
            moveOrigin = Input.mousePosition;
            return;
        }
        if (!Input.GetMouseButton(2))
        {
            Vector3 tempVec = centerPos;
            centerPos = new Vector3(Mathf.Clamp(Mathf.Round(centerPos.x * 2) / 2, MapManager.inst.currentMap.minBorder.x, MapManager.inst.currentMap.maxBorder.x),
                0, Mathf.Clamp(Mathf.Round(centerPos.z * 2) / 2, MapManager.inst.currentMap.minBorder.y, MapManager.inst.currentMap.maxBorder.y));
            transform.Translate(centerPos - tempVec, Space.World);
            return;
        }
        float previousY = transform.position.y;
        Vector3 centerDiff = transform.position;
        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - moveOrigin);
        moveOrigin = Input.mousePosition;
        transform.Translate(new Vector3(pos.x * -8, pos.y * -8, 0), Space.Self);
        transform.position = new Vector3(transform.position.x, previousY, transform.position.z);
        centerPos += transform.position - centerDiff;
        centerPos.y = 0;
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
        GameManager.inst.isZooming = true;
        previousPos = transform.position;
        previousAngle = new Vector3(transform.eulerAngles.x > 180 ? transform.eulerAngles.x - 360 : transform.eulerAngles.x,
            transform.eulerAngles.y > 180 ? transform.eulerAngles.y - 360 : transform.eulerAngles.y,
            transform.eulerAngles.z > 180 ? transform.eulerAngles.z - 360 : transform.eulerAngles.z);
        int i;
        for (i = 0; i < 40; i++)
        {
            yield return null;
            if (Input.GetMouseButtonDown(0))
                break;
            transform.position += posDiff;
            transform.eulerAngles += new Vector3(angleDiff, 0, 0);
            Camera.main.fieldOfView += fovDiff;
        }
        if(i < 40)
        {
            transform.position += posDiff * (40 - i);
            transform.eulerAngles += new Vector3(angleDiff * (40 - i), 0, 0);
            Camera.main.fieldOfView += fovDiff * (40 - i);
        }
        player.transform.eulerAngles = new Vector3(player.transform.eulerAngles.x, transform.eulerAngles.y, player.transform.eulerAngles.z);
        transform.position = player.head.transform.position;
        rotationX = transform.eulerAngles.y;
        rotationY = transform.eulerAngles.x;
        GameManager.inst.isZooming = false;
        player.laser.SetActive(true);
        player.anim.SetBool("isShooting", true);
        player.head.transform.Find("Head 19").gameObject.layer = LayerMask.NameToLayer("Head");

        // Invisible mouse cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    /// <summary>
    /// Zoom out from player.
    /// </summary>
    /// <returns></returns>
    public IEnumerator ZoomOutFromPlayer(Player player)
    {
        float startTime = Time.time;
        Vector3 posDiff = (previousPos - transform.position) / 40;
        float fovDiff = (mapFov - shootingFov) / 40f;
        player.laser.SetActive(false);
        GameManager.inst.isZooming = true;
        player.anim.SetBool("isShooting", false);
        player.head.transform.Find("Head 19").gameObject.layer = LayerMask.NameToLayer("Player");
        player.head.SetActive(true);
        Vector3 tempAngle = new Vector3(transform.eulerAngles.x > 180 ? transform.eulerAngles.x - 360 : transform.eulerAngles.x,
            transform.eulerAngles.y > 180 ? transform.eulerAngles.y - 360 : transform.eulerAngles.y,
            transform.eulerAngles.z > 180 ? transform.eulerAngles.z - 360 : transform.eulerAngles.z);
        Vector3 angleDiff = (previousAngle - tempAngle) / 40;
        angleDiff = new Vector3(angleDiff.x > 180 ? 360 - angleDiff.x : angleDiff.x,
            angleDiff.y > 180 ? 360 - angleDiff.y : angleDiff.y,
            angleDiff.z > 180 ? 360 - angleDiff.z : angleDiff.z);
        int i;
        for (i = 0; i < 40; i++)
        {
            yield return null;
            if (Input.GetMouseButtonDown(0))
                break;
            transform.position += posDiff;
            transform.eulerAngles += angleDiff;
            Camera.main.fieldOfView += fovDiff;
        }
        if (i < 40)
        {
            transform.position += posDiff * (40 - i);
            transform.eulerAngles += angleDiff * (40 - i);
            Camera.main.fieldOfView += fovDiff * (40 - i);
        }
        transform.position = previousPos;
        GameManager.inst.isPlayerShooting = false;
        GameManager.inst.isZooming = false;

        // Visible mouse cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Start is called before the first frame update
    void Start()
    {
        Camera.main.fieldOfView = mapFov;
        transform.eulerAngles = new Vector3(30, transform.eulerAngles.y, transform.eulerAngles.z);
        distance = transform.position - centerPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.inst.isZooming)
        {
            if (!GameManager.inst.isPlayerShooting)
            {
                CameraMove();
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
