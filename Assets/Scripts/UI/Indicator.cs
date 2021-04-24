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

    public SpriteRenderer sprite;

    //post destination lerp

    //in beats
    public float postLerpLength = 0.5f;

    public Vector3 postLerpDistance = new Vector3(0.5f, 0, 0);

    public float lerpProgress, postLerpProgress;

    public bool isTrueOrFalse;


    private void Start() {
        sprite = GetComponent<SpriteRenderer>();
    }
    public void SetIndicatorTime(float time, Transform dest)
    {
        indicatorTime = time;
        if (indicatorTime == 0)
        {
            indicatorTime = 1;
        }
        startPosition = new Vector3(0, indicatorTime / 1000f, 0);
        transform.position = startPosition;
        this.enabled = true;
        destinationPoint = dest;

    }

    public void SetIndicatorInfo(bool trueOrFalse){
        if(trueOrFalse){
            sprite.color = Color.green;
        }
        else{
            sprite.color = Color.red;
        }

        isTrueOrFalse = trueOrFalse;
    }

    // Update is called once per frame
    void Update()
    {

        if (MusicManager.current.enabled)
        {
            if (lerpProgress < 1)
            {
                lerpProgress = MusicManager.current.timelineInfo.currentPosition / indicatorTime;
                transform.position = Vector3.Lerp(startPosition, destinationPoint.position, lerpProgress);
            }
        }

        //TODO: reimpliment this
        // else if (postLerpProgress < 1)
        // {
        //     postLerpProgress = MusicManager.current.timelineInfo.currentPosition / (indicatorTime + (lerpProgress = MusicManager.current.timelineInfo.currentPosition / (indicatorTime + (postLerpLength * (60f / 80f)))));
        //     transform.position = Vector3.Lerp(destinationPoint.position, destinationPoint.position + postLerpDistance, postLerpProgress);
        // }


    }
}
