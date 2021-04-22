using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static InputManager current;
    public float tolerance, missDeleteTolerance;



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

    }


    // Update is called once per frame
    void Update()
    {
        TwoButtonInput();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
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

        if (BattleManager.current.battleMode == "offense" || BattleManager.current.currentBeatCounter <= 1)
        {

            CheckIndicatorStatus();
        }
    }

    public void EndBattle()
    {
        this.enabled = false;
    }


    //stop holding of midi keys (bootleg getkeydown)
    bool gotMidiInput1 = false, gotMidiInput2 = false;


    public string checkHitAccuracy(float hitDifference)
    {
        string status = "good";


        if (hitDifference / tolerance < 0.10f)
        {
            status = "perfect!!";
        }
        else if (hitDifference / tolerance < 0.35f)
        {
            status = "nice!";
        }

        return status;
    }


    public void TwoButtonInput()
    {
        if (BattleManager.current.battleMode == "defense")
        {
            if (Input.GetKeyDown(twoButtonBindOne) || (MidiJack.MidiMaster.GetKey(midiBind1) > 0.0f & !gotMidiInput1))
            {
                gotInputLastDefense = true;
                if (defenseInputOpen && BattleManager.current.currentDefense.trueOrFalse)
                {
                    //TODO: check for degrees of success (good, nice, perfect!)

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
                    //Debug.Log("Good");
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

            //TODO: so this system doesnt really work, i think a better way of handling it will be to have 

            if (Input.GetKeyDown(twoButtonBindOne) || (MidiJack.MidiMaster.GetKey(midiBind1) > 0.0f && !gotMidiInput1))
            {
                //check and see if theres currently an indicator in lane one that is ready to be hit

                if (BattleManager.current.currentLevelSongInfo.songInfo.indicatorOneInfo.Count <= 0)
                {
                    return;
                }


                if (Mathf.Abs((float)BattleManager.current.currentLevelSongInfo.songInfo.indicatorOneInfo[0] - (float)MusicManager.current.timelineInfo.currentPosition) < tolerance)
                {
                    //time to do a hit
                    //destroy the indicator attacked to the time


                    //TODO: check for degrees of success (good, nice, perfect!)

                    string status = checkHitAccuracy(Mathf.Abs((float)BattleManager.current.currentLevelSongInfo.songInfo.indicatorOneInfo[0] - (float)MusicManager.current.timelineInfo.currentPosition));

                    UIManager.current.SpawnFeedBackText(true, 1, status);

                    //Debug.Log(BattleManager.current.currentLevelSongInfo.songInfo.indicatorDict);
                    Destroy(BattleManager.current.currentLevelSongInfo.songInfo.indicatorDict[BattleManager.current.currentLevelSongInfo.songInfo.indicatorOneInfo[0]].gameObject);
                    //remove it from the dictioanry as well

                    BattleManager.current.currentLevelSongInfo.songInfo.indicatorDict.Remove(BattleManager.current.currentLevelSongInfo.songInfo.indicatorOneInfo[0]);
                    //remove the array entry as well
                    BattleManager.current.currentLevelSongInfo.songInfo.indicatorOneInfo.RemoveAt(0);

                    //Debug.Log("hit!");
                    BattleManager.current.ProcessHit(true);
                }
                else
                {
                    UIManager.current.SpawnFeedBackText(false, 1);

                    if (checkMissWithinPadTolerance(1))
                    {
                        //delete the note and remove the indicator

                        Destroy(BattleManager.current.currentLevelSongInfo.songInfo.indicatorDict[BattleManager.current.currentLevelSongInfo.songInfo.indicatorOneInfo[0]].gameObject);
                        //remove it from the dictioanry as well

                        BattleManager.current.currentLevelSongInfo.songInfo.indicatorDict.Remove(BattleManager.current.currentLevelSongInfo.songInfo.indicatorOneInfo[0]);
                        //remove the array entry as well
                        BattleManager.current.currentLevelSongInfo.songInfo.indicatorOneInfo.RemoveAt(0);
                    }
                }

            }
            if (Input.GetKeyDown(twoButtonBindTwo) || (MidiJack.MidiMaster.GetKey(midiBind2) > 0.0f && !gotMidiInput2))
            {

                //make sure theres like actually any notes left to hit
                if (BattleManager.current.currentLevelSongInfo.songInfo.indicatorTwoInfo.Count <= 0)
                {
                    //for now we just dont penalize you
                    return;
                }


                if (Mathf.Abs((float)BattleManager.current.currentLevelSongInfo.songInfo.indicatorTwoInfo[0] - (float)MusicManager.current.timelineInfo.currentPosition) < tolerance)
                {

                    string status = checkHitAccuracy(Mathf.Abs((float)BattleManager.current.currentLevelSongInfo.songInfo.indicatorTwoInfo[0] - (float)MusicManager.current.timelineInfo.currentPosition));


                    UIManager.current.SpawnFeedBackText(true, 2, status);


                    Destroy(BattleManager.current.currentLevelSongInfo.songInfo.indicatorDict[BattleManager.current.currentLevelSongInfo.songInfo.indicatorTwoInfo[0]].gameObject);
                    //remove it from the dictioanry as well

                    BattleManager.current.currentLevelSongInfo.songInfo.indicatorDict.Remove(BattleManager.current.currentLevelSongInfo.songInfo.indicatorTwoInfo[0]);
                    //remove the array entry as well
                    BattleManager.current.currentLevelSongInfo.songInfo.indicatorTwoInfo.RemoveAt(0);

                    //Debug.Log("hit!");
                    BattleManager.current.ProcessHit(true);
                }
                else
                {
                    //offense mode hit but timings off
                    UIManager.current.SpawnFeedBackText(false, 2);

                    //so now we're also going to check and see if the next note is within another tolerance value which will delete it 
                    //ie: if the note is right about to hit the pad and the user fucks up jsut clean up the note 
                    if (checkMissWithinPadTolerance(2))
                    {
                        //delete the note and remove the indicator

                        Destroy(BattleManager.current.currentLevelSongInfo.songInfo.indicatorDict[BattleManager.current.currentLevelSongInfo.songInfo.indicatorTwoInfo[0]].gameObject);
                        //remove it from the dictioanry as well

                        BattleManager.current.currentLevelSongInfo.songInfo.indicatorDict.Remove(BattleManager.current.currentLevelSongInfo.songInfo.indicatorTwoInfo[0]);
                        //remove the array entry as well
                        BattleManager.current.currentLevelSongInfo.songInfo.indicatorTwoInfo.RemoveAt(0);
                    }
                }
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


    bool checkMissWithinPadTolerance(int padNum)
    {
        if (padNum == 1)
        {
            //check if the difference between the current time and the time of the next note is within the missTolerance value to get auto cleaned up

            if (Mathf.Abs((float)BattleManager.current.currentLevelSongInfo.songInfo.indicatorOneInfo[0] - (float)MusicManager.current.timelineInfo.currentPosition) < missDeleteTolerance)
            {
                return true;
            }
        }
        else if (padNum == 2)
        {
            if (Mathf.Abs((float)BattleManager.current.currentLevelSongInfo.songInfo.indicatorTwoInfo[0] - (float)MusicManager.current.timelineInfo.currentPosition) < missDeleteTolerance)
            {
                return true;
            }
        }


        return false;
    }

    public void OpenDefeneseWindow(bool halfTime)
    {
        if (!halfTime)
        {
            StartCoroutine(openWindowRoutine(MusicManager.current.timelineInfo.currentPosition + ((1 - defenseTolerance) * (60f / 80f) * 1000), true));
            StartCoroutine(openWindowRoutine(MusicManager.current.timelineInfo.currentPosition + ((1 + defenseTolerance) * (60f / 80f) * 1000), false));
        }
        else
        {
            StartCoroutine(openWindowRoutine(MusicManager.current.timelineInfo.currentPosition + (2 * (1 - defenseTolerance) * (60f / 80f) * 1000), true));
            StartCoroutine(openWindowRoutine(MusicManager.current.timelineInfo.currentPosition + (2 * (1 + defenseTolerance) * (60f / 80f) * 1000), false));
        }

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
