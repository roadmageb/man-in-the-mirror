using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageSelector : MonoBehaviour
{
    public Text stageIndex;
    public static int selectedStage;
    public TextAsset[] stage;
    int totalStageCount;

    public void ChangeStage(int i)
    {
        if(i < 0 && selectedStage == 0) selectedStage = totalStageCount - 1;
        else selectedStage = (selectedStage + i) % totalStageCount;
        stageIndex.text = "Stage : " + (selectedStage + 1);
    }

    public void StartSelectedStage()
    {
        gameObject.GetComponent<Canvas>().enabled = false;
        SceneManager.LoadScene("PlayStage");
    }

    void Awake()
    {
        DontDestroyOnLoad(this);
        stage = Resources.LoadAll<TextAsset>("Stages");
    }

    // Start is called before the first frame update
    void Start()
    {
        selectedStage = 0;
        totalStageCount = stage.Length;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
