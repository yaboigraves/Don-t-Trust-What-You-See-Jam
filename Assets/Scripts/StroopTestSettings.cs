using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StroopTestSettings", menuName = "Create New Stroop Test Settings")]
public class StroopTestSettings : ScriptableObject
{
    public string sceneName;

    public DefensePrompt[] tests;

}

[System.Serializable]
public class DefensePrompt
{
    public bool trueOrFalse;
    public bool colorModel = false;
    public string text;
    public Sprite sprite;
    public GameObject model;
    public Color textColor = Color.white, assetColor = Color.white;
}