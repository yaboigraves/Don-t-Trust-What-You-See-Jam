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
    public int defenseQueueLength = 64;
    public Queue<bool> defenseQueue;
    public string battleMode = "defense";
    public bool currentDefense;




    private void Awake()
    {
        current = this;
    }

    void Start()
    {
        //load the midi data for the current song name into two lists then send that off to the 
        //indicator manager and the input manager 

        SongInfo info = MusicManager.current.getMidiInfo(currentBattleSongName);
        defenseQueue = new Queue<bool>();

        //initialize the defense queue stuff
        for (int i = 0; i < defenseQueueLength; i++)
        {
            defenseQueue.Enqueue(Random.Range(0, 2) == 0);
        }

        float beatsPerSecond = 60f / 80f;
        for (int i = 0; i < info.indicatorOneInfo.Count; i++)
        {
            info.indicatorOneInfo[i] *= (beatsPerSecond * 1000);
        }
        Debug.Log("Snares");
        for (int i = 0; i < info.indicatorTwoInfo.Count; i++)
        {
            info.indicatorTwoInfo[i] *= (beatsPerSecond * 1000);
        }

        currentSongInfo = info;
        //UIManager.current.SetupIndicators();

        //init the input mode
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

    }
}
