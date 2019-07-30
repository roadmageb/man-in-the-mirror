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

    public void SetComment(string commentStr)
    {
        commentString = commentStr;
        comment.text = commentString;
        commentUI.SetActive(true);
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(5f);
        float currentTime = Time.time;
        for (; currentTime + 5000 >= Time.time;)
        {
            yield return null;
        }
    }
}
