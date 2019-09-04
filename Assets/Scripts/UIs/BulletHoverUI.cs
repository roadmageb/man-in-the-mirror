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
        hoverUI.SetActive(true);
    }

    public void OnMouseExit()
    {
        hoverUI.SetActive(false);
    }
}
