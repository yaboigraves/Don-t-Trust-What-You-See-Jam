using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;



public class MenuManager : MonoBehaviour
{
    public GameObject ControlBindPanel;
    public Button[] keyBindButtons;
    private void Awake()
    {
        SaveStateManager.LoadSaveState();
        //check and see if the game has launched before, and if not ask for midi binds if the user wants to enter them
        CheckIfGameLaunched();
    }


    public string mainSceneName;
    public void StartGame()
    {
        SceneManager.LoadScene(mainSceneName);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void CheckIfGameLaunched()
    {
        if (SaveStateManager.saveState.hasGameLaunched == 0)
        {
            //mark th at we have launched the gamen now (note this doesnt actually save till we call the save function)
            SaveStateManager.saveState.hasGameLaunched = 1;
            SaveStateManager.SaveGame();

            //prompt the user with a ui thing to essentially bind the keys to what they want
            ToggleKeyRebindWindow(true);
        }
    }

    public void ToggleKeyRebindWindow(bool toggle)
    {
        ControlBindPanel.SetActive(toggle);
    }


    public void CaptureMidiBind(int midi)
    {
        //set all the buttons as not interactable 
        ToggleButtons(false);
        StartCoroutine(captureMidiRoutine(midi));

    }

    public void CaptureKeyBind(int key)
    {
        ToggleButtons(false);
        StartCoroutine(captureKeyRoutine(key));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            //resets the save state
            SaveStateManager.ResetSaveState();
        }
    }

    IEnumerator captureMidiRoutine(int midi)
    {
        bool gotMidiInput = false;
        while (gotMidiInput == false)
        {
            //loop through all of the possible midi keys that coule be pressed
            bool gotInput = false;
            for (int i = 23; i < 125; i++)
            {
                if (MidiJack.MidiMaster.GetKey(i) > 0.0f)
                {
                    if (midi == 1)
                    {
                        SaveStateManager.saveState.midiBind1 = i;
                    }
                    else
                    {
                        SaveStateManager.saveState.midiBind2 = i;

                    }

                    Debug.Log("got input of midi key " + i.ToString());
                    SaveStateManager.SaveGame();
                    ToggleButtons(true);
                    gotInput = true;
                    break;

                }
            }

            if (gotInput)
            {
                break;
            }


            yield return null;
        }

        UpdateRebindButtons();

    }

    public void UpdateRebindButtons()
    {
        //update all the ui buttons
        foreach (Button b in keyBindButtons)
        {
            b.gameObject.GetComponentInChildren<RebindButton>().UpdateButtonText();
        }
    }


    IEnumerator captureKeyRoutine(int key)
    {
        bool gotKeyInput = false;
        while (gotKeyInput == false)
        {
            //check and see if a key is pressed
            if (Input.anyKeyDown)
            {
                string inputString = Input.inputString;
                Debug.Log("got input string of " + inputString);
                gotKeyInput = true;

                if (key == 1)
                {
                    SaveStateManager.saveState.keyBind1 = inputString;
                }
                else
                {
                    SaveStateManager.saveState.keyBind2 = inputString;
                }

                SaveStateManager.SaveGame();
                ToggleButtons(true);

                //TODO: set the text of the button

            }
            yield return null;
        }



        UpdateRebindButtons();

    }

    void ToggleButtons(bool toggle)
    {
        foreach (Button b in keyBindButtons)
        {
            b.interactable = toggle;
        }
    }
}
