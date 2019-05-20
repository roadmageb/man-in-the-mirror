using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorMaterial : MonoBehaviour
{
    [SerializeField] private Material mat;
    [SerializeField] public static int count = 0;
    public int number;
    public Renderer mirrorReflect;

    // Start is called before the first frame update
    void Start()
    {
        number = count++;
        mat = new Material(Shader.Find("Custom/Mirror"));
        mirrorReflect.materials[0] = mat;
    }
}
