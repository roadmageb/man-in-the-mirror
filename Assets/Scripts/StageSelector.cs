using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class StageSelector : SingletonBehaviour<StageSelector>
{
    public static string selectedStage;
    public static string nextStage;
    public TextAsset[] stage;
    public List<string> stageIdxs = new List<string>();
    public int stageIdx;

    [Header("맵 추가시 반드시 바꿔줘야하는 값.각 카테고리마다의 스테이지 수")]
    public int[] categoryCounts; // 맵 추가시 반드시 바꿔줘야하는 값. 각 카테고리마다의 스테이지 수
    public string[] categoryTitles; // 카테고리 추가시 추가해줘야하는 값, Length가 위의 Length와 같아야함.

    public ClearData playerData;

    [Header("UI Settings")]
    public GameObject buttonUI;
    public GameObject titleUI;
    public Sprite tutorialFalse; 
    public Color tutorialFalseColor = new Color(1f, 0.9921569f, 0.8666667f);
    public Sprite tutorialTrue;
    public Color tutorialTrueColor = new Color(0.9686275f, 0.9137256f, 0.04313726f);
    public Sprite mainFalse;
    public Color mainFalseColor = Color.white;
    public Sprite mainTrue;
    public Color mainTrueColor = new Color(0.1921569f, 1f, 0.3843138f);
    int maxRow = 8; // y-=155
    private Vector3 generatePoint = new Vector3(-470, 360); // x+=160
    private Vector3 titleGeneratePoint = new Vector3(-770, 360);

    public void GenerateStageUI()
    {
        int isColorSel = 1;
        int stageIdxCounter = 0;
        for (int i = 0; i < categoryCounts.Length; i++) // "Stagei-j"
        {
            var nameInst = Instantiate(titleUI, transform);
            nameInst.transform.localPosition = titleGeneratePoint;
            nameInst.GetComponent<Text>().text = categoryTitles[i];
            if (isColorSel < 0) nameInst.GetComponent<Text>().color = tutorialTrueColor;
            else nameInst.GetComponent<Text>().color = mainTrueColor;

            int rowCount = 0;
            for (int j = 0; j < categoryCounts[i]; j++)
            {
                var uiInst = Instantiate(buttonUI, transform);
                var uiText = uiInst.GetComponentInChildren<Text>();
                string uiStage = (i + 1) + "_" + (j + 1);
                stageIdxs.Add(uiStage);
                string nextStage = (j + 1 < categoryCounts[i]) ? ((i + 1) + "_" + (j + 2)) : ((i + 2) + "_0");
                uiInst.GetComponent<Button>().onClick.AddListener(() => StartSelectedStage(uiStage, nextStage, stageIdxCounter));
                stageIdxCounter++;
                uiInst.transform.localPosition = generatePoint;
                uiText.text = (j + 1).ToString();
                if (playerData.isCleared.ContainsKey(uiStage) && playerData.isCleared[uiStage])
                {
                    if (isColorSel < 0)
                    {
                        uiInst.GetComponent<Image>().sprite = tutorialTrue;
                        uiText.color = tutorialTrueColor;
                    }
                    else
                    {
                        uiInst.GetComponent<Image>().sprite = mainTrue;
                        uiText.color = mainTrueColor;
                    }
                }
                else
                {
                    if (isColorSel < 0)
                    {
                        uiInst.GetComponent<Image>().sprite = tutorialFalse;
                        uiText.color = tutorialFalseColor;
                    }
                    else
                    {
                        uiInst.GetComponent<Image>().sprite = mainFalse;
                        uiText.color = mainFalseColor;
                    }
                }

                if ((rowCount + 1) / maxRow > 0)
                {
                    generatePoint += new Vector3(-160 * (maxRow - 1), -155);
                    titleGeneratePoint.y -= 155;
                    rowCount = 0;
                }
                else generatePoint += new Vector3(160, 0);
                rowCount++;
            }
            generatePoint.x = -470;
            generatePoint.y -= 180;
            titleGeneratePoint.y -= 180;
            isColorSel *= -1;
        }
    }

    public void StartSelectedStage(string stageStr, string nextStr, int stageIdx)
    {
        selectedStage = stageStr;
        nextStage = nextStr;
        this.stageIdx = stageIdx;
        gameObject.GetComponent<Canvas>().enabled = false;
        SceneManager.LoadScene("PlayStage");
    }

    public void SaveClearData(string stage = "", bool isClear = false)
    {
        if (stage != "")
        {
            if (playerData.isCleared.ContainsKey(stage))
            {
                if (isClear) playerData.isCleared[stage] = isClear;
            }
            else playerData.isCleared.Add(stage, isClear);
        }

        string jsonData = JsonConvert.SerializeObject(playerData);
        File.WriteAllText("./saveData.json", jsonData);
    }

    public void LoadClearData()
    {
        if (File.Exists("./saveData.json"))
        {
            Debug.Log("data Load");
            string strData = File.ReadAllText("./saveData.json");
            playerData = JsonConvert.DeserializeObject<ClearData>(strData);
        }
        else
        {
            Debug.Log("generate New Data");
            playerData = new ClearData();
            SaveClearData();
        }
    }

    public class ClearData
    {
        public Dictionary<string, bool> isCleared = new Dictionary<string, bool>();
    }

    void Awake()
    {
        DontDestroyOnLoad(this);
        stage = Resources.LoadAll<TextAsset>("Stages");
        LoadClearData();
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateStageUI();
        selectedStage = "0_0";
    }
}
