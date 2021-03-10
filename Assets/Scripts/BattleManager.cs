using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{

    public static BattleManager current;
    public string currentBattleSongName;
    public SongInfo currentSongInfo;

    //defense mode queue 

    public int defenseQueueLength = 16;
    public Queue<bool> defenseQueue;


    private void Awake()
    {
        current = this;
    }

    void Start()
    {
        //load the midi data for the current song name into two lists then send that off to the 
        //indicator manager and the input manager 

        SongInfo info = MusicManager.current.getMidiInfo(currentBattleSongName);


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
    }


    // Update is called once per frame
    void Update()
    {

    }
}
