using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager current;

    public GameObject indicator;

    public Transform indicatorContainer;

    //100 pixels for every beat
    public int pixelToSeconds = 100;

    private void Awake()
    {
        current = this;
    }
    void Start()
    {

    }

    //so this is just gonna rig up a bunch of indicators pre-compiled
    public void SetupIndicators(SongInfo info)
    {
        //assuming every beat is based on the ratio, plop them down
        for (int i = 0; i < info.indicatorOneInfo.Count; i++)
        {
            Vector3 indicPos = indicatorContainer.transform.position + new Vector3((float)info.indicatorOneInfo[i] / 1000 * pixelToSeconds, 0, 0);
            Instantiate(indicator, indicPos, Quaternion.identity, indicatorContainer.transform);
        }

        for (int i = 0; i < info.indicatorTwoInfo.Count; i++)
        {
            Vector3 indicPos = indicatorContainer.transform.position - new Vector3((float)info.indicatorTwoInfo[i] / 1000 * pixelToSeconds, 0, 0);
            Instantiate(indicator, indicPos, Quaternion.identity, indicatorContainer.transform);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
