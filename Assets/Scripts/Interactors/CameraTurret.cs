using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTurret : MonoBehaviour, IBreakable
{
    public void Break()
    {
        Destroy(gameObject);
    }
}
