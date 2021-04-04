using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveStateManager
{
    //so the only real thing that this needs to do is store what levels have been completed, keybinds, and
    //whether the game has been launched before

    public static SaveState saveState;

    //so this is run when the main scene runs when the game first opens to just load all the settings
    public static void LoadSaveState()
    {
        if (PlayerPrefs.HasKey("hasGameLaunched"))
        {
            saveState = new SaveState(PlayerPrefs.GetInt("hasGameLaunched"), PlayerPrefs.GetInt("completedLevels"), PlayerPrefs.GetInt("midiBind1"), PlayerPrefs.GetInt("midiBind2"), PlayerPrefs.GetString("keyBind1"), PlayerPrefs.GetString("keyBind2"));
        }
        else
        {
            ResetSaveState();
        }
    }

    public static void SaveGame()
    {
        //basically just set all the player pref from the savestate object that it currently has
        PlayerPrefs.SetInt("hasGameLaunched", saveState.hasGameLaunched);
        PlayerPrefs.SetInt("completedLevels", saveState.completedLevels);
        PlayerPrefs.SetInt("midiBind1", saveState.midiBind1);
        PlayerPrefs.SetInt("midiBind2", saveState.midiBind2);
        PlayerPrefs.SetString("keyBind1", saveState.keyBind1.ToString());
        PlayerPrefs.SetString("keyBind2", saveState.keyBind2.ToString());

        Debug.Log("Game Saved!");
    }

    public static void ResetSaveState()
    {
        PlayerPrefs.DeleteAll();
        saveState = new SaveState(0, 0, 0, 0, "a", "d");
        SaveGame();
    }
}


public class SaveState
{
    //0 for false, 1 for true
    public int hasGameLaunched;
    public int completedLevels;
    public int midiBind1, midiBind2;
    public string keyBind1, keyBind2;

    public SaveState(int hasGameLaunched, int completedLevels, int midiBind1, int midiBind2, string keyBind1, string keyBind2)
    {
        this.hasGameLaunched = hasGameLaunched;
        this.completedLevels = completedLevels;
        this.midiBind1 = midiBind1;
        this.midiBind2 = midiBind2;
        this.keyBind1 = keyBind1;
        this.keyBind2 = keyBind2;
    }
}