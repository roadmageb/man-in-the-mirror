using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// code that moves Reflection Probe to proper position
public class MirrorReflect : MonoBehaviour
{
    enum Direction { X, Z } // +X direction and +Z direction. Mirror on both side

    [SerializeField]
    private Direction direction = Direction.X;

    public Transform mirror;
    public Transform mainCam;

    private float offset;
    private Vector3 probePos;

    // Update is called once per frame
    void Update()
    {
        mainCam = Camera.main.transform; // find main camera

        if (direction == Direction.X)
        {
            offset = mirror.position.x - mainCam.position.x;

            probePos.x = mirror.position.x + offset;
            probePos.y = mainCam.position.y;
            probePos.z = mainCam.position.z;
        }
        else if (direction == Direction.Z)
        {
            offset = mirror.position.z - mainCam.position.z;

            probePos.x = mainCam.position.x;
            probePos.y = mainCam.position.y;
            probePos.z = mirror.position.z + offset;
        }

        transform.position = probePos;
    }
}
