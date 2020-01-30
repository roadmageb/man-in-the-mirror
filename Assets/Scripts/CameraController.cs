using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject helpUI;
    public GameObject helpUI2;

    Vector3 dragOrigin;
    Vector3 moveOrigin;
    public float dragSpeed;
    float cameraMoveDuration = 50;
    Vector3 previousPos;
    Quaternion previousRotation;
    float shootingFov = 60f;
    float mapFov = 0;
    float rotationX = 0, rotationY = 0;
    float sensitivity = 5;
    public float minFOV, maxFOV;
    public float minAngleX, maxAngleX;

    public Vector3 centerPos = new Vector3(-0.5f, 0, -0.5f);
    [SerializeField] private float distance = 0;
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

        float difX = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin).x * dragSpeed;
        float difY = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin).y * dragSpeed;

        transform.RotateAround(centerPos, Vector3.up, difX);
        if (transform.eulerAngles.x - difY >= maxAngleX)
        {
            transform.RotateAround(centerPos, transform.right, -(maxAngleX - transform.eulerAngles.x));
        }
        else if(transform.eulerAngles.x - difY <= minAngleX)
        {
            transform.RotateAround(centerPos, transform.right, -(transform.eulerAngles.x - minAngleX));
        }
        else
        {
            transform.RotateAround(centerPos, transform.right, -difY);
        }

        transform.LookAt(centerPos);
        dragOrigin = Input.mousePosition;
    }
    /// <summary>
    /// Zoom in / out camera with mouse scroll.
    /// </summary>
    void CameraScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        if (Camera.main.fieldOfView >= maxFOV && scroll < 0) Camera.main.fieldOfView = maxFOV;
        else if (Camera.main.fieldOfView <= minFOV && scroll > 0) Camera.main.fieldOfView = minFOV;
        else Camera.main.fieldOfView -= scroll;
    }
    /// <summary>
    /// Zoom in at player.
    /// </summary>
    /// <param name="player">Player to be zoomed in.</param>
    /// <returns></returns>
    public IEnumerator ZoomInAtPlayer(Player player)
    {
        GameManager.inst.isZooming = true;
        helpUI2.SetActive(false);
        previousPos = transform.position;
        previousRotation = transform.rotation;
        for (int i = 0; i < cameraMoveDuration; i += 1)
        {
            yield return new WaitForSeconds(0.01f);
            if (!StageInfo.inst.isMapEditor && !StageSelector.inst.gameSettings["zoomAnim"]) break;
            transform.position = Vector3.Lerp(previousPos, player.head.transform.position, i / cameraMoveDuration);
            transform.rotation = Quaternion.Lerp(previousRotation, player.transform.rotation, i / cameraMoveDuration);
            Camera.main.fieldOfView = Mathf.Lerp(mapFov, shootingFov, i / cameraMoveDuration);
        }
        /*transform.position += posDiff * (cameraMoveDuration - i);
        transform.eulerAngles += new Vector3(angleDiff * (cameraMoveDuration - i), 0, 0);*/
        Camera.main.fieldOfView = shootingFov;
        transform.position = player.head.transform.position;
        transform.rotation = player.transform.rotation;

        rotationX = transform.eulerAngles.y;
        rotationY = transform.eulerAngles.x;
        GameManager.inst.isZooming = false;
        player.laser.SetActive(true);
        player.anim.SetBool("isShooting", true);
        player.head.transform.Find("Head 19").gameObject.layer = LayerMask.NameToLayer("Head");

        // Invisible mouse cursor
        helpUI.SetActive(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    /// <summary>
    /// Zoom out from player.
    /// </summary>
    /// <returns></returns>
    public IEnumerator ZoomOutFromPlayer(Player player)
    {
        GameManager.inst.isZooming = true;
        player.laser.SetActive(false);
        helpUI.SetActive(false);
        player.anim.SetBool("isShooting", false);
        player.head.transform.Find("Head 19").gameObject.layer = LayerMask.NameToLayer("Player");
        player.head.SetActive(true);

        Vector3 beforeZoomOutPos = transform.position;
        Quaternion beforeZoomOutRotation = transform.rotation;
        previousRotation = Quaternion.Euler(previousRotation.eulerAngles.x, player.transform.rotation.eulerAngles.y, player.transform.rotation.eulerAngles.z);
        previousPos = centerPos - Mathf.Cos(Mathf.Deg2Rad * previousRotation.eulerAngles.x) * distance * player.transform.forward + new Vector3(0, previousPos.y - centerPos.y, 0);

        for (int i = 0; i < cameraMoveDuration; i += 1)
        {
            yield return new WaitForSeconds(0.01f);
            if (!StageInfo.inst.isMapEditor && !StageSelector.inst.gameSettings["zoomAnim"]) break;
            transform.position = Vector3.Lerp(beforeZoomOutPos, previousPos, i / cameraMoveDuration);
            transform.rotation = Quaternion.Lerp(beforeZoomOutRotation, previousRotation, i / cameraMoveDuration);
            Camera.main.fieldOfView = Mathf.Lerp(shootingFov, mapFov, i / cameraMoveDuration);
        }
        /*transform.position += posDiff * (cameraMoveDuration - i);
        transform.eulerAngles += angleDiff * (cameraMoveDuration - i);*/
        Camera.main.fieldOfView = mapFov;
        transform.position = previousPos;
        transform.rotation = previousRotation;
        transform.LookAt(centerPos);

        //transform.LookAt(centerPos);
        GameManager.inst.isPlayerShooting = false;
        GameManager.inst.isZooming = false;

        // Visible mouse cursor
        helpUI2.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Start is called before the first frame update
    void Start()
    {
        Camera.main.fieldOfView = mapFov;
        transform.eulerAngles = new Vector3(30, transform.eulerAngles.y, transform.eulerAngles.z);
        distance = Vector3.Distance(transform.position, centerPos);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.inst.isZooming)
        {
            if (!GameManager.inst.isPlayerShooting)
            {
                mapFov = Camera.main.fieldOfView;
                CameraMove();
                CameraDrag();
                CameraScroll();
            }
            else if (!GameManager.inst.isGameOver)
            {
                float mouseMoveValueX = Input.GetAxis("Mouse X");
                float mouseMoveValueY = Input.GetAxis("Mouse Y");
                rotationX += mouseMoveValueX * sensitivity;
                rotationY += mouseMoveValueY * sensitivity;
                rotationY = Mathf.Clamp(rotationY, -20, 10);
                transform.eulerAngles = new Vector3(-rotationY, rotationX, 0);
            }
        }
    }

}
