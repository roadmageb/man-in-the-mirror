using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIController : MonoBehaviour
{
    public bool isMenuOn = false;
    public GameObject menuObject;
    public Text titleText;

    [SerializeField]
    private bool wasCursorLocked = false;

    public void ToggleMenu(bool forceClose = false)
    {
        menuObject.SetActive(!(forceClose || isMenuOn));
        isMenuOn = !(forceClose || isMenuOn);
        if (wasCursorLocked)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            wasCursorLocked = false;
            GameManager.inst.isGameOver = false;
        }
        else if (Cursor.lockState == CursorLockMode.Locked)
        {
            wasCursorLocked = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            GameManager.inst.isGameOver = true;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }
}
