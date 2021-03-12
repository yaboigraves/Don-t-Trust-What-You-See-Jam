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
    public Queue<bool> defenseQueue;
    public string battleMode = "defense";
    public bool currentDefense;

    public Level currentLevelSongInfo;

    public StatusInfo statusInfo;


    private void Awake()
    {
        current = this;
    }

    void Start()
    {


        // SongInfo info = MusicManager.current.getMidiInfo(currentBattleSongName);
        currentLevelSongInfo.songInfo.indicatorDict = new Dictionary<double, Indicator>();
        SongInfo info = currentLevelSongInfo.songInfo;



        defenseQueue = new Queue<bool>();

        FillDefenseQueue();

        currentSongInfo = info;


        UIManager.current.SetupIndicators();

        UIManager.current.EnableDefenseModeUi();

        //init the input mode
    }

    public void FillDefenseQueue()
    {
        defenseQueueLength = currentLevelSongInfo.defensePhaseLength / 2;
        for (int i = 0; i < defenseQueueLength; i++)
        {
            defenseQueue.Enqueue(Random.Range(0, 2) == 0);
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

        CheckStatus();
    }

    public void CheckStatus()
    {
        if (statusInfo.currentVibe < statusInfo.minVibe)
        {
            //lose state
            UIManager.current.EnableWinLoseUI(false);
            InputManager.current.enabled = false;
        }
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

                UIManager.current.EnableOffenseModeUi();
                battleMode = "offense";
                currentBeatCounter = 0;

            }
        }
        else if (battleMode == "offense")
        {
            if (currentBeatCounter >= currentLevelSongInfo.offensePhaseLength)
            {
                //requeue a bunch more defense prompts into the queue
                FillDefenseQueue();
                battleMode = "defense";
                UIManager.current.EnableDefenseModeUi();
                currentBeatCounter = 0;
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