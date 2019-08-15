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

    IEnumerator ForceInteract(Collider col, float _time)
    {
        if (col.CompareTag("Mirror") && this is FakeBullet)
        {
            col.GetComponent<Mirror>().StartCopy();
            yield return new WaitForSeconds(_time);
            col.GetComponent<Mirror>().doReflect = true;
            OnTriggerEnter(col);
        }
        else
        {
            yield return new WaitForSeconds(_time);
            OnTriggerEnter(col);
        }
        Destroy(gameObject, 0.1f);
    }

    public void Init(Vector3 velocity, Collider col)
    {
        GameManager.inst.isBulletFlying = true;
        GetComponent<Rigidbody>().velocity = velocity;
        float flightTime;
        if (col != null) flightTime = (col.transform.position - transform.position).magnitude / velocity.magnitude;
        else flightTime = MapManager.inst.currentMap.maxMapSize / velocity.magnitude;
        StartCoroutine(ForceInteract(col, flightTime));
    }    
}
