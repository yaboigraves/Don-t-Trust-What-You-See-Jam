﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager current;
    public GameObject indicator;
    public Transform indicatorContainer;
    public Transform indicatorOneDestination, indicatorTwoDestination;
    //100 pixels for every beat
    public int pixelToSeconds = 100;
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
    public void SetupIndicators()
    {
        SongInfo info = BattleManager.current.currentLevelSongInfo.songInfo;
        //so when we spawn in the songinfo stuff, we should map the times to the indicators to reference them
        //assuming every beat is based on the ratio, plop them down
        for (int i = 0; i < info.indicatorOneInfo.Count; i++)
        {
            Vector3 indicPos = indicatorContainer.transform.position + new Vector3(0, (float)info.indicatorOneInfo[i] / 1000 * pixelToSeconds, 0);
            Indicator indic = Instantiate(indicator, indicPos, Quaternion.identity, indicatorContainer.transform).GetComponent<Indicator>();
            indic.SetIndicatorTime((float)info.indicatorOneInfo[i], indicatorOneDestination);

            // Debug.Log(info.indicatorDict);
            //map the indicator at this time in the song info dictionary
            info.indicatorDict[info.indicatorOneInfo[i]] = indic;
        }

        for (int i = 0; i < info.indicatorTwoInfo.Count; i++)
        {
            Vector3 indicPos = indicatorContainer.transform.position + new Vector3(0, (float)info.indicatorTwoInfo[i] / 1000 * pixelToSeconds, 0);
            Indicator indic = Instantiate(indicator, indicPos, Quaternion.identity, indicatorContainer.transform).GetComponent<Indicator>();
            indic.SetIndicatorTime((float)info.indicatorTwoInfo[i], indicatorTwoDestination);
            info.indicatorDict[info.indicatorTwoInfo[i]] = indic;
        }
    }

    //spawn this at the pad location
    public void SpawnFeedBackText(bool didHit, int spawnPos = 0)
    {
        Vector3 feedbackSpawnPos = Vector3.zero;
        if (spawnPos == 0)
        {
            //default spot no pad hit
            feedbackSpawnPos = defensePromptText.transform.position;

        }
        else if (spawnPos == 1)
        {
            //left pad 
            feedbackSpawnPos = padText1.transform.position;
        }
        else if (spawnPos == 2)
        {
            //right pad
            feedbackSpawnPos = padText2.transform.position;
        }

        FeedbackText fText = Instantiate(feedbackText, feedbackSpawnPos, Quaternion.identity, feedbackContainer).GetComponent<FeedbackText>();
        fText.SetText(didHit);
        // fText.startPosition = Vector3.zero;

    }

    public void SpawnDefensePrompt(DefensePrompt prompt)
    {
        if (prompt.sprite)
        {
            //instantiate the sprite
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
    }

    public void ToggleDefenseInputUi()
    {
        //turn on the white bar or turn it off
        defenseCenterPointIndicator.enabled = !defenseCenterPointIndicator.enabled;
    }

    //vibe slider stuff
    public void InitVibeSlider(int maxVibe, int currentVibe)
    {
        vibeBarSlider.maxValue = maxVibe;
        vibeBarSlider.value = currentVibe;
    }

    public void UpdateVibeBarSlider(int curVibe)
    {
        vibeBarSlider.value = curVibe;
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

    public void EnableDefenseModeUi()
    {
        offenseUIContainer.SetActive(false);
        defenseUIContainer.SetActive(true);
    }


    public void EnableOffenseModeUi()
    {
        otherDefenseContainer.SetActive(false);
        offenseUIContainer.SetActive(true);
        defenseUIContainer.SetActive(false);
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
    }

    public TextMeshProUGUI padText1, padText2;

    //new functions for toggling offense/defense just ignore the old ones

    public void EnableOffenseUI()
    {
        //so this is going to need to disable the prompt text, and disable the true/false text on the pads

        defensePromptText.text = "";
        padText1.text = "";
        padText2.text = "";



    }

    public void EnableDefenseUI()
    {
        padText1.text = "Yah!";
        padText2.text = "Nah!";
    }

}
