using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    //ass
    public string mainSceneName;
    public void StartGame()
    {
        SceneManager.LoadScene(mainSceneName);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
