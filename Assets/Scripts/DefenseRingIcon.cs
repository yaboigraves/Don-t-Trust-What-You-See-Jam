using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefenseRingIcon : MonoBehaviour
{
    RectTransform rectTransform;
    public Vector2 startSize, targetSize;
    public float spawnTime, lerpEndTime;

    private void Awake()
    {

        rectTransform = GetComponent<RectTransform>();
        startSize = new Vector2(rectTransform.rect.width, rectTransform.rect.height);

        //mark the currentposition when we spawn the ring in
        //measure out the amount of time for one beat and then lerp down to the target scale 
        spawnTime = MusicManager.current.timelineInfo.currentPosition;
        lerpEndTime = MusicManager.current.timelineInfo.currentPosition + (60f / MusicManager.current.timelineInfo.currentTempo) * 1000f;

    }

    public void makeHalfTime()
    {
        lerpEndTime = MusicManager.current.timelineInfo.currentPosition + (2 * (60f / MusicManager.current.timelineInfo.currentTempo) * 1000f);
    }

    private void Update()
    {
        float lerpProgress = (MusicManager.current.timelineInfo.currentPosition - spawnTime) / (lerpEndTime - spawnTime);
        rectTransform.sizeDelta = Vector2.Lerp(startSize, targetSize, lerpProgress);


        if (lerpProgress > 1)
        {
            Destroy(this.gameObject);
        }
    }

}
