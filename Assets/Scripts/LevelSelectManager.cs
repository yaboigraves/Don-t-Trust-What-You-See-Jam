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
        levelTitleText.enabled = true;
        loadButton.interactable = true;
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
        //look at what name is currently selected and load that level

        //TODO: so this is going to need a scene manager tool that can dynamically load in scenes that just have the assets in them
        //then we can link up the manager assets with all the stuff it needs to manage in the actual scene
        //can basically do this by just loading everyhing on top of this scene additively and culling with loading screens

        Debug.Log("loading " + levelNames[levelIndex]);

        //so for now we're going to load in the battle scene and then load in the environment scene on top of that, then tie them toegther 
        // //make sure to de load this scene as well after those load
        // SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);

        // //so then we need to also load on top the environment scene
        // //once both of those are loaded deload this scene
        // SceneManager.LoadScene("JungleCube", LoadSceneMode.Additive);


        //later throw together a loading screen here
        StartCoroutine(loadBattleScenesAsync(levelNames[levelIndex]));



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

        BattleManager.current.LoadLevelInfo();
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
