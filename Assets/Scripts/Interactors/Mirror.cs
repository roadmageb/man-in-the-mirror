using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mirror : MonoBehaviour, IBulletInteractor, IBreakable
{
    private Camera camera;
    private RenderTexture rt;

    public void Break()
    {
        Destroy(gameObject);
    }

    public void Interact(Bullet bullet)
    {
        if (bullet is FakeBullet)
        {
            //Break Mirror & Make reflected objects
        }
    }

    private void Start()
    {
        camera = GetComponent<Camera>();
        //TODO : Create RenderTexture and put it into Camera's targeTexture
    }

    private void Update()
    {
        //TODO :Calculate Camera's Position and Rotation
    }
}
