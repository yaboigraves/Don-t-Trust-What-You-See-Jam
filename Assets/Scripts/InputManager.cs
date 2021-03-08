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
        //assuming no loop for now

        //check if the current beat is 4 or 1

        //so we need to know what time in the song all the 1's are (past the first bar)
        //seconds per beat * 4, seconds per beat * 8 ....

        //TODO: left off here debug from here
        if (Mathf.Abs(MusicManager.current.timelineInfo.currentPosition - (MusicManager.current.msPerBeat * MusicManager.current.timelineInfo.currentBar)) < tolerance)
        {
            return true;
        }

        return false;
    }


}
