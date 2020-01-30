﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPole : MonoBehaviour, IObject, IBulletInteractor
{
    public Vector2 position;
    public float radius = 0.5f;

    public Transform shootPoint;
    public  LineRenderer rayRenderer; 

    private bool isRayActive = false;
    private float rayHeight;
    private LightGetter receivedGetter;
    private bool isRotating = false;

    private void Awake()
    {
        shootPoint = transform.Find("RotateObject");
        rayRenderer = GetComponent<LineRenderer>();

        rayRenderer.SetPosition(0, shootPoint.position);
        rayRenderer.SetPosition(1, shootPoint.position);
        rayHeight = shootPoint.localPosition.y;
    }

    public void SetRayActive(bool isActive)
    {
        if (isRayActive != isActive)
        {
            isRayActive = isActive;
            if (isActive)
            { // turn on ray
                rayRenderer.enabled = true;
                ShootRay();
            }
            else
            { // turn off ray
                rayRenderer.enabled = false;
                receivedGetter?.SetReceived(false);
                receivedGetter = null;
            }
        }
    }

    public void ShootRay()
    {
        // transform.forward 잘쓰면될듯
        List<Vector3> points = new List<Vector3>();
        Vector3 lastPoint = shootPoint.position;
        Vector3 lastDirection = transform.forward;
        int maxSize = MapManager.inst.currentMap.maxMapSize;
        bool isHit;

        points.Add(lastPoint);
        do
        {
            isHit = Physics.Raycast(lastPoint, lastDirection, out RaycastHit hit, maxSize);
            if (isHit)
            {
                lastPoint = hit.transform.position;
                lastPoint.y = rayHeight;
                points.Add(lastPoint);

                if (hit.transform.GetComponent<Wall>() is Wall w)
                {
                    if (w.type == WallType.Mirror)
                    {
                        if (w.dir)
                        {
                            lastDirection.z *= -1;
                        }
                        else
                        {
                            lastDirection.x *= -1;
                        }
                    }
                    else if (w.type == WallType.Normal)
                    {
                        isHit = false; // end ray
                    }
                }
                else if (hit.transform.GetComponent<LightGetter>() is LightGetter lg)
                {
                    lg.SetReceived(true);
                    receivedGetter = lg;

                    isHit = false; // end ray
                }
            }
            else
            {
                points.Add(lastPoint + lastDirection * maxSize);
            }
        } while (isHit && maxSize > Mathf.Max(lastPoint.x, lastPoint.z));
        rayRenderer.positionCount = points.Count;
        rayRenderer.SetPositions(points.ToArray());
    }

    public IEnumerator RotatePole(bool isRightHandRotate)
    {
        GameManager.inst.isBulletFlying = true;
        isRotating = true;
        SetRayActive(false);

        float time = 0f;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, isRightHandRotate ? 90 : -90, 0) + transform.localRotation.eulerAngles);
        while (time < 1f)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, time);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localRotation = targetRotation;

        SetRayActive(true);
        isRotating = false;
        GameManager.inst.isBulletFlying = false;
    }

    private void Update()
    {
        if (GameManager.inst.isPlayerMoving)
        {
            SetRayActive(false);
        }
        else if (!isRotating)
        {
            SetRayActive(true);
        }
    }

    #region IObject

    public GameObject GetObject()
    {
        return gameObject;
    }

    public Vector2 GetPos()
    {
        return position;
    }

    ObjType IObject.GetType()
    {
        return ObjType.LightPole;
    }

    public float GetRadius()
    {
        return radius;
    }

    /// <param name="additonal">
    /// <br/>No additional data
    /// </param>
    public void Init(Vector2 pos, params object[] additonal)
    {
        position = pos;
        SetRayActive(true);
    }

    public object[] GetAdditionals()
    {
        return new object[0];
    }

    public int GetMirrorAble()
    {
        return 2;
    }
    #endregion

    #region IBulletInteractor
    public void Interact(Bullet bullet)
    {
        if (bullet is TruthBullet)
        {
            StartCoroutine(RotatePole(true));
        }
        else if (bullet is FakeBullet)
        {
            StartCoroutine(RotatePole(false));
        }
    }
    #endregion
}
