using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager current;
    public GameObject indicator;
    public Transform indicatorContainer;
    public Transform indicatorDestination;

    //100 pixels for every beat
    public int pixelToSeconds = 100;
    public GameObject feedbackText;
    public Transform feedbackContainer;

    public TextMeshProUGUI defensePromptText;


    public SpriteRenderer defenseCenterPointIndicator;
    private void Awake()
    {
        current = this;
    }
    void Start()
    {

    }

    //so this is just gonna rig up a bunch of indicators pre-compiled
    public void SetupIndicators()
    {

        SongInfo info = BattleManager.current.currentSongInfo;
        //so when we spawn in the songinfo stuff, we should map the times to the indicators to reference them
        //assuming every beat is based on the ratio, plop them down
        for (int i = 0; i < info.indicatorOneInfo.Count; i++)
        {
            Vector3 indicPos = indicatorContainer.transform.position + new Vector3((float)info.indicatorOneInfo[i] / 1000 * pixelToSeconds, 0, 0);
            Indicator indic = Instantiate(indicator, indicPos, Quaternion.identity, indicatorContainer.transform).GetComponent<Indicator>();
            indic.SetIndicatorTime((float)info.indicatorOneInfo[i], indicatorDestination);

            //map the indicator at this time in the song info dictionary
            info.indicatorDict[info.indicatorOneInfo[i]] = indic;
        }

        for (int i = 0; i < info.indicatorTwoInfo.Count; i++)
        {
            Vector3 indicPos = indicatorContainer.transform.position - new Vector3((float)info.indicatorTwoInfo[i] / 1000 * pixelToSeconds, 0, 0);
            Indicator indic = Instantiate(indicator, indicPos, Quaternion.identity, indicatorContainer.transform).GetComponent<Indicator>();
            indic.SetIndicatorTime((float)info.indicatorTwoInfo[i], indicatorDestination);
            info.indicatorDict[info.indicatorTwoInfo[i]] = indic;
        }
    }

    public void SpawnFeedBackText()
    {
        Instantiate(feedbackText, feedbackContainer.transform.position, Quaternion.identity, feedbackContainer);
    }

    public void SpawnDefensePrompt(bool b)
    {
        defensePromptText.text = b.ToString();
    }

    public void ToggleDefenseInputUi()
    {
        //turn on the white bar or turn it off
        defenseCenterPointIndicator.enabled = !defenseCenterPointIndicator.enabled;
    }






}
