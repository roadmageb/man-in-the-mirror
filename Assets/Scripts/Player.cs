using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using cakeslice;

public class Player : MonoBehaviour
{
    public Vector2 pos
    {
        get { return new Vector2(currentFloor.mapPos.x, currentFloor.mapPos.y); }
    }

    Coroutine playerArrivalCheck;
    public GameObject head;
    public Transform shootingArm;
    public GameObject shootingFinger;
    public GameObject laser;
    public Quaternion armRotation;
    public Animator anim;

    public Floor currentFloor;

    public GameObject selectPointer;
    public VLight aimLight;

    public bool canShoot = false;
    public Collider lastCol = null;
    private Collider beforeRay = null;
    private GameObject currentBullet;
    private float lastShoot;

    /// <summary>
    /// Set this player as the current player.
    /// </summary>
    /// <returns></returns>
    public IEnumerator SetCurrentPlayer()
    {
        GetComponent<NavMeshObstacle>().enabled = false;
        selectPointer.SetActive(true);
        MapManager.inst.surface.BuildNavMesh();
        yield return null;
        GetComponent<NavMeshAgent>().enabled = true;
        StartCoroutine(MapManager.inst.Rebaker());
    }
    /// <summary>
    /// Reset this player from the current player.
    /// </summary>
    /// <returns></returns>
    public void ResetCurrentPlayer()
    {
        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<NavMeshObstacle>().enabled = true;
        selectPointer.SetActive(false);
        OffAllOutline();
        StartCoroutine(MapManager.inst.Rebaker());
        PlayerController.inst.currentPlayer = null;
    }
    /// <summary>
    /// Move player to the destination.
    /// </summary>
    /// <param name="destination">Destination of the player.</param>
    public void MovePlayer(Vector3 destination)
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        NavMeshPath path = new NavMeshPath();
        if(playerArrivalCheck != null)
            StopCoroutine(playerArrivalCheck);
		agent.CalculatePath(destination, path);
        if(path.status == NavMeshPathStatus.PathComplete && !MapManager.inst.currentMap.GetFloorAtPos((int)destination.x, (int)destination.z).isPlayerOn)
        {
            GameManager.inst.isPlayerMoving = true;
            playerArrivalCheck = StartCoroutine(CheckIfPlayerArrived(destination));
            GetComponent<NavMeshAgent>().SetDestination(destination);
        }
        else
            Debug.Log("Destination is not reachable.");
    }
    /// <summary>
    /// Check if player is arrived at the destination.
    /// </summary>
    /// <param name="destination">Destination of the player.</param>
    /// <returns></returns>
    IEnumerator CheckIfPlayerArrived(Vector3 destination)
    {
        anim.SetBool("isWalking", true);
        while (Mathf.Abs(transform.position.x - destination.x) > 0.01f || Mathf.Abs(transform.position.z - destination.z) > 0.01f)
        {
            yield return null;
            Floor positionFloor = MapManager.inst.currentMap.GetFloorAtPos(
                new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z)));
            if (positionFloor != currentFloor)
            {
                currentFloor.isPlayerOn = false;
                currentFloor = positionFloor;
                currentFloor.isPlayerOn = true;
            }
        }
        currentFloor.isPlayerOn = false;
        currentFloor = MapManager.inst.currentMap.GetFloorAtPos(new Vector2Int((int)destination.x, (int)destination.z));
        currentFloor.isPlayerOn = true;

        transform.position = new Vector3(destination.x, transform.position.y, destination.z);
        PlayerController.inst.CheckCurrentFloors();
        anim.SetBool("isWalking", false);
        anim.speed = 1;
        GetComponent<NavMeshAgent>().speed = 1.5f;
        GameManager.inst.isFast = false;
        GameManager.inst.isPlayerMoving = false;
    }
    /// <summary>
    /// Count 2 second to make player in shooting mode.
    /// </summary>
    /// <param name="startTime">Start time of the timer.</param>
    /// <returns></returns>
    public IEnumerator CountPlayerClick(float startTime, bool direct = false)
    {
        float time = Time.time;
        float doubleClickDelay = 0.2f;
        float endTime = startTime + 1f;
        bool doubleClicked = direct;
        bool isHoldExit = false;
        aimLight.gameObject.SetActive(true);
        while (time <= endTime && !direct)
        {
            yield return null;
            aimLight.lightMultiplier += 10 * Time.deltaTime;
            aimLight.spotAngle -= 60 * Time.deltaTime;
            time = Time.time;
            if (!Input.GetMouseButton(0))
            {
                isHoldExit = true;
                break;
            }
        }
        if (isHoldExit)
        {
            while (time + doubleClickDelay > Time.time)
            {
                yield return null;
                aimLight.lightMultiplier *= 0.8f;
                if (Input.GetMouseButtonDown(0))
                {
                    doubleClicked = true;
                    break;
                }
            }
            aimLight.lightMultiplier = 0;
            aimLight.spotAngle = 60;
            aimLight.gameObject.SetActive(false);
        }
        if ((!isHoldExit && time > endTime) || doubleClicked)
        {
            aimLight.lightMultiplier = 0;
            aimLight.spotAngle = 60;
            aimLight.gameObject.SetActive(false);
            GameManager.inst.isPlayerShooting = true;
            StartCoroutine(Camera.main.GetComponent<CameraController>().ZoomInAtPlayer(this));
        }
        PlayerController.inst.zoomReady = null;
    }

    public void Shoot(BulletCode bulletCode)
    {
        OffAllOutline();
        Bullet newBullet = MapManager.inst.bulletFactory.MakeBullet(bulletCode);
        newBullet.transform.position = shootingFinger.transform.position;
        newBullet.transform.LookAt(shootingArm.transform.forward + newBullet.transform.position);
        newBullet.Init(shootingArm.transform.forward * 3, lastCol);
        currentBullet = newBullet.gameObject;
        PlayerController.inst.bulletList.RemoveAt(0);
        GameManager.inst.bulletUIGenerator.RemoveBulletUI();
        laser.SetActive(false);
        lastShoot = Time.time;
        anim.SetTrigger("shoot");
    }
    public void OffAllOutline()
    {
        canShoot = false;
        if (beforeRay != null) lastCol = beforeRay;
        laser.GetComponent<LineRenderer>().startColor = Color.red;
        laser.GetComponent<LineRenderer>().endColor = Color.red;
        if (beforeRay != null)
        {
            if (beforeRay.tag.Equals("wall") || beforeRay.tag.Equals("Mirror") || beforeRay.tag.Equals("Glass"))
            {
                beforeRay.GetComponent<Outline>().enabled = false;
            }
            else if (beforeRay.tag.Equals("CameraTurret") || beforeRay.tag.Equals("Mannequin"))
            {
                foreach (var comp in beforeRay.GetComponentsInChildren<Outline>())
                {
                    comp.enabled = false;
                }
            }
            beforeRay = null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        armRotation = shootingArm.rotation;
        currentFloor.RefreshGoal(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.inst.isGameOver && PlayerController.inst.currentPlayer == this && GameManager.inst.isPlayerShooting && !GameManager.inst.isZooming)
        {
            laser.transform.position = shootingFinger.transform.position;
            if (currentBullet == null && lastShoot + 1f < Time.time)
            {
                laser.SetActive(true);

                Ray ray = new Ray(shootingFinger.transform.position, shootingArm.transform.forward);
                int layerMask = (-1) - (1 << LayerMask.NameToLayer("Scattered"));
                RaycastHit hit;
                bool isHit = Physics.Raycast(ray, out hit, float.MaxValue, layerMask);
                if (isHit && PlayerController.inst.bulletList.Count > 0 && beforeRay != hit.collider)
                {
                    OffAllOutline();
                    beforeRay = hit.collider;
                    if (PlayerController.inst.bulletList[0] == BulletCode.True)
                    {
                        if (beforeRay.tag.Equals("Mirror"))
                        {
                            beforeRay.GetComponent<Outline>().enabled = true;
                            canShoot = true;
                            laser.GetComponent<LineRenderer>().startColor = Color.green;
                            laser.GetComponent<LineRenderer>().endColor = Color.green;
                        }
                        else if (beforeRay.tag.Equals("Glass"))
                        {
                            beforeRay.GetComponent<Outline>().enabled = true;
                            canShoot = true;
                            laser.GetComponent<LineRenderer>().startColor = Color.green;
                            laser.GetComponent<LineRenderer>().endColor = Color.green;
                        }
                        else if (beforeRay.tag.Equals("CameraTurret"))
                        {
                            foreach (var comp in beforeRay.GetComponentsInChildren<Outline>())
                            {
                                comp.enabled = true;
                            }
                            canShoot = true;
                            laser.GetComponent<LineRenderer>().startColor = Color.green;
                            laser.GetComponent<LineRenderer>().endColor = Color.green;
                        }
                        else if (beforeRay.tag.Equals("Mannequin") && beforeRay.GetComponent<Mannequin>().isWhite == false)
                        {
                            foreach (var comp in beforeRay.GetComponentsInChildren<Outline>())
                            {
                                comp.enabled = true;
                            }
                            canShoot = true;
                            laser.GetComponent<LineRenderer>().startColor = Color.green;
                            laser.GetComponent<LineRenderer>().endColor = Color.green;
                        }
                    }
                    else if (PlayerController.inst.bulletList[0] == BulletCode.False)
                    {
                        if (beforeRay.tag.Equals("Mirror"))
                        {
                            beforeRay.GetComponent<Outline>().enabled = true;
                            canShoot = true;
                            laser.GetComponent<LineRenderer>().startColor = Color.green;
                            laser.GetComponent<LineRenderer>().endColor = Color.green;
                        }
                        else if (beforeRay.tag.Equals("Glass"))
                        {
                            beforeRay.GetComponent<Outline>().enabled = true;
                            canShoot = true;
                            laser.GetComponent<LineRenderer>().startColor = Color.green;
                            laser.GetComponent<LineRenderer>().endColor = Color.green;
                        }
                        else if (beforeRay.tag.Equals("Mannequin") && beforeRay.GetComponent<Mannequin>().isWhite == true)
                        {
                            foreach (var comp in beforeRay.GetComponentsInChildren<Outline>())
                            {
                                comp.enabled = true;
                            }
                            canShoot = true;
                            laser.GetComponent<LineRenderer>().startColor = Color.green;
                            laser.GetComponent<LineRenderer>().endColor = Color.green;
                        }
                    }
                    else if (PlayerController.inst.bulletList[0] == BulletCode.Mirror)
                    {
                        if (beforeRay.tag.Equals("wall"))
                        {
                            beforeRay.GetComponent<Outline>().enabled = true;
                            canShoot = true;
                            laser.GetComponent<LineRenderer>().startColor = Color.green;
                            laser.GetComponent<LineRenderer>().endColor = Color.green;
                        }
                    }
                }
                else if (!isHit)
                {
                    OffAllOutline();
                    beforeRay = null;
                }
            }
        }
        else if (laser.activeSelf)
        {
            OffAllOutline();
            laser.SetActive(false);
        }
    }
}
