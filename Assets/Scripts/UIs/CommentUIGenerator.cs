using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommentUIGenerator : MonoBehaviour
{
    public GameObject commentUI;
    public Text comment;
    public string commentString = "";

    public bool isViewed = false;
    private Coroutine currentFadeOut;

    public void SetComment(string commentStr)
    {
        commentString = commentStr;
        comment.text = commentString;
        commentUI.SetActive(true);
        currentFadeOut = StartCoroutine(FadeOut());
    }

    public void ViewComment()
    {
        StopCoroutine(currentFadeOut);
        isViewed = true;
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(5f);
        float currentTime = Time.time;
        float backAlpha = 200;
        float textAlpha = 255;
        for (; currentTime + 4000 >= Time.time;)
        {
            backAlpha -= 50f * Time.deltaTime;
            textAlpha -= (255 / 4) * Time.deltaTime;
            commentUI.GetComponent<Image>().color = new Color(0.2358491f, 0.2358491f, 0.2358491f, backAlpha / 255);
            comment.color = new Color(1, 1, 1, textAlpha / 255);
            yield return null;
        }
        commentUI.SetActive(false);
        isViewed = true;
    }
}
