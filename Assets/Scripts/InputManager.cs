using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static InputManager current;
    public float tolerance;
    public KeyCode oneButtonBind = KeyCode.Space;
    //so going to need to maintain two lists for the indicators 

    public KeyCode twoButtonBindOne = KeyCode.A, twoButtonBindTwo = KeyCode.D;
    public RotateOn cubeTest;

    public bool defenseInputOpen = false;

    public float defenseTolerance = 0.2f;

    //one or two
    public string inputMode = "one";



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
    }

    public void InitInputs()
    {
        if (inputMode == "one")
        {
            UIManager.current.ToggleOneButtonDefenseInput();
        }
        else if (inputMode == "two")
        {
            UIManager.current.ToggleTwoButtonDefenseInput();
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (inputMode == "one")
        {
            OneButtonInput();
        }
        else if (inputMode == "two")
        {
            TwoButtonInput();
        }


        //only do this if we're in offense mode

        if (BattleManager.current.battleMode == "offense")
        {
            CheckIndicatorStatus();
        }
        else
        {

        }
    }

    public void OneButtonInput()
    {
        if (Input.GetKeyDown(oneButtonBind))
        {
            //check if we hit on the right time
            if (BattleManager.current.battleMode == "offense")
            {
                if (CheckInput(MusicManager.current.timelineInfo.currentPosition))
                {
                    print("Good");
                    //spawn a good feedback
                    UIManager.current.SpawnFeedBackText();
                    BattleManager.current.ProcessHit(true);


                }
                else
                {
                    BattleManager.current.ProcessHit(false);
                    print("Bad");
                }
            }
            else if (BattleManager.current.battleMode == "defense")
            {
                //check if we're currently in a window to accept an input
                if (defenseInputOpen && BattleManager.current.currentDefense)
                {
                    Debug.Log("Good");
                    UIManager.current.SpawnFeedBackText();
                    defenseInputOpen = false;

                    BattleManager.current.ProcessHit(true);
                }
                else
                {
                    Debug.Log("Bad");
                    BattleManager.current.ProcessHit(false);
                    defenseInputOpen = false;
                }
            }
        }
    }

    public void TwoButtonInput()
    {
        if (BattleManager.current.battleMode == "defense")
        {
            //so assuming that input 1 is true and input 2 is false for now

            if (Input.GetKeyDown(twoButtonBindOne))
            {
                if (defenseInputOpen && BattleManager.current.currentDefense)
                {
                    Debug.Log("Good");
                    UIManager.current.SpawnFeedBackText();
                    defenseInputOpen = false;
                    BattleManager.current.ProcessHit(true);
                }
                else
                {
                    BattleManager.current.ProcessHit(false);
                }
            }
            else if (Input.GetKeyDown(twoButtonBindTwo))
            {
                if (defenseInputOpen && !BattleManager.current.currentDefense)
                {
                    Debug.Log("Good");
                    UIManager.current.SpawnFeedBackText();
                    defenseInputOpen = false;
                }
                else
                {
                    BattleManager.current.ProcessHit(false);
                }
            }
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

        //so we're also in here going to need to have the ui manager indicate that its opened
        //just flash it green for now
        UIManager.current.ToggleDefenseInputUi();
    }

    void CheckIndicatorStatus()
    {
        if (BattleManager.current.currentSongInfo.indicatorOneInfo.Count > 0 && BattleManager.current.currentSongInfo.indicatorOneInfo[0] < MusicManager.current.timelineInfo.currentPosition - tolerance)
        {
            Destroy(BattleManager.current.currentSongInfo.indicatorDict[BattleManager.current.currentSongInfo.indicatorOneInfo[0]].gameObject);
            BattleManager.current.currentSongInfo.indicatorOneInfo.RemoveAt(0);

            //delete the indicator too
            Debug.Log("missed a kick");
            BattleManager.current.ProcessHit(false);
            cubeTest.rotate(0);
        }

        if (BattleManager.current.currentSongInfo.indicatorTwoInfo.Count > 0 && BattleManager.current.currentSongInfo.indicatorTwoInfo[0] < MusicManager.current.timelineInfo.currentPosition - tolerance)
        {
            Destroy(BattleManager.current.currentSongInfo.indicatorDict[BattleManager.current.currentSongInfo.indicatorTwoInfo[0]].gameObject);
            BattleManager.current.currentSongInfo.indicatorTwoInfo.RemoveAt(0);

            //delete the indicator too
            Debug.Log("missed a snare");
            BattleManager.current.ProcessHit(false);
            cubeTest.rotate(1);
        }

        //if theres no indicators left turn off the input
        if (BattleManager.current.currentSongInfo.indicatorOneInfo.Count < 1 && BattleManager.current.currentSongInfo.indicatorTwoInfo.Count < 1)
        {
            this.enabled = false;
        }
    }

    public bool CheckInput(int time)
    {


        //TODO: this is extremely ugly and messy refactor this later, at least its evil is contained for now

        double nextIndicatorTime = 0;

        double indicatorOnePeek = -1, indicatorTwoPeek = -1;

        int indicatorNext = 0;


        if (BattleManager.current.currentSongInfo.indicatorOneInfo.Count > 0)
        {
            indicatorOnePeek = BattleManager.current.currentSongInfo.indicatorOneInfo[0];
        }
        if (BattleManager.current.currentSongInfo.indicatorTwoInfo.Count > 0)
        {
            indicatorTwoPeek = BattleManager.current.currentSongInfo.indicatorTwoInfo[0];
        }


        if (indicatorOnePeek != -1 && indicatorTwoPeek == -1)
        {
            //theres still at least one indicator one left
            nextIndicatorTime = indicatorOnePeek;
            indicatorNext = 1;
        }
        else if (indicatorTwoPeek != -1 && indicatorOnePeek == -1)
        {
            //theres still at least one indicator two left
            nextIndicatorTime = indicatorTwoPeek;
            indicatorNext = 2;
        }
        else
        {
            if (BattleManager.current.currentSongInfo.indicatorOneInfo[0] < BattleManager.current.currentSongInfo.indicatorTwoInfo[0])
            {
                //indicator 1 is the next one
                nextIndicatorTime = BattleManager.current.currentSongInfo.indicatorOneInfo[0];
                indicatorNext = 1;

            }
            //account for floating point fuckery (ignore the lazy casts lol)
            else if (Mathf.Abs((float)BattleManager.current.currentSongInfo.indicatorOneInfo[0] - (float)BattleManager.current.currentSongInfo.indicatorTwoInfo[0]) < 0.01f)
            {
                //same time, dequeue both
                nextIndicatorTime = BattleManager.current.currentSongInfo.indicatorOneInfo[0];
                indicatorNext = 1;

            }
            else if (BattleManager.current.currentSongInfo.indicatorTwoInfo.Count > 1 && BattleManager.current.currentSongInfo.indicatorOneInfo[0] > BattleManager.current.currentSongInfo.indicatorTwoInfo[0])
            {
                //indicator 2 is the next one
                nextIndicatorTime = BattleManager.current.currentSongInfo.indicatorTwoInfo[0];
                indicatorNext = 2;

            }
            else
            {
                Debug.LogWarning("SOMETHING IS FUCKED UP  :)");
            }
        }

        //so now that we know the next indicator lets see if you hit it on time or you suck
        //again lazy casts
        if (Mathf.Abs(time - (float)nextIndicatorTime) < tolerance)
        {
            //Debug.Log("got it with a difference of " + Mathf.Abs(time - (float)nextIndicatorTime));

            //delete the indicator object
            Destroy(BattleManager.current.currentSongInfo.indicatorDict[nextIndicatorTime].gameObject);

            if (indicatorNext == 1)
            {
                BattleManager.current.currentSongInfo.indicatorOneInfo.RemoveAt(0);
            }
            else if (indicatorNext == 2)
            {
                BattleManager.current.currentSongInfo.indicatorTwoInfo.RemoveAt(0);
            }

            return true;
        }
        else
        {
            //Debug.Log("missed with a difference of " + Mathf.Abs(time - (float)nextIndicatorTime));
            return false;
        }

    }



}
