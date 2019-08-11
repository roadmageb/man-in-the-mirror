using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletHoverUI : MonoBehaviour
{
    public GameObject hoverUI;
    public Text headerText;
    public Text bodyText;

    public void OnMouseEnter()
    {
        Debug.Log("hello");
        hoverUI.SetActive(true);
    }

    public void OnMouseExit()
    {
        Debug.Log("hello");
        hoverUI.SetActive(false);
    }
}
