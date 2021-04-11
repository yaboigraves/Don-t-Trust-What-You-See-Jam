using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBar : MonoBehaviour
{
    //so we just gotta lerp down to 0,0 from our initial position
    // Start is called before the first frame update

    //so we basically just need to lerp down over a period of beats

    public float startBeat;

    public float lerpProgress, lerpEnd, indicatorTime;

    public Vector3 startPos;
    private void Start()
    {
        startBeat = transform.position.y;

        //nBeats * time per beat 

        indicatorTime = startBeat * (60f / MusicManager.current.timelineInfo.currentTempo) * 1000f;
        startPos = transform.position;
    }


    //lerp the bar down over time
    void Update()
    {
        if (MusicManager.current.enabled)
        {
            lerpProgress = MusicManager.current.timelineInfo.currentPosition / indicatorTime;
            transform.position = transform.position = Vector3.Lerp(startPos, Vector3.zero, lerpProgress);

        }
    }
}
