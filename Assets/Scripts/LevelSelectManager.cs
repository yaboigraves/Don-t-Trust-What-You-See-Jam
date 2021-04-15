using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;



public class LevelSelectManager : MonoBehaviour
{
    //so we need a function so when you hit a ui button you move left or right through the circle
    public Transform levelsContainer;
    public bool rotating;
    Quaternion startRotation, endRotation;
    public float rotateTime = 0, maxRotateTime = 2;

    //ui stuff
    public TextMeshProUGUI levelTitleText;
    public Button loadButton;
    public string[] levelNames;

    //this is bad maybe do this dif later
    public Level[] levelObjects;
    int levelIndex = 0;



    private void Start()
    {
        levelTitleText.text = levelNames[levelIndex];
    }

    private void Update()
    {

        if (rotating)
        {
            rotateTime += Time.deltaTime;
            levelsContainer.transform.rotation = Quaternion.Lerp(startRotation, endRotation, rotateTime / maxRotateTime);

            if (rotateTime >= maxRotateTime)
            {
                rotating = false;
                rotateTime = 0;
                levelsContainer.transform.rotation = endRotation;
                startRotation = levelsContainer.transform.rotation;

                //update the ui
                UpdateUI();
            }
        }
    }

    void UpdateUI()
    {
        levelTitleText.text = levelNames[levelIndex];

        //turn off the level text


        if (SaveStateManager.saveState.completedLevels >= levelIndex)
        {
            levelTitleText.enabled = true;
            loadButton.interactable = true;
        }

    }


    public void MoveRight()
    {
        if (rotating)
        {
            return;
        }

        rotating = true;
        startRotation = levelsContainer.transform.rotation;
        endRotation = Quaternion.Euler(levelsContainer.transform.rotation.eulerAngles + Quaternion.Euler(0, 90, 0).eulerAngles);

        levelIndex++;
        if (levelIndex >= levelNames.Length)
        {
            levelIndex = 0;
        }

        //turn off the level text
        levelTitleText.enabled = false;

        //check and see if we can access the level

        loadButton.interactable = false;


    }

    public void MoveLeft()
    {
        if (rotating)
        {
            return;
        }
        rotating = true;
        startRotation = levelsContainer.transform.rotation;
        endRotation = Quaternion.Euler(levelsContainer.transform.rotation.eulerAngles - Quaternion.Euler(0, 90, 0).eulerAngles);

        levelIndex--;
        if (levelIndex < 0)
        {
            levelIndex = levelNames.Length - 1;
        }

        //turn off the level text
        levelTitleText.enabled = false;



        loadButton.interactable = false;


    }

    public void LoadLevel()
    {

        //check if we can even  load the level based on the players save progress 
        if (SaveStateManager.saveState.completedLevels >= levelIndex)
        {
            Debug.Log("loading " + levelNames[levelIndex]);
            //later throw together a loading screen here
            StartCoroutine(loadBattleScenesAsync(levelNames[levelIndex]));
        }
        else
        {
            Debug.Log("cant Load that level yet son");
        }



    }

    IEnumerator loadBattleScenesAsync(string sceneName)
    {
        AsyncOperation asyncLoad1 = SceneManager.LoadSceneAsync("BattleScene", LoadSceneMode.Additive);
        AsyncOperation asyncLoad2 = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!asyncLoad1.isDone || !asyncLoad2.isDone)
        {
            yield return null;
        }
        BattleManager.current.debugMode = false;

        //set the track in the battle manager
        BattleManager.current.currentLevelSongInfo = Instantiate(levelObjects[levelIndex]);

        //wait a frame
        yield return null;

        BattleManager.current.LoadLevelInfo(sceneName);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        SceneManager.UnloadSceneAsync("LevelSelect");
    }

    public void GoBackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
