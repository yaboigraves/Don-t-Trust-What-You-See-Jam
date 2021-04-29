using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class UIManager : MonoBehaviour
{

    public static UIManager current;
    public GameObject indicator;
    public Transform indicatorContainer;
    public Transform indicatorOneDestination, indicatorTwoDestination;
    //100 pixels for every beat
    public GameObject feedbackText;
    public Transform feedbackContainer;
    public TextMeshProUGUI defensePromptText;
    public SpriteRenderer defenseCenterPointIndicator;
    public GameObject oneButtonDefenseUI, twoButtonDefenseUI;
    public GameObject defenseUIContainer, offenseUIContainer;
    public Slider vibeBarSlider;
    public GameObject winLoseContainer;
    public TextMeshProUGUI streakText;
    public GameObject otherDefenseContainer;
    public GameObject defensePromptAssetContainer;
    public VibeBar vibeBarController;
    public Sprite defenseIcon, offenseIcon;
    public Image battlePhaseIcon;
    public GameObject pauseMenuPanel;
    public GameObject defenseRingIcon;
    public GameObject indicatorsAndPadContainer;
    public GameObject gridBar, gridBarContainer;
    public RectTransform feedbackTextDefenseTextSpawnPos;
    public Transform offenseTextSpawn1, offenseTextSpawn2;
    public Camera battleUICamera;
    //so this needs to be abstracted a bit more probably but eh
    public Sprite[] indicatorArrowSprites;
    public Image[] offensePrompts;
    //so we gotta know what stroop test assets to load from and how to load it depending on the current test type
    public string currentStroopTestType;


    public TextMeshProUGUI offensePromptSwitchCounterText;

    public TextMeshProUGUI multiplierText;

    private void Awake()
    {
        current = this;
    }
    void Start()
    {

    }

    //so big sprint time
    //cocks out

    //ok

    //indicators are going to have different destinations depending on which lane they are set in here
    //we can set that when we set the indicatoe time

    public void TogglePauseMenu(bool toggle)
    {
        pauseMenuPanel.SetActive(toggle);
    }

    public void SetStroopTestType(string stroopType)
    {
        currentStroopTestType = stroopType;
    }

    public void ExitBackToMenu()
    {
        //reset the timescale back to 1
        Time.timeScale = 1;
        SceneManager.LoadScene("LevelSelect");
    }

    public void SetupGridBars()
    {
        //Debug.Log("setting up grid bars for song " + BattleManager.current.currentLevelSongInfo.fmodSongName);
        int defLength = BattleManager.current.currentLevelSongInfo.defensePhaseLength;
        int offLength = BattleManager.current.currentLevelSongInfo.offensePhaseLength;

        int phaseLength = defLength + offLength;

        int numBarIterations = BattleManager.current.currentLevelSongInfo.songLengthInBeats / phaseLength;

        // Debug.Log(defLength);
        // Debug.Log(offLength);
        // Debug.Log("bpm of current grid bars is based off " + BattleManager.current.currentLevelSongInfo.bpm);

        float beatsPerSecond = 60 / BattleManager.current.currentLevelSongInfo.bpm;
        Debug.Log("beats per second " + beatsPerSecond);

        //need to initialize this for the amount of offensephases we're going to have which depends on the total length of the song in beats

        for (int j = 0; j < numBarIterations; j++)
        {
            for (int i = defLength + (phaseLength * j); i < phaseLength + (phaseLength * j) + 1; i++)
            {
                //so these positions need to be time based, not beat based
                //convert from beat to time
                //beats * beats per second oh duh lol & 1000
                float barPos = (i) * beatsPerSecond;
                Transform bar = Instantiate(gridBar, new Vector3(0, barPos, 0), Quaternion.identity, gridBarContainer.transform).transform;
                //bar.SetParent(gridBarContainer.transform);
            }
        }
    }

    //coroutine to wait to set the gridbars up
    IEnumerator waitForMusic()
    {
        yield return new WaitUntil(() => MusicManager.current.timelineInfo.currentTempo != 0);
        // Debug.Log("setting up grid bars with bpm" + MusicManager.current.timelineInfo.currentTempo);
        //SetupGridBars();s
    }

    public void SetupIndicators()
    {
        //first set up all the bars
        //basically just need to set these up every x units between the current level song infos size of the phases

        SongInfo info = BattleManager.current.currentLevelSongInfo.songInfo;

        Debug.Log(BattleManager.current.currentLevelSongInfo.bpm);

        // StartCoroutine(waitForMusic());

        //TODO: fix this so it can read in the bpm dynamically later
        SetupGridBars();

        info.indicatorDict = new Dictionary<double, Indicator>();

        //TODO: so we need to make the positions of the indicators more uniform 
        //positions that beats should spawn at should NOT be related to time, but should rather be related to beat

        //legacy two button initialization
        // for (int i = 0; i < info.indicatorOneInfo.Count; i++)
        // {
        //     Vector3 indicPos = indicatorContainer.transform.position + new Vector3(0, (float)info.indicatorOneInfo[i] / 1000f, 0);
        //     Indicator indic = Instantiate(indicator, indicPos, Quaternion.identity, indicatorContainer.transform).GetComponent<Indicator>();
        //     indic.SetIndicatorTime((float)info.indicatorOneInfo[i], indicatorOneDestination);
        //     info.indicatorDict[info.indicatorOneInfo[i]] = indic;
        // }

        // for (int i = 0; i < info.indicatorTwoInfo.Count; i++)
        // {
        //     Vector3 indicPos = indicatorContainer.transform.position + new Vector3(0, (float)info.indicatorTwoInfo[i] / 1000, 0);
        //     Indicator indic = Instantiate(indicator, indicPos, Quaternion.identity, indicatorContainer.transform).GetComponent<Indicator>();
        //     indic.SetIndicatorTime((float)info.indicatorTwoInfo[i], indicatorTwoDestination);
        //     info.indicatorDict[info.indicatorTwoInfo[i]] = indic;
        // }





        //TODO: need to dynamically figure out what mode we're in direction,math, etc and stuff
        currentStroopTestType = BattleManager.current.currentLevelSongInfo.stroopTestType;
        //Debug.Log(currentStroopTestType);


        OffensePrompt[] prompts = BattleManager.current.currentTestSettings.offensePrompts;


        for (int i = 0; i < info.mergedIndicatorInfo.Count; i++)
        {
            Vector3 indicPos = indicatorContainer.transform.position + new Vector3(0, (float)info.mergedIndicatorInfo[i] / 1000f, 0);
            Indicator indic = Instantiate(indicator, indicPos, Quaternion.identity, indicatorContainer.transform).GetComponent<Indicator>();
            indic.SetIndicatorTime((float)info.mergedIndicatorInfo[i], indicatorOneDestination);
            info.indicatorDict[info.mergedIndicatorInfo[i]] = indic;

            //so now we can set the info just by grabbing randomly from the offense prompts
            OffensePrompt p = prompts[Random.Range(0, prompts.Length)];

            indic.SetIndicatorInfo(p.sprite, p.promptLabel, p.assetColor);

        }


        //instnatiate the object 
        BattleManager.current.currentSongInfo = info;


    }


    //so this needs to get called every 1 and 3 by the music manager
    //basically spawns a ring that scales itself down to another size over 1? beat

    //so ring close time needs to be configurable based on halftime
    public void SpawnDefenseIndicatorRing(bool halftime)
    {

        //spawn the ring

        //double check with the battle manager and make sure the beat counter for the current phase isnt small
        if (BattleManager.current.currentBeatCounter <= 2 || BattleManager.current.currentBeatCounter >= BattleManager.current.currentLevelSongInfo.defensePhaseLength - 2)
        {
            return;
        }

        GameObject ring = Instantiate(defenseRingIcon, defenseUIContainer.transform.position, Quaternion.identity, defenseUIContainer.transform);
        if (halftime)
        {
            ring.GetComponent<DefenseRingIcon>().makeHalfTime();
        }
    }


    //spawn this at the pad location
    public void SpawnFeedBackText(bool didHit, int spawnPos = 0, string statusText = "")
    {
        Vector3 feedbackSpawnPos = Vector3.zero;


        if (didHit)
        {
            pulseAll();
        }
        //depending on the phase we need to spawn these at different positions

        if (spawnPos == 0)
        {
            //default spot no pad hit
            feedbackSpawnPos = defensePromptText.transform.position;

        }
        else if (spawnPos == 1)
        {
            //left pad 
            if (BattleManager.current.battleMode == "defense")
            {
                feedbackSpawnPos = feedbackTextDefenseTextSpawnPos.position;
            }
            else if (BattleManager.current.battleMode == "offense")
            {
                //TODO: convert this to screen space

                feedbackSpawnPos = battleUICamera.WorldToScreenPoint(offenseTextSpawn1.transform.position);
            }
        }
        else if (spawnPos == 2)
        {
            //right pad
            // feedbackSpawnPos = padText2.transform.position;

            if (BattleManager.current.battleMode == "defense")
            {
                feedbackSpawnPos = feedbackTextDefenseTextSpawnPos.position;
            }
            else if (BattleManager.current.battleMode == "offense")
            {
                // //TODO: convert this to screen space
                // Debug.Log(Camera.main);
                // Debug.Log(offenseTextSpawn2.transform.position);
                // Debug.Log(Camera.main.WorldToScreenPoint(offenseTextSpawn2.transform.position));
                feedbackSpawnPos = battleUICamera.WorldToScreenPoint(offenseTextSpawn2.transform.position);
            }
        }

        FeedbackText fText = Instantiate(feedbackText, feedbackSpawnPos, Quaternion.identity, feedbackContainer).GetComponent<FeedbackText>();
        fText.SetText(didHit, statusText);
        // fText.startPosition = Vector3.zero;
    }

    public Image defenseSpriteContainer;
    public void SpawnDefensePrompt(DefensePrompt prompt)
    {
        if (prompt.sprite)
        {

            defenseSpriteContainer.sprite = prompt.sprite;
            defenseSpriteContainer.color = Color.white;
        }
        else if (prompt.model)
        {
            //instantiate the model
            GameObject promptModel = Instantiate(prompt.model, defensePromptAssetContainer.transform.position, Quaternion.identity, defensePromptAssetContainer.transform);

            if (prompt.colorModel)
            {
                promptModel.GetComponentInChildren<MeshRenderer>().material.color = prompt.assetColor;
            }
        }

        defensePromptText.text = prompt.text;
        defensePromptText.color = prompt.textColor;
    }

    public void ClearDefenseAssets()
    {
        defensePromptText.text = "";
        for (int i = 0; i < defensePromptAssetContainer.transform.childCount; i++)
        {
            Destroy(defensePromptAssetContainer.transform.GetChild(i).gameObject);
        }
        defenseSpriteContainer.sprite = null;
        defenseSpriteContainer.color = new Color(0, 0, 0, 0);
    }



    //vibe slider stuff
    public void InitVibeSlider(int maxVibe, int currentVibe)
    {
        vibeBarSlider.maxValue = maxVibe;
        vibeBarSlider.value = currentVibe;
        vibeBarController.EvaluateFillAreaColor(vibeBarSlider.value / vibeBarSlider.maxValue);

    }

    public void UpdateVibeBarSlider(int curVibe)
    {
        vibeBarSlider.value = curVibe;
        vibeBarController.EvaluateFillAreaColor(curVibe / vibeBarSlider.maxValue);
    }

    public void UpdateStreakText(int streak)
    {
        streakText.text = "Streak X " + streak.ToString();
    }

    public void ToggleOneButtonDefenseInput()
    {
        oneButtonDefenseUI.SetActive(true);
        twoButtonDefenseUI.SetActive(false);
    }

    public void ToggleTwoButtonDefenseInput()
    {
        oneButtonDefenseUI.SetActive(false);
        twoButtonDefenseUI.SetActive(true);
    }

    public void ToggleDefenseModeUI(bool toggle)
    {
        // offenseUIContainer.SetActive(false);

        if (toggle)
        {
            indicatorsAndPadContainer.SetActive(false);
            offensePromptSwitchCounterText.enabled = false;
        }
        defenseUIContainer.SetActive(toggle);


    }






    public void EnableWinLoseUI(bool didWin)
    {
        //disable the other ui
        offenseUIContainer.SetActive(false);
        defenseUIContainer.SetActive(false);

        winLoseContainer.SetActive(true);
        if (didWin)
        {
            // winLoseContainer.GetComponent<TextMeshProUGUI>().text = "You Won!";
        }
        else
        {
            // winLoseContainer.GetComponent<TextMeshProUGUI>().text = "You Lose!";
        }

        //clear all the indicators
        ClearIndicators();
    }

    public void ClearIndicators()
    {
        for (int i = 0; i < indicatorContainer.transform.childCount; i++)
        {
            Destroy(indicatorContainer.GetChild(i).gameObject);
        }
    }

    public TextMeshProUGUI padText1, padText2;

    //new functions for toggling offense/defense just ignore the old ones




    public void EnableOffenseUI()
    {
        //so this is going to need to disable the prompt text, and disable the true/false text on the pads

        defensePromptText.text = "";
        padText1.text = "";
        padText2.text = "";

        indicatorsAndPadContainer.SetActive(true);
        ToggleOffensePrompts(true);
        //turn on the counter text

        offensePromptSwitchCounterText.enabled = true;

    }

    public void ToggleMultiplierText(bool toggle, int multiplier)
    {
        if (toggle)
        {
            multiplierText.text = "x" + multiplier.ToString();
            multiplierText.enabled = true;

        }
        else
        {
            multiplierText.enabled = false;
        }
    }

    public void EnableDefenseUI()
    {
        padText1.text = "Yah!";
        padText2.text = "Nah!";
    }

    public TextMeshProUGUI phaseText;
    public void SwapPhaseIcon(string phase)
    {
        phaseText.text = phase;
        if (phase == "offense")
        {
            battlePhaseIcon.sprite = offenseIcon;

        }
        else if (phase == "defense")
        {
            battlePhaseIcon.sprite = defenseIcon;
        }
    }

    //reload the scene
    public void RetryButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //go back to level select
    public void MenuButton()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    //OFFENSE PROMPT VARIABLES

    public void ToggleOffensePrompts(bool toggle)
    {
        //enable all the offense prompts as long as theirs room in in the queue

        //calculate num prompts to actually show

        int numPrompts = 3;


        if (toggle && BattleManager.current.offenseQueue.Count < numPrompts)
        {
            numPrompts = BattleManager.current.offenseQueue.Count;
        }

        //refresh 
        for (int i = 0; i < offensePrompts.Length; i++)
        {
            offensePrompts[i].enabled = false;
        }

        for (int i = 0; i < numPrompts; i++)
        {
            offensePrompts[i].enabled = toggle;
        }


        if (offensePrompts[0].isActiveAndEnabled)
        {

            //TODO: only do this up to the amount of prompts that are there
            for (int i = 0; i < numPrompts; i++)
            {
                OffensePrompt op = BattleManager.current.offenseQueue[i];
                offensePrompts[i].sprite = op.sprite;
            }
        }
    }

    public void UpdateOffenseCountdownText(int count)
    {
        offensePromptSwitchCounterText.text = count.ToString();
        if (count == 1)
        {
            offensePromptSwitchCounterText.text = "Ready!";
        }
    }


    public List<BeatPulse> pulseBois;

    public void pulseAll()
    {
        foreach (BeatPulse b in pulseBois)
        {
            b.Pulse();
        }
    }
}
