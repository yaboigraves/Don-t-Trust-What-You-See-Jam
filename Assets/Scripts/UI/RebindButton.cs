using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RebindButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    public int keyNum;
    public string prefsStringName;
    public bool isMidi;

    private void Start()
    {
        //buttonText = GetComponentInChildren<TextMeshProUGUI>();

        UpdateButtonText();
    }

    public void UpdateButtonText()
    {
        string buttonTextStr = "";
        if (isMidi)
        {
            int midiKey = PlayerPrefs.GetInt(prefsStringName, 0);
            if (keyNum == 1)
            {
                if (midiKey == 0)
                {
                    buttonTextStr = "Midi Key 1 Unbound";
                }
                else
                {
                    buttonTextStr = "Midi Key 1 : " + midiKey.ToString();
                }
            }
            else if (keyNum == 2)
            {
                if (midiKey == 0)
                {
                    buttonTextStr = "Midi Key 2 Unbound";
                }
                else
                {
                    buttonTextStr = "Midi Key 2 : " + midiKey.ToString();
                }
            }
        }
        else
        {
            string key = PlayerPrefs.GetString(prefsStringName, null);

            if (key == null)
            {
                buttonTextStr = "Keyboard Key " + keyNum.ToString() + " unbound.";
            }
            else
            {
                buttonTextStr = "Keyboard Key " + keyNum.ToString() + " : " + key;
            }
        }

        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = buttonTextStr;
    }


}
