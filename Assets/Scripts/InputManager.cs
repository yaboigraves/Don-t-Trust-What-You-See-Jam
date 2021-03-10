using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static InputManager current;
    public float tolerance;
    public KeyCode bind = KeyCode.Space;
    //so going to need to maintain two lists for the indicators 

    public RotateOn cubeTest;

    public string battleMode = "defense";


    //notes for defense mode
    //-queue of true/false to start 
    //-dequeue every beat and present to ui, wait a beat for input, dequeue again and then present etc



    private void Awake()
    {
        current = this;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(bind))
        {
            //check if we hit on the right time
            if (battleMode == "offense")
            {
                if (CheckInput(MusicManager.current.timelineInfo.currentPosition))
                {
                    print("Good");
                    //spawn a good feedback
                    UIManager.current.SpawnFeedBackText();


                }
                else
                {
                    print("Bad");
                }
            }
            else if (battleMode == "defense")
            {

            }

        }

        //only do this if we're in offense mode

        if (battleMode == "offense")
        {
            CheckIndicatorStatus();
        }
        else
        {

        }

    }


    void CheckIndicatorStatus()
    {
        if (BattleManager.current.currentSongInfo.indicatorOneInfo.Count > 0 && BattleManager.current.currentSongInfo.indicatorOneInfo[0] < MusicManager.current.timelineInfo.currentPosition - tolerance)
        {
            Destroy(BattleManager.current.currentSongInfo.indicatorDict[BattleManager.current.currentSongInfo.indicatorOneInfo[0]].gameObject);
            BattleManager.current.currentSongInfo.indicatorOneInfo.RemoveAt(0);

            //delete the indicator too
            Debug.Log("missed a kick");
            cubeTest.rotate(0);
        }

        if (BattleManager.current.currentSongInfo.indicatorTwoInfo.Count > 0 && BattleManager.current.currentSongInfo.indicatorTwoInfo[0] < MusicManager.current.timelineInfo.currentPosition - tolerance)
        {
            Destroy(BattleManager.current.currentSongInfo.indicatorDict[BattleManager.current.currentSongInfo.indicatorTwoInfo[0]].gameObject);
            BattleManager.current.currentSongInfo.indicatorTwoInfo.RemoveAt(0);

            //delete the indicator too
            Debug.Log("missed a snare");
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
