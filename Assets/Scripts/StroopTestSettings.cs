using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StroopTestSettings", menuName = "Create New Stroop Test Settings")]
public class StroopTestSettings : ScriptableObject
{
    public string sceneName;

    public DefensePrompt[] defensePrompts;
    public OffensePrompt[] offensePrompts;
}


[System.Serializable]
public class DefensePrompt
{
    public string promptLabel = "";
    public bool trueOrFalse;
    public bool colorModel = false;
    public string text;
    public Sprite sprite;
    public GameObject model;
    public Color textColor = Color.white, assetColor = Color.white;

    //any prompts that can only be 
    public int levelUnlocked = 1;
}

//probably need an offense prompt to load as well


//so these arent inherently true or false theyre just assets, they can be combined with and/or statements as well
[System.Serializable]
public class OffensePrompt
{
    public string promptLabel = "";
    public bool colorModel = false;
    public string text;
    public Sprite sprite;
    public GameObject model;
    public Color textColor = Color.white, assetColor = Color.white;

    //any prompts that can only be 
    public int levelUnlocked = 1;
}
