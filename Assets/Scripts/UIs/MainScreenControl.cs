using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScreenControl : MonoBehaviour
{
    public GameObject mainScreen;
    public GameObject settingMenu;
    public GameObject offMenu;

    public bool isSettingMenuOn = false;
    public bool isTurnOffMenuOn = false;

    IEnumerator Darker(bool showMain)
    {
        Image backImg = mainScreen.GetComponent<Image>();
        Image[] textImgs = mainScreen.GetComponentsInChildren<Image>();
        textImgs[0] = textImgs[2];
        float startTime = Time.time;
        Color alphaPos = new Color(0, 0, 0, 1);
        Color colorPos = new Color(1, 1, 1, 0);
        yield return null;

        if (showMain)
        {
            mainScreen.SetActive(true);
            while (startTime + 1 > Time.time)
            {
                backImg.color += Time.deltaTime * alphaPos;
                yield return new WaitForEndOfFrame();
            }
            while (startTime + 1.5f > Time.time)
            {
                backImg.color += Time.deltaTime * colorPos * 2;
                foreach (var text in textImgs) text.color += Time.deltaTime * (colorPos + alphaPos) * 2;
                yield return new WaitForEndOfFrame();
            }
            backImg.color = colorPos + alphaPos;
            foreach (var text in textImgs) text.color = colorPos + alphaPos;
        }
        else
        {
            while (startTime + 1 > Time.time)
            {
                backImg.color -= Time.deltaTime * colorPos;
                textImgs[1].color -= Time.deltaTime * colorPos;
                yield return new WaitForEndOfFrame();
            }
            while (startTime + 1.5f > Time.time)
            {
                textImgs[0].color -= Time.deltaTime * colorPos * 2;
                yield return new WaitForEndOfFrame();
            }
            foreach (var text in textImgs) text.color = 0 * colorPos;
            yield return new WaitForSeconds(0.5f);

            startTime = Time.time;
            while (startTime + 0.5f > Time.time)
            {
                backImg.color -= Time.deltaTime * alphaPos * 2;
                yield return new WaitForEndOfFrame();
            }
            backImg.color = 0 * colorPos;
            mainScreen.SetActive(false);
        }
    }

    public void ShowMainScreen()
    {
        StartCoroutine(Darker(true));
    }
    public void HideMainScreen()
    {
        StartCoroutine(Darker(false));
    }

    public void ToggleTurnOffMenu(bool forceOff = false)
    {
        offMenu.SetActive(!(forceOff || isTurnOffMenuOn));
        isTurnOffMenuOn = !(forceOff || isTurnOffMenuOn);
    }
    public void TurnOff()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    public void ToggleSettingMenu(bool forceOff = false)
    {
        settingMenu.SetActive(!(forceOff || isSettingMenuOn));
        isSettingMenuOn = !(forceOff || isSettingMenuOn);
    }
}
