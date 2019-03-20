using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mirror : MonoBehaviour, IInteractor
{
    private Camera camera;
    private RenderTexture rt;

    public void Interact(Bullet bullet)
    {
        if (bullet is TruthBullet)
        {
            //Break Mirror
        }
        else if (bullet is FakeBullet)
        {
            //Break Mirror & Make reflected objects
        }
        else if (bullet is MirrorBullet)
        {
            //Nothing happens
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
