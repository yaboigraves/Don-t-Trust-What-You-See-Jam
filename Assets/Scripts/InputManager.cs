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

    public SongInfo currentInfo;

    public RotateOn cubeTest;




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

            if (CheckInput(MusicManager.current.timelineInfo.currentPosition))
            {
                print("Good");
            }
            else
            {
                print("Bad");
            }
        }

        CheckIndicatorStatus();






    }


    void CheckIndicatorStatus()
    {

        if (currentInfo.indicatorOneInfo.Count > 0 && currentInfo.indicatorOneInfo[0] < MusicManager.current.timelineInfo.currentPosition - tolerance)
        {
            currentInfo.indicatorOneInfo.RemoveAt(0);
            Debug.Log("missed a kick");
            cubeTest.rotate(0);
        }

        if (currentInfo.indicatorTwoInfo.Count > 0 && currentInfo.indicatorTwoInfo[0] < MusicManager.current.timelineInfo.currentPosition - tolerance)
        {
            currentInfo.indicatorTwoInfo.RemoveAt(0);
            Debug.Log("missed a snare");
            cubeTest.rotate(1);
        }

        //if theres no indicators left turn off the input
        if (currentInfo.indicatorOneInfo.Count < 1 && currentInfo.indicatorTwoInfo.Count < 1)
        {
            this.enabled = false;
        }
    }

    public bool CheckInput(int time)
    {


        //TODO: this is extremely ugly and messy refactor this later, at least its evil is contained for now

        double nextIndicatorTime = 0;

        double indicatorOnePeek = -1, indicatorTwoPeek = -1;


        if (currentInfo.indicatorOneInfo.Count > 0)
        {
            indicatorOnePeek = currentInfo.indicatorOneInfo[0];
        }
        if (currentInfo.indicatorTwoInfo.Count > 0)
        {
            indicatorTwoPeek = currentInfo.indicatorTwoInfo[0];
        }


        if (indicatorOnePeek != -1 && indicatorTwoPeek == -1)
        {
            //theres still at least one indicator one left
            nextIndicatorTime = indicatorOnePeek;
        }
        else if (indicatorTwoPeek != -1 && indicatorOnePeek == -1)
        {
            //theres still at least one indicator two left
            nextIndicatorTime = indicatorTwoPeek;
        }
        else
        {

            if (currentInfo.indicatorOneInfo[0] < currentInfo.indicatorTwoInfo[0])
            {
                //indicator 1 is the next one
                nextIndicatorTime = currentInfo.indicatorOneInfo[0];
                // currentInfo.indicatorOneInfo.RemoveAt(0);

            }
            //account for floating point fuckery (ignore the lazy casts lol)
            else if (Mathf.Abs((float)currentInfo.indicatorOneInfo[0] - (float)currentInfo.indicatorTwoInfo[0]) < 0.01f)
            {
                //same time, dequeue both
                nextIndicatorTime = currentInfo.indicatorOneInfo[0];
                // currentInfo.indicatorOneInfo.RemoveAt(0);
                // currentInfo.indicatorTwoInfo.RemoveAt(0);
            }
            else if (currentInfo.indicatorTwoInfo.Count > 1 && currentInfo.indicatorOneInfo[0] > currentInfo.indicatorTwoInfo[0])
            {
                //indicator 2 is the next one
                nextIndicatorTime = currentInfo.indicatorTwoInfo[0];
                // currentInfo.indicatorTwoInfo.RemoveAt(0);
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
            Debug.Log("got it with a difference of " + Mathf.Abs(time - (float)nextIndicatorTime));
            return true;
        }
        else
        {
            Debug.Log("missed with a difference of " + Mathf.Abs(time - (float)nextIndicatorTime));
            return false;
        }

    }
}
