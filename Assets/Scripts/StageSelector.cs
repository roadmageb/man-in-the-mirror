using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class StageSelector : SingletonBehaviour<StageSelector>
{
    public static int selectedStage;
    public static int nextStage;
    public TextAsset[] stage;

    public ClearData playerData;

    [Header("UI Settings")]
    public GameObject buttonUI;
    public Sprite tutorialFalse; 
    public Color tutorialFalseColor = new Color(1f, 0.9921569f, 0.8666667f);
    public Sprite tutorialTrue;
    public Color tutorialTrueColor = new Color(0.9686275f, 0.9137256f, 0.04313726f);
    public Sprite mainFalse;
    public Color mainFalseColor = Color.white;
    public Sprite mainTrue;
    public Color mainTrueColor = new Color(0.1921569f, 1f, 0.3843138f);
    int tutorialCount = 12;
    int maxRow = 8; // y-=155
    Vector3 tutorialPoint = new Vector3(-470, 265); // x+=160
    Vector3 mainPoint = new Vector3(-470, -138);
    List<Button> buttons = new List<Button>();

    public void GenerateStageUI()
    {
        int rowCount = 0;
        for (int i = 0; i < tutorialCount; i++)
        {
            var uiInst = Instantiate(buttonUI, transform);
            var uiText = uiInst.GetComponentInChildren<Text>();
            buttons.Add(uiInst.GetComponent<Button>());
            uiInst.transform.localPosition = tutorialPoint;
            uiText.text = (i + 1).ToString();
            if (playerData.isCleared.ContainsKey(i + 1) && playerData.isCleared[i + 1])
            {
                uiInst.GetComponent<Image>().sprite = tutorialTrue;
                uiText.color = tutorialTrueColor;
            }
            else
            {
                uiInst.GetComponent<Image>().sprite = tutorialFalse;
                uiText.color = tutorialFalseColor;
            }

            if ((rowCount + 1) / maxRow > 0)
            {
                tutorialPoint += new Vector3(-160 * (maxRow - 1), -155);
                rowCount = 0;
            }
            else tutorialPoint += new Vector3(160, 0);
            rowCount++;
        }
        rowCount = 0;
        for (int i = tutorialCount; i < stage.Length; i++)
        {
            var uiInst = Instantiate(buttonUI, transform);
            var uiText = uiInst.GetComponentInChildren<Text>();
            buttons.Add(uiInst.GetComponent<Button>());
            uiInst.transform.localPosition = mainPoint;
            uiText.text = (i - tutorialCount + 1).ToString();
            if (playerData.isCleared.ContainsKey(i + 1) && playerData.isCleared[i + 1])
            {
                uiInst.GetComponent<Image>().sprite = mainTrue;
                uiText.color = mainTrueColor;
            }
            else
            {
                uiInst.GetComponent<Image>().sprite = mainFalse;
                uiText.color = mainFalseColor;
            }

            if ((rowCount + 1) / maxRow > 0)
            {
                mainPoint += new Vector3(-160 * (maxRow - 1), -155);
                rowCount = 0;
            }
            else mainPoint += new Vector3(160, 0);
            rowCount++;
        }
        for (int i = 0; i < stage.Length; i++)
        {
            int _i = i;
            buttons[i].onClick.AddListener(() => StartSelectedStage(_i));
        }
    }

    public void StartSelectedStage(int stageNum)
    {
        selectedStage = stageNum;
        nextStage = stageNum + 1;
        gameObject.GetComponent<Canvas>().enabled = false;
        SceneManager.LoadScene("PlayStage");
    }

    public void SaveClearData(int stage = -1, bool isClear = false)
    {
        if (stage != -1)
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
        public Dictionary<int, bool> isCleared = new Dictionary<int, bool>();
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
        selectedStage = 0;
    }
}
