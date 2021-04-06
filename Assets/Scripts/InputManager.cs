using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static InputManager current;
    public float tolerance;

    public KeyCode twoButtonBindOne = KeyCode.A, twoButtonBindTwo = KeyCode.D;

    public int midiBind1, midiBind2;

    public bool defenseInputOpen = false;

    public float defenseTolerance = 0.2f;

    //one or two
    public string inputMode;

    public bool gotInputLastDefense = false;

    public bool paused = false;


    private void Awake()
    {
        current = this;
    }

    void Start()
    {
        if (PlayerPrefs.GetInt("inputMode") == 2)
        {
            inputMode = "two";
        }
        else
        {
            inputMode = "one";
        }

        InitInputs();

        this.enabled = false;
    }

    public void InitInputs()
    {
        //TODO: so this needs to essentially read from playerprefs and set some variables in here that will bind input 

        twoButtonBindOne = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("keyBind1", "A").ToUpper());
        twoButtonBindTwo = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("keyBind2", "D").ToUpper());

        midiBind1 = PlayerPrefs.GetInt("midiBind1", 0);
        midiBind2 = PlayerPrefs.GetInt("midiBind2", 0);

        // if (inputMode == "one")
        // {
        //     UIManager.current.ToggleOneButtonDefenseInput();
        // }
        // else if (inputMode == "two")
        // {
        // UIManager.current.ToggleTwoButtonDefenseInput();
        // }
    }


    // Update is called once per frame
    void Update()
    {
        TwoButtonInput();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //TODO: Open pause menu


            if (!paused)
            {
                //stop the music 
                MusicManager.current.TogglePause(true);
                //set timescale to 0


                Time.timeScale = 0;
                //open the pause menu ui
                UIManager.current.TogglePauseMenu(true);

            }
            else
            {
                //resume music
                MusicManager.current.TogglePause(false);
                //set timescale to 1
                Time.timeScale = 1;

                UIManager.current.TogglePauseMenu(false);
            }


            paused = !paused;
        }


        //TODO: need to delay this from stopping for like one more beat?
        // Debug.Log(BattleManager.current);
        // Debug.Log(BattleManager.current.totalBeatCounter);
        // Debug.Break();

        if (BattleManager.current.battleMode == "offense" || BattleManager.current.currentBeatCounter <= 1)
        {

            CheckIndicatorStatus();
        }
        else
        {

        }
    }

    public void EndBattle()
    {
        this.enabled = false;
    }


    //stop holding of midi keys (bootleg getkeydown)
    bool gotMidiInput1 = false, gotMidiInput2 = false;
    public void TwoButtonInput()
    {
        if (BattleManager.current.battleMode == "defense")
        {
            //so assuming that input 1 is true and input 2 is false for now

            //ignore inputs if we're currently in that buffer area


            if (Input.GetKeyDown(twoButtonBindOne) || (MidiJack.MidiMaster.GetKey(midiBind1) > 0.0f & !gotMidiInput1))
            {
                gotInputLastDefense = true;
                if (defenseInputOpen && BattleManager.current.currentDefense.trueOrFalse)
                {
                    //Debug.Log("Good");
                    UIManager.current.SpawnFeedBackText(true, 1);
                    defenseInputOpen = false;
                    BattleManager.current.ProcessHit(true);
                }
                else
                {
                    UIManager.current.SpawnFeedBackText(false, 1);
                    BattleManager.current.ProcessHit(false);
                }
            }
            else if (Input.GetKeyDown(twoButtonBindTwo) || (MidiJack.MidiMaster.GetKey(midiBind2) > 0.00f && !gotMidiInput2))
            {
                gotInputLastDefense = true;
                if (defenseInputOpen && !BattleManager.current.currentDefense.trueOrFalse)
                {
                    Debug.Log("Good");
                    UIManager.current.SpawnFeedBackText(true, 2);
                    defenseInputOpen = false;
                }
                else
                {
                    UIManager.current.SpawnFeedBackText(false, 2);
                    BattleManager.current.ProcessHit(false);
                }
            }
        }
        else if (BattleManager.current.battleMode == "offense")
        {
            if (Input.GetKeyDown(twoButtonBindOne) || (MidiJack.MidiMaster.GetKey(midiBind1) > 0.0f && !gotMidiInput1))
            {
                //check and see if theres currently an indicator in lane one that is ready to be hit

                if (Mathf.Abs((float)BattleManager.current.currentLevelSongInfo.songInfo.indicatorOneInfo[0] - (float)MusicManager.current.timelineInfo.currentPosition) < tolerance)
                {
                    //time to do a hit
                    //destroy the indicator attacked to the time
                    UIManager.current.SpawnFeedBackText(true, 1);

                    //TODO: debug this shit

                    Debug.Log(BattleManager.current.currentLevelSongInfo.songInfo.indicatorDict);
                    Destroy(BattleManager.current.currentLevelSongInfo.songInfo.indicatorDict[BattleManager.current.currentLevelSongInfo.songInfo.indicatorOneInfo[0]].gameObject);
                    //remove it from the dictioanry as well

                    BattleManager.current.currentLevelSongInfo.songInfo.indicatorDict.Remove(BattleManager.current.currentLevelSongInfo.songInfo.indicatorOneInfo[0]);
                    //remove the array entry as well
                    BattleManager.current.currentLevelSongInfo.songInfo.indicatorOneInfo.RemoveAt(0);

                    Debug.Log("hit!");
                    BattleManager.current.ProcessHit(true);
                }

            }
            else if (Input.GetKeyDown(twoButtonBindTwo) || (MidiJack.MidiMaster.GetKey(midiBind2) > 0.0f && !gotMidiInput2))
            {
                //check and see if theres currently an indicator in lane two that is ready to be hit
                UIManager.current.SpawnFeedBackText(true, 2);

                //TODO: debug why this causes a reference not foun
                if (BattleManager.current.currentLevelSongInfo.songInfo.indicatorDict[BattleManager.current.currentLevelSongInfo.songInfo.indicatorTwoInfo[0]] != null)
                {
                    Destroy(BattleManager.current.currentLevelSongInfo.songInfo.indicatorDict[BattleManager.current.currentLevelSongInfo.songInfo.indicatorTwoInfo[0]].gameObject);

                }

                BattleManager.current.currentLevelSongInfo.songInfo.indicatorDict.Remove(BattleManager.current.currentLevelSongInfo.songInfo.indicatorTwoInfo[0]);
                //remove the array entry as well
                BattleManager.current.currentLevelSongInfo.songInfo.indicatorTwoInfo.RemoveAt(0);

                // Debug.Log("hit!");
                BattleManager.current.ProcessHit(true);
            }
        }


        //bootleg getkeydown
        if (MidiJack.MidiMaster.GetKey(midiBind1) > 0.0f && !gotMidiInput1)
        {
            gotMidiInput1 = true;
        }
        if (MidiJack.MidiMaster.GetKey(midiBind2) > 0.0f)
        {
            gotMidiInput2 = true;
        }

        if (MidiJack.MidiMaster.GetKeyUp(midiBind1) && gotMidiInput1)
        {
            gotMidiInput1 = false;
        }
        if (MidiJack.MidiMaster.GetKeyUp(midiBind2) && gotMidiInput2)
        {
            gotMidiInput2 = false;
        }

    }


    public void OpenDefeneseWindow()
    {
        StartCoroutine(openWindowRoutine(MusicManager.current.timelineInfo.currentPosition + ((1 - defenseTolerance) * (60f / 80f) * 1000), true));
        StartCoroutine(openWindowRoutine(MusicManager.current.timelineInfo.currentPosition + ((1 + defenseTolerance) * (60f / 80f) * 1000), false));
    }


    public IEnumerator openWindowRoutine(double time, bool toggle)
    {
        yield return new WaitUntil(() => MusicManager.current.timelineInfo.currentPosition >= time);
        defenseInputOpen = toggle;

        if (toggle == false)
        {
            UIManager.current.ClearDefenseAssets();
            //clear the text as well
        }

        if (toggle == false && !gotInputLastDefense)
        {
            BattleManager.current.ProcessHit(false);
            UIManager.current.SpawnFeedBackText(false, 0);
        }

        //UIManager.current.ToggleDefenseInputUi();
    }

    void CheckIndicatorStatus()
    {
        if (BattleManager.current.currentSongInfo.indicatorOneInfo.Count > 0 && BattleManager.current.currentSongInfo.indicatorOneInfo[0] < MusicManager.current.timelineInfo.currentPosition - tolerance)
        {

            //so before we destroy these, check and make sure they're actually in the dictionary
            if (BattleManager.current.currentSongInfo.indicatorDict.ContainsKey(BattleManager.current.currentSongInfo.indicatorOneInfo[0]))
            {
                Destroy(BattleManager.current.currentSongInfo.indicatorDict[BattleManager.current.currentSongInfo.indicatorOneInfo[0]].gameObject);
            }

            BattleManager.current.currentSongInfo.indicatorOneInfo.RemoveAt(0);

            UIManager.current.SpawnFeedBackText(false, 0);
            //delete the indicator too
            //Debug.Log("missed a kick");
            BattleManager.current.ProcessHit(false);

        }

        if (BattleManager.current.currentSongInfo.indicatorTwoInfo.Count > 0 && BattleManager.current.currentSongInfo.indicatorTwoInfo[0] < MusicManager.current.timelineInfo.currentPosition - tolerance)
        {
            if (BattleManager.current.currentSongInfo.indicatorDict.ContainsKey(BattleManager.current.currentSongInfo.indicatorTwoInfo[0]))
            {
                Destroy(BattleManager.current.currentSongInfo.indicatorDict[BattleManager.current.currentSongInfo.indicatorTwoInfo[0]].gameObject);
            }
            BattleManager.current.currentSongInfo.indicatorTwoInfo.RemoveAt(0);

            //delete the indicator too
            //Debug.Log("missed a snare");
            UIManager.current.SpawnFeedBackText(false, 0);
            BattleManager.current.ProcessHit(false);

        }

        //if theres no indicators left turn off the input
        if (BattleManager.current.currentSongInfo.indicatorOneInfo.Count < 1 && BattleManager.current.currentSongInfo.indicatorTwoInfo.Count < 1)
        {
            this.enabled = false;
        }
    }
}
