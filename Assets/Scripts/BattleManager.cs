using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BattleManager : MonoBehaviour
{
    public static BattleManager current;
    public string currentBattleSongName;
    public SongInfo currentSongInfo;
    //defense mode queue 
    public int defenseQueueLength = 8;

    public DefensePrompt[] defensePromptOptions;
    public Queue<DefensePrompt> defenseQueue;
    public string battleMode = "defense";
    public DefensePrompt currentDefense;

    public Level currentLevelSongInfo;

    public StatusInfo statusInfo;

    public GameObject levelObject;

    public bool started;

    private void Awake()
    {
        current = this;
    }

    void Start()
    {

        currentLevelSongInfo = Instantiate(currentLevelSongInfo);
        // GameObject lo = Instantiate(levelObject, trans
        // currentLevelSongInfo = lo.GetComponent<LevelObject>().level;

        // SongInfo info = MusicManager.current.getMidiInfo(currentBattleSongName);
        currentLevelSongInfo.songInfo.indicatorDict = new Dictionary<double, Indicator>();

        SongInfo info = currentLevelSongInfo.songInfo;

        defenseQueue = new Queue<DefensePrompt>();

        FillDefenseQueue();

        currentSongInfo = info;

        UIManager.current.SetupIndicators();

        UIManager.current.EnableDefenseModeUi();
        UIManager.current.InitVibeSlider(statusInfo.maxVibe, statusInfo.currentVibe);
        //init the input mode
    }

    public void LoadLevelInfo()
    {
        MusicManager.current.LoadSong(currentLevelSongInfo.fmodSongName);
    }

    public void FillDefenseQueue()
    {
        defenseQueueLength = currentLevelSongInfo.defensePhaseLength / 2;
        for (int i = 0; i < defenseQueueLength; i++)
        {
            defenseQueue.Enqueue(defensePromptOptions[Random.Range(0, defensePromptOptions.Length)]);
        }
    }

    public void DequeuDefensePrompt()
    {
        //tell the ui manager to present a new prompt to the screen

        if (defenseQueue.Count > 1)
        {
            currentDefense = defenseQueue.Dequeue();
            UIManager.current.SpawnDefensePrompt(currentDefense);
            //tell the input manager to get ready to open up a window for input 
            InputManager.current.OpenDefeneseWindow();
        }
        else
        {
            //switch the battle mode
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayerPrefs.SetInt("inputMode", 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayerPrefs.SetInt("inputMode", 2);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKeyDown(KeyCode.Space) && !started)
        {
            MusicManager.current.StartBattle();
        }

        CheckStatus();
    }

    public void CheckStatus()
    {
        if (statusInfo.currentVibe < statusInfo.minVibe)
        {
            //lose state
            // UIManager.current.EnableWinLoseUI(false);
            // InputManager.current.enabled = false;

            EndBattle();
        }
    }

    public void EndBattle()
    {
        UIManager.current.EnableWinLoseUI(false);
        InputManager.current.EndBattle();
        MusicManager.current.EndBattle();
    }

    public void ProcessHit(bool hit)
    {
        if (hit)
        {
            if (statusInfo.currentVibe + statusInfo.vibeIncreaseRate <= statusInfo.maxVibe)
            {
                statusInfo.currentVibe += statusInfo.vibeIncreaseRate;
            }

            statusInfo.streak++;
        }
        else
        {
            statusInfo.currentVibe += statusInfo.vibeDecreaseRate;
            statusInfo.streak = 0;
        }

        UIManager.current.UpdateVibeBarSlider(statusInfo.currentVibe);
        UIManager.current.UpdateStreakText(statusInfo.streak);
    }

    //call this in the music manager every beat

    public int currentBeatCounter;
    public int totalBeatCounter;
    public void CheckPhase()
    {
        currentBeatCounter++;
        totalBeatCounter++;

        if (totalBeatCounter > +currentLevelSongInfo.songLengthInBeats)
        {
            //win state
            UIManager.current.EnableWinLoseUI(true);
            InputManager.current.enabled = false;
        }

        if (battleMode == "defense")
        {
            if (currentBeatCounter >= currentLevelSongInfo.defensePhaseLength)
            {
                //switch the ui to offense mode

                // UIManager.current.EnableOffenseModeUi();
                battleMode = "offense";
                currentBeatCounter = 0;
                UIManager.current.EnableOffenseUI();
                UIManager.current.SwapPhaseIcon(battleMode);

            }
        }
        else if (battleMode == "offense")
        {
            if (currentBeatCounter >= currentLevelSongInfo.offensePhaseLength)
            {
                //requeue a bunch more defense prompts into the queue
                FillDefenseQueue();
                battleMode = "defense";
                //UIManager.current.EnableDefenseModeUi();
                currentBeatCounter = 0;
                UIManager.current.EnableDefenseUI();
                UIManager.current.SwapPhaseIcon(battleMode);
            }
        }
    }
}

[System.Serializable]
public class StatusInfo
{
    public int maxVibe;
    public int streak;

    public int currentVibe;

    public int minVibe;
    public int vibeIncreaseRate, vibeDecreaseRate;


}


//for now we're just going to have these simple options, if theres no sprite or model its just text
[System.Serializable]
public class DefensePrompt
{
    public bool trueOrFalse;
    public bool colorModel = false;
    public string text;
    public GameObject sprite;
    public GameObject model;
    public Color textColor, assetColor;
}