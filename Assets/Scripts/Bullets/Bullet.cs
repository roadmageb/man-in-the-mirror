using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    private Rigidbody rb;
    protected abstract void OnTriggerEnter(Collider other);

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnDestroy()
    {
        GameManager.inst.isBulletFlying = false;
    }

    public void Init(Vector3 velocity)
    {
        GameManager.inst.isBulletFlying = true;
        GetComponent<Rigidbody>().velocity = velocity;
        Destroy(gameObject, MapManager.inst.currentMap.maxMapSize / velocity.magnitude);
    }    
}
