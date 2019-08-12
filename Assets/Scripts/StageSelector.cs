using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class StageSelector : SingletonBehaviour<StageSelector>
{
    public bool isLoaded = false;
    public static string selectedStage;
    public static string nextStage;
    public TextAsset[] stage;
    public List<string> stageIdxs = new List<string>();
    public int stageIdx;
    public Dictionary<string, bool> gameSettings = new Dictionary<string, bool>()
    {
        { "postProcessing", true },
        { "sound", true }
    };

    [Header("맵 추가시 반드시 바꿔줘야하는 값.각 카테고리마다의 스테이지 수")]
    public int[] categoryCounts; // 맵 추가시 반드시 바꿔줘야하는 값. 각 카테고리마다의 스테이지 수
    public string[] categoryTitles; // 카테고리 추가시 추가해줘야하는 값, Length가 위의 Length와 같아야함.

    public ClearData playerData;

    [Header("UI Settings")]
    public GameObject buttonUI;
    public GameObject titleUI;
    public Transform scrollTransform;
    public Sprite tutorialFalse; 
    public Color tutorialFalseColor = new Color(1f, 0.9921569f, 0.8666667f);
    public Sprite tutorialTrue;
    public Color tutorialTrueColor = new Color(0.9686275f, 0.9137256f, 0.04313726f);
    public Sprite mainFalse;
    public Color mainFalseColor = Color.white;
    public Sprite mainTrue;
    public Color mainTrueColor = new Color(0.1921569f, 1f, 0.3843138f);
    int maxRow = 8; // y-=155
    private Vector3 generatePoint = new Vector3(-470, -220); // x+=160
    private Vector3 titleGeneratePoint = new Vector3(-770, -220);
    private List<Image> buttons = new List<Image>();

    [Header("기타 다른 메뉴세팅들")]
    public GameObject mainScreen;
    public GameObject optionScreen;

    public void GenerateStageUI()
    {
        int isColorSel = 1;
        int stageIdxCounter = 0;
        for (int i = 0; i < categoryCounts.Length; i++) // "Stagei-j"
        {
            var nameInst = Instantiate(titleUI, scrollTransform);
            nameInst.transform.localPosition = titleGeneratePoint;
            nameInst.GetComponent<Text>().text = categoryTitles[i];
            if (isColorSel < 0) nameInst.GetComponent<Text>().color = tutorialTrueColor;
            else nameInst.GetComponent<Text>().color = mainTrueColor;

            int rowCount = 0;
            for (int j = 0; j < categoryCounts[i]; j++)
            {
                var uiInst = Instantiate(buttonUI, scrollTransform);
                buttons.Add(uiInst.GetComponent<Image>());
                var uiText = uiInst.GetComponentInChildren<Text>();
                string uiStage = (i + 1) + "_" + (j + 1);
                stageIdxs.Add(uiStage);
                string nextStage = (j + 1 < categoryCounts[i]) ? ((i + 1) + "_" + (j + 2)) : "";
                int _stageidx = stageIdxCounter++;
                uiInst.GetComponent<Button>().onClick.AddListener(() => StartSelectedStage(uiStage, nextStage, _stageidx));
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

    public void RefreshStageUI()
    {
        int isColorSel = 1;
        for (int i = 0; i < categoryCounts.Length; i++)
        { 
            for (int j = 0; j < categoryCounts[i]; j++)
            {
                string uiStage = (i + 1) + "_" + (j + 1);
                int uiIdx = -1;
                for (int k = 0; k < stageIdxs.Count; k++)
                {
                    if (stageIdxs[k] == uiStage)
                    {
                        uiIdx = k;
                        break;
                    }
                }

                if (uiIdx >= 0)
                {
                    if (playerData.isCleared.ContainsKey(uiStage) && playerData.isCleared[uiStage])
                    {
                        if (isColorSel < 0)
                        {
                            buttons[uiIdx].sprite = tutorialTrue;
                            buttons[uiIdx].GetComponentInChildren<Text>().color = tutorialTrueColor;
                        }
                        else
                        {
                            buttons[uiIdx].sprite = mainTrue;
                            buttons[uiIdx].GetComponentInChildren<Text>().color = mainTrueColor;
                        }
                    }
                    else
                    {
                        if (isColorSel < 0)
                        {
                            buttons[uiIdx].sprite = tutorialFalse;
                            buttons[uiIdx].GetComponentInChildren<Text>().color = tutorialFalseColor;
                        }
                        else
                        {
                            buttons[uiIdx].sprite = mainFalse;
                            buttons[uiIdx].GetComponentInChildren<Text>().color = mainFalseColor;
                        }
                    }
                }
            }
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

    public void ResetClearData()
    {
        Debug.Log("Reset Clear Data");
        playerData = new ClearData();
        SaveClearData();
        RefreshStageUI();
    }

    public class ClearData
    {
        public Dictionary<string, bool> isCleared = new Dictionary<string, bool>();
    }

    public void ToggleSetting(string key)
    {
        if (gameSettings.ContainsKey(key))
        {
            gameSettings[key] = !gameSettings[key];
        }
        else Debug.LogError("gameSettings have no key with name " + key);
    }

    void Awake()
    {
        if (!inst.isLoaded)
        {
            DontDestroyOnLoad(this);
            isLoaded = true;
        }
        else Destroy(gameObject);
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
