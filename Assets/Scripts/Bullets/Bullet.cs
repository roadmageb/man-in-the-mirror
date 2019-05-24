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

    public void Init(Vector3 velocity)
    {
        GetComponent<Rigidbody>().velocity = velocity;
    }
}
