using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempMapEditor : MonoBehaviour
{
    public GameObject test;
    Vector3 GetMousePoint(bool isFloat = false, bool isAtPoint = false)
    {
        Vector3 originPos = Camera.main.ScreenPointToRay(Input.mousePosition).origin;
        Vector3 mousePoint = new Vector3(Mathf.Round(originPos.x - (isFloat ? 0.5f : 0)) + (isFloat ? 0.5f : 0), 0, 
            Mathf.Round(originPos.z - (isFloat ? 0.5f : 0)) + (isFloat ? 0.5f : 0));
        if (!isAtPoint)
        {
            if(Mathf.Abs(originPos.x - mousePoint.x) > Mathf.Abs(originPos.z - mousePoint.z)) mousePoint = new Vector3(Mathf.Round(mousePoint.x), 0, mousePoint.z);
            else mousePoint = new Vector3(mousePoint.x, 0, Mathf.Round(mousePoint.z));
        }
        return mousePoint;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        test.transform.position = GetMousePoint(true, false);
    }
}
