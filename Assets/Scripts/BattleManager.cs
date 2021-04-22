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
    public StroopTestSettings[] testSettings;
    public StroopTestSettings currentTestSettings;
    public Queue<DefensePrompt> defenseQueue;
    public string battleMode = "defense";
    public DefensePrompt currentDefense;

    public Level currentLevelSongInfo;

    public StatusInfo statusInfo;

    public bool started;
    public bool battleOver = false;

    public bool debugMode = true;

    public int defensePhaseCount = 1;

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

        // if (debugMode)
        // {
        //     LoadLevelInfo("aaaaa");
        //     //currentLevelSongInfo.songInfo.indicatorDict = new Dictionary<double, Indicator>();
        //     UIManager.current.SetupIndicators();
        //     FillDefenseQueue();

        // }
    }

    public void LoadLevelInfo(string sceneName)
    {
        // Debug.Log("loading song info for " + currentLevelSongInfo.fmodSongName);
        MusicManager.current.LoadSong(currentLevelSongInfo.fmodSongName);
        //currentLevelSongInfo.songInfo.indicatorDict = new Dictionary<double, Indicator>();

        //so before we do shit, we gotta instnatiate the currentsonginfo 
        UIManager.current.SetupIndicators();


        //before we fill the defense queue load the stroop test settings for the level
        currentTestSettings = GetStroopTestByName(sceneName);


        FillDefenseQueue();

        //tell the animation controller to do its fuckin job
        AnimationManager.current.FindAnimationControllersInScene();



        //load the defense queue with enough indicators depending on the defense phase length
    }

    StroopTestSettings GetStroopTestByName(string sceneName)
    {
        Debug.Log("trying to load stroop settings for scene " + sceneName);



        foreach (StroopTestSettings s in testSettings)
        {
            if (s.sceneName == sceneName)
            {
                return s;

            }
        }



        Debug.LogError("COULDNT FIND STROOP SETTINGS FOR SCENE " + sceneName);
        Debug.Break();
        return null;
    }


    //TODO: so this needs to load the options depending on the current stroop test thats loaded
    public void FillDefenseQueue()
    {

        defenseQueue.Clear();
        defenseQueueLength = currentLevelSongInfo.defensePhaseLength / 2;

        //go through the current tests and break them up into levels 

        List<DefensePrompt> legalPrompts = new List<DefensePrompt>();

        foreach (DefensePrompt d in currentTestSettings.tests)
        {
            if (d.levelUnlocked <= defensePhaseCount)
            {
                legalPrompts.Add(d);
            }
        }

        //TODO: make it so you cant get the same one too many times in a row

        for (int i = 0; i < defenseQueueLength - 1; i++)
        {
            defenseQueue.Enqueue(legalPrompts[Random.Range(0, legalPrompts.Count)]);
            Debug.Log(defenseQueue.Peek().text);
        }
    }



    //TODO: so depending on if we're half time or not we just need to have the circles close smaller and the windows open differently
    public void DequeuDefensePrompt(bool halfTime)
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
            UIManager.current.SpawnDefenseIndicatorRing(halfTime);
            currentDefense = defenseQueue.Dequeue();
            //Debug.Log(currentDefense.text);
            UIManager.current.SpawnDefensePrompt(currentDefense);
            //tell the input manager to get ready to open up a window for input 
            InputManager.current.OpenDefeneseWindow(halfTime);
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

                //advance the defense count 
                defensePhaseCount++;

                //requeue a bunch more defense prompts into the queue
                FillDefenseQueue();
                battleMode = "defense";
                defenseFirstBeatBreak = true;
                //UIManager.current.EnableDefenseModeUi();
                currentBeatCounter = 1;

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


