using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorMaterial : MonoBehaviour
{
    [SerializeField] private Material mat;
    [SerializeField] public static int count = 0;
    public int number;
    public Renderer mirrorReflect;
    public GameObject mirrorCam;
    public GameObject mirrorRef;

    // Start is called before the first frame update
    void Awake()
    {
        number = count++;
        mat = new Material(Shader.Find("Custom/Mirror"));
        mirrorReflect.materials[0] = mat;
    }

    private void Update()
    {
        if (Vector3.Dot(transform.forward, transform.position - Camera.main.transform.position) <= 0)
        {
            mirrorCam.SetActive(false);
            mirrorRef.SetActive(false);
        }
        else
        {
            mirrorCam.SetActive(true);
            mirrorRef.SetActive(true);
        }
    }
}
