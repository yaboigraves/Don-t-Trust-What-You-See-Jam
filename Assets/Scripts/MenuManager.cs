using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class MenuManager : MonoBehaviour
{
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

            //prompt the user with a ui thing to essentially bind the keys to what they want

        }
    }


    public void CaptureMidiBind(int midi)
    {
        //set all the buttons as not interactable 
        ToggleButtons(false);

        if (midi == 1)
        {

        }
        else
        {

        }
    }

    public void CaptureKeyBind(int key)
    {
        //set all the buttons as not interactable 
        ToggleButtons(false);

        if (key == 1)
        {
            StartCoroutine(captureKeyRoutine());
        }
        else
        {
            StartCoroutine(captureKeyRoutine());
        }
    }



    IEnumerator captureKeyRoutine()
    {
        bool gotKeyInput = false;
        while (gotKeyInput == false)
        {
            //check and see if a key is pressed
            if (Input.anyKeyDown)
            {
                //TODO: 
                //find the key being pressed
                string inputString = Input.inputString;
                Debug.Log("got input string of " + inputString);
                gotKeyInput = true;
                ToggleButtons(true);
            }

            yield return null;
        }
    }

    void ToggleButtons(bool toggle)
    {
        foreach (Button b in keyBindButtons)
        {
            b.interactable = toggle;
        }
    }
}
