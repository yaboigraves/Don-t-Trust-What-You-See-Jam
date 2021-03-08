using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Start is called before the first frame update
    public float tolerance;
    public KeyCode bind = KeyCode.Space;
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
    }


    //TODO: get this to work by having us hit specifically on the 1 of the loop

    //so if we know what beat something is supposed to run on and we know the time that beat occurs, we can do some 
    //math to guestimate it i guess?
    public bool CheckInput(int time)
    {

        //so we need to check if we're near a 1

        //the way to do this is use the current bar * 4 * seconds per beat
        //store this in the timeline info for now

        //check and see if the difference between the current time and the next one is less than tolerance

        if (Mathf.Abs(MusicManager.current.timelineInfo.currentPosition - MusicManager.current.nextOneTime) < tolerance)
        {
            return true;
        }


        return false;
    }


}
