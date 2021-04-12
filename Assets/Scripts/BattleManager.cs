using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BattleManager : MonoBehaviour
{
    public static BattleManager current;
    public string currentBattleSongName;

    //ok so my sphaget has failed me here, this variable should be the one everyone is referencing ,not the imported one from the level info object
    public SongInfo currentSongInfo;
    //defense mode queue 
    public int defenseQueueLength = 8;

    public DefensePrompt[] defensePromptOptions;
    public Queue<DefensePrompt> defenseQueue;
    public string battleMode = "defense";
    public DefensePrompt currentDefense;

    public Level currentLevelSongInfo;

    public StatusInfo statusInfo;

    public bool started;
    public bool battleOver = false;

    public bool debugMode = true;

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
        //currentLevelSongInfo.songInfo.indicatorDict = new Dictionary<double, Indicator>();

        SongInfo info = currentLevelSongInfo.songInfo;

        defenseQueue = new Queue<DefensePrompt>();

        // FillDefenseQueue();

        currentSongInfo = info;

        // UIManager.current.SetupIndicators();

        //UIManager.current.ToggleDefenseModeUI(true);
        UIManager.current.InitVibeSlider(statusInfo.maxVibe, statusInfo.currentVibe);
        //init the input mode

        if (debugMode)
        {
            LoadLevelInfo();
            //currentLevelSongInfo.songInfo.indicatorDict = new Dictionary<double, Indicator>();
            UIManager.current.SetupIndicators();
            FillDefenseQueue();

        }
    }

    public void LoadLevelInfo()
    {
        // Debug.Log("loading song info for " + currentLevelSongInfo.fmodSongName);
        MusicManager.current.LoadSong(currentLevelSongInfo.fmodSongName);
        //currentLevelSongInfo.songInfo.indicatorDict = new Dictionary<double, Indicator>();

        //so before we do shit, we gotta instnatiate the currentsonginfo 
        UIManager.current.SetupIndicators();
        FillDefenseQueue();

        //tell the animation controller to do its fuckin job
        AnimationManager.current.FindAnimationControllersInScene();



        //load the defense queue with enough indicators depending on the defense phase length
    }

    public void FillDefenseQueue()
    {
        defenseQueueLength = currentLevelSongInfo.defensePhaseLength / 2;

        //-1 because we skip the first beat now
        //-2 for the skip at the end too
        for (int i = 0; i < defenseQueueLength - 3; i++)
        {
            defenseQueue.Enqueue(defensePromptOptions[Random.Range(0, defensePromptOptions.Length)]);
        }
    }




    //TODO: so this needs to essentially ignore the first call to this every new defense phase
    public void DequeuDefensePrompt()
    {
        if (defenseFirstBeatBreak)
        {
            defenseFirstBeatBreak = false;
            UIManager.current.EnableDefenseUI();
            return;
        }

        //tell the ui manager to present a new prompt to the screen



        if (defenseQueue.Count > 1 && currentBeatCounter <= currentLevelSongInfo.defensePhaseLength - 2)
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
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKeyDown(KeyCode.Space) && !started)
        {
            MusicManager.current.StartBattle();

            started = true;
            InputManager.current.enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            DictionaryIndicatorsDebug();
        }



        CheckStatus();
    }


    public void DictionaryIndicatorsDebug()
    {
        Debug.Log("dictionary of times to indicators");
        Debug.Log(currentSongInfo.indicatorDict.Count);
        Debug.Log("-----------------------------------");
        foreach (KeyValuePair<double, Indicator> pair in currentSongInfo.indicatorDict)
        {
            Debug.Log(pair.Key);
        }
        Debug.Log("-----------------------------------");


        Debug.Log("\n array 1 of indicators");

        foreach (double d in currentSongInfo.indicatorOneInfo)
        {
            Debug.Log(d);
        }

        Debug.Log("\n array 2 of indicators");

        foreach (double d in currentSongInfo.indicatorTwoInfo)
        {
            Debug.Log(d);
        }
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
        battleOver = true;

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

    public void WinRound()
    {
        UIManager.current.EnableWinLoseUI(true);
        InputManager.current.enabled = false;

        battleOver = true;
        //increase our progress and save the game
        //TODO: limited to 4 levels for now fix this later
        if (SaveStateManager.saveState.completedLevels < 3)
        {
            SaveStateManager.saveState.completedLevels++;
            SaveStateManager.SaveGame();
        }
    }


    public bool defenseFirstBeatBreak = true;

    public void CheckPhase()
    {
        if (battleOver)
        {
            return;
        }

        currentBeatCounter++;
        totalBeatCounter++;


        if (totalBeatCounter > +currentLevelSongInfo.songLengthInBeats)
        {
            //win state
            WinRound();
        }

        if (battleMode == "defense")
        {
            if (currentBeatCounter >= currentLevelSongInfo.defensePhaseLength)
            {
                //switch the ui to offense mode

                // UIManager.current.EnableOffenseModeUi();
                battleMode = "offense";
                currentBeatCounter = 0;
                UIManager.current.ToggleDefenseModeUI(false);
                UIManager.current.EnableOffenseUI();
                UIManager.current.SwapPhaseIcon(battleMode);
            }
            //turn off the ui for defense mode and just let the player get ready if we're 2 beats away
            else if (currentBeatCounter >= currentLevelSongInfo.defensePhaseLength - 1)
            {
                //turn off the defense mode ui
                UIManager.current.ToggleDefenseModeUI(false);
            }
            else if (currentBeatCounter >= 2)
            {
                UIManager.current.ToggleDefenseModeUI(true);
            }

        }
        else if (battleMode == "offense")
        {
            if (currentBeatCounter >= currentLevelSongInfo.offensePhaseLength + 1)
            {
                //requeue a bunch more defense prompts into the queue
                FillDefenseQueue();
                battleMode = "defense";
                defenseFirstBeatBreak = true;
                //UIManager.current.EnableDefenseModeUi();
                currentBeatCounter = 0;

                //wait to do this
                //UIManager.current.ToggleDefenseModeUI(true);
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