using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{

    //TODO: so indicators need to go a little bit past the center point
    //after the first lerp is finished we can add a second lerp that lerps over a beat duration (as to be reflexive with speed)


    public float indicatorTime;
    public Transform destinationPoint;
    public Vector3 startPosition;

    //post destination lerp

    //in beats
    public float postLerpLength = 0.5f;

    public Vector3 postLerpDistance = new Vector3(0.5f, 0, 0);

    public float lerpProgress, postLerpProgress;

    void Start()
    {

    }
    private void Awake()
    {

    }

    public void SetIndicatorTime(float time, Transform dest)
    {
        indicatorTime = time;
        if (indicatorTime == 0)
        {
            indicatorTime = 1;
        }
        startPosition = transform.position;
        this.enabled = true;
        destinationPoint = dest;

    }

    // Update is called once per frame
    void Update()
    {


        if (lerpProgress < 1)
        {
            lerpProgress = MusicManager.current.timelineInfo.currentPosition / indicatorTime;
            transform.position = Vector3.Lerp(startPosition, destinationPoint.position, lerpProgress);
        }
        //TODO: reimpliment this
        // else if (postLerpProgress < 1)
        // {
        //     postLerpProgress = MusicManager.current.timelineInfo.currentPosition / (indicatorTime + (lerpProgress = MusicManager.current.timelineInfo.currentPosition / (indicatorTime + (postLerpLength * (60f / 80f)))));
        //     transform.position = Vector3.Lerp(destinationPoint.position, destinationPoint.position + postLerpDistance, postLerpProgress);
        // }


    }
}
