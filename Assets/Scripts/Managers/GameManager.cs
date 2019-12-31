using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using Newtonsoft.Json;

public class GameManager : SingletonBehaviour<GameManager>
{
    [Header("Saved Data")]
    /// <summary>
    /// The index of the current stage.
    /// </summary>
    public TextAsset currentStage;
    public string stageStrIdx;

    [Header("UIs in Scene")]
    public ClearUIGenerator uiGenerator;
    public BulletUIGenerator bulletUIGenerator;
    public CommentUIGenerator commentUIGenerator;
    public MenuUIController menuUIController;
    public Image whiteout;
    public GameObject clearUI;
    public GameObject clearUInextBtn;
    public GameObject buttonUIs;

    [Header("Stage Data")]
    public bool isGameOver = false;
    public bool isPlayerMoving, isPlayerShooting, isZooming, isBulletFlying, isFast;

    public int[] clearIndex = new int[9];
    public int clearCounter = 0;
    public static int nFloor, nTurret, nCase, nPlayer, aFloor, aTurret, aCase, white, black;

    public void ResetClearIndex()
    {
        //Reset clear index to -1.
        for (int i = 0; i < clearIndex.Length; i++) clearIndex[i] = -1;
        nFloor = nTurret = nCase = nPlayer = aFloor = aTurret = aCase = white = black = -1;
    }

    //Find and set the index of clear conditions of the map to clear type.
    public void SetClearIndex(Map map)
    {
        foreach (var child in map.clearConditions)
        {
            clearIndex[(int)child.type] = map.clearConditions.IndexOf(child);
            clearCounter++;
        }
        nFloor = clearIndex[(int)ClearType.NFloor];
        nTurret = clearIndex[(int)ClearType.NTurret];
        nCase = clearIndex[(int)ClearType.NCase];
        nPlayer = clearIndex[(int)ClearType.NPlayer];
        aFloor = clearIndex[(int)ClearType.AllFloor];
        aTurret = clearIndex[(int)ClearType.AllTurret];
        aCase = clearIndex[(int)ClearType.AllCase];
        white = clearIndex[(int)ClearType.White];
        black = clearIndex[(int)ClearType.Black];
    }

    public void StartStage(MapEditor.MapSaveData loadedMapData = null)
    {
        if(loadedMapData == null) loadedMapData = JsonConvert.DeserializeObject<MapEditor.MapSaveData>(currentStage.ToString());
        MapManager.inst.LoadMap(loadedMapData);
        menuUIController.titleText.text = "Stage\n" + (stageStrIdx.Replace("_", " - "));
        StartCoroutine(Whiteout(false));
    }

    IEnumerator Whiteout(bool goToWhite)
    {
        whiteout.gameObject.SetActive(true);
        float setTime = 0.2f;
        float resetTime = 1.5f;
        if (goToWhite)
        {
            for (float i = 0; i < setTime; i += Time.deltaTime)
            {
                whiteout.color = new Color(1, 1, 1, Mathf.Sin((i / setTime) * (Mathf.PI / 2)));
                yield return null;
            }
            whiteout.color = new Color(1, 1, 1, 1);
        }
        else
        {
            for (float i = resetTime; i > 0; i -= Time.deltaTime)
            {
                whiteout.color = new Color(1, 1, 1, i / resetTime);
                yield return null;
            }
            whiteout.color = new Color(1, 1, 1, 0);
            whiteout.gameObject.SetActive(false);
        }
    }
    
    public IEnumerator ClearStage()
    {
        yield return new WaitForSeconds(0.1f);
        if (clearCounter == 0 && !isGameOver)
        {
            if (isPlayerShooting) yield return StartCoroutine(Camera.main.gameObject.GetComponent<CameraController>().ZoomOutFromPlayer(PlayerController.inst.currentPlayer));
            yield return null;
            clearUI.SetActive(true);
            if (StageInfo.inst.nextStage.Length < 3) clearUInextBtn.SetActive(false);
            buttonUIs.SetActive(false);
            Debug.Log("Stage Clear!");

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            isGameOver = true;
            if (!StageInfo.inst.isMapEditor) StageSelector.inst.SaveClearData(stageStrIdx, true);
        }
    }

    public void GameOver(bool onlyRestart = false)
    {
        if (!onlyRestart) Debug.Log("Game Over!");
        if (!StageInfo.inst.isMapEditor) StageSelector.inst.SaveClearData(stageStrIdx, false);
        isGameOver = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        StopAllCoroutines();
        uiGenerator.ResetAllClearUIs();
        StartCoroutine(RestartStage());
    }

    public IEnumerator RestartStage()
    {
        StartCoroutine(Whiteout(true));
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToStageSelect()
    {
        if(!StageInfo.inst.isMapEditor)
        {
            StageSelector.inst.GetComponent<Canvas>().enabled = true;
            StageSelector.inst.RefreshStageUI();
            SceneManager.LoadScene("SelectStage");
        }
        else
        {
            SceneManager.LoadScene("MapEditor");
        }
    }

    public void LoadNextStage()
    {
        if (!StageInfo.inst.isMapEditor)
        {
            StageInfo.inst.selectedStage = StageInfo.inst.nextStage;
            StageSelector.inst.stageIdx++;
            if (StageSelector.inst.stageIdxs.Count > StageSelector.inst.stageIdx + 1)
            {
                var tempNext = StageSelector.inst.stageIdxs[StageSelector.inst.stageIdx + 1];
                if (tempNext[2] == '1') StageInfo.inst.nextStage = "";
                else StageInfo.inst.nextStage = tempNext;
            }
            else
            {
                StageInfo.inst.nextStage = "";
            }

            StartCoroutine(RestartStage());
        }
    }

    void AdjustSettings()
    {
        FindObjectOfType<PostProcessVolume>().enabled = StageSelector.inst.gameSettings["postProcessing"];
        Camera.main.GetComponent<PostProcessLayer>().enabled = StageSelector.inst.gameSettings["postProcessing"];
    }

    // Start is called before the first frame update
    void Start()
    {
        isGameOver = false;
        stageStrIdx = StageInfo.inst.selectedStage;
        if (!StageInfo.inst.isMapEditor)
        {
            currentStage = Resources.Load<TextAsset>("Stages/" + "stage" + StageInfo.inst.selectedStage);
            if (MapManager.inst.emptyMap != null) StartStage();
            AdjustSettings();
        }
        else StartStage(StageInfo.inst.testMap);
    }
}
