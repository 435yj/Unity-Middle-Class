using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject escMenu;
    [SerializeField]
    private GameObject settingPanel;
    [SerializeField]
    private GameObject titleCanvas;

    [SerializeField]
    private GameObject stageCanvas;

    private bool isEsc = false;
    [SerializeField]
    private GameObject tutorialPanel;
    [SerializeField]
    private Text tutorialText;
    [SerializeField]
    private Text fireText;
    [SerializeField]
    private GameObject fireUIPanal;
    [SerializeField]
    [TextArea]
    private string[] tutorialString;
    private int tutorialTextNum = 0;
    private bool isTutorialed = false;
    private bool isTitle;
    [SerializeField]
    private Slider effectSlider;
    [SerializeField]
    private Slider bgmSlider;
    
    void Start()
    {
        if (!DataManager.Inst.isReStart)
        {
            Time.timeScale = 0f;
            isTitle = true;
            GameManager.Inst.SetGameState(GameState.Stop);
            DataManager.Inst.StartCheck();
        }
        else
        {
            titleCanvas.SetActive(false);
            fireUIPanal.SetActive(true);
        }

        SliderValueSet();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            OnClickRestart();
        }
        if (isTutorialed)
        {
            if (Input.GetMouseButtonDown(0))
            {
                NextTutorialText();
            }

        }
        if (GameManager.Inst.gameState != GameState.Clear)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isTitle && settingPanel.activeSelf)
                {
                    SoundManager.Inst.SetEffectSound(0);
                    settingPanel.SetActive(false);
                }

                if (!isEsc)
                {
                    SoundManager.Inst.SetEffectSound(0);
                    escMenu.SetActive(true);
                    GameManager.Inst.SetGameState(GameState.Stop);
                    isEsc = true;
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        OnClickReturnToGame();
                        settingPanel.SetActive(false);
                    }
                }
            }
        }
    }
    public void OnClickStartBtn()
    {
        stageCanvas.SetActive(false);
        fireUIPanal.SetActive(true);
        OnClickReturnToGame();
        if (!DataManager.Inst.CurrentPlayer.isTutorial)
        {
            TurnOnTutorialPanel();
        }
    }

    public void ClickStageBtn()
    {
        titleCanvas.SetActive(false);
        stageCanvas.SetActive(true);
    }

    public void ClickBackBtn()
    {
        titleCanvas.SetActive(true);
        stageCanvas.SetActive(false);
    }

    public void ActivePanal(GameObject panal)
    {
        SoundManager.Inst.SetEffectSound(0);
        panal.SetActive(true);
    }
    public void UnActivePanal(GameObject panal)
    {
        SoundManager.Inst.SetEffectSound(0);
        OnClickReturnToGame();
        panal.SetActive(false);
    }

    public void OnClickReturnToGame()
    {
        SoundManager.Inst.SetEffectSound(0);
        isEsc = false;
        escMenu.SetActive(false);
        isTitle = false;
        Time.timeScale = 1;
        GameManager.Inst.SetGameState(GameState.Start);
    }
    public void OnClickSetting()
    {
        SoundManager.Inst.SetEffectSound(0);
        escMenu.SetActive(false);
        settingPanel.SetActive(true);
    }
    public void OnClickExit()
    {
        SoundManager.Inst.SetEffectSound(0);
        Application.Quit();
    }
    public void OnClickRestart()
    {
        Time.timeScale = 1f;
        GameManager.Inst.SetGameState(GameState.Start);
        SceneManager.LoadScene("Main");
    }
    public void TurnOnTutorialPanel()
    {
        tutorialPanel.SetActive(true);
        tutorialText.text = tutorialString[tutorialTextNum];
        isTutorialed = true;
        DataManager.Inst.TutorialTurnOn();
    }
    public void NextTutorialText()
    {
        if (tutorialTextNum >= 3)
        {
            isTutorialed = false;
            tutorialPanel.SetActive(false);
        }
        else
        {
            tutorialText.text = "";
            tutorialTextNum++;
            tutorialText.DOText(tutorialString[tutorialTextNum], tutorialString[tutorialTextNum].Length * 0.03f);
        }
    }

    public void UpdateFireCountText(int count, int maxCount)
    {
        fireText.text = string.Format("{0} / {1}", count, maxCount);
    }
    public void SliderValueSet()
    {
        effectSlider.value = DataManager.Inst.CurrentPlayer.effectSoundVolume;
        bgmSlider.value = DataManager.Inst.CurrentPlayer.bgmSoundVolume;
    }
}
