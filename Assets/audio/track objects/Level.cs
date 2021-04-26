using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BattleLevel", menuName = "Create New Level")]

[System.Serializable]
public class Level : ScriptableObject
{
    public SongInfo songInfo;
    public string midiFileName;
    public float bpm;
    //in beats
    public int defensePhaseLength = 16;
    public int offensePhaseLength = 16;
    public int songLengthInBeats = 64;
    public string fmodSongName;

    //color,math,direction
    public string stroopTestType;

    public void buildSongInfo()
    {
        Debug.Log("buildind dat young song info");
        songInfo = SongLoader.loadSongInfoFromMidi(midiFileName, bpm);
        songInfo.indicatorDict = new Dictionary<double, Indicator>();

        float beatsPerSecond = 60f / bpm;

        //old build for two button 
        // for (int i = 0; i < songInfo.indicatorOneInfo.Count; i++)
        // {
        //     songInfo.indicatorOneInfo[i] *= (beatsPerSecond) * 1000;
        // }
        // // Debug.Log("Snares");
        // for (int i = 0; i < songInfo.indicatorTwoInfo.Count; i++)
        // {
        //     songInfo.indicatorTwoInfo[i] *= (beatsPerSecond) * 1000;
        // }

        //one button new version

        for (int i = 0; i < songInfo.mergedIndicatorInfo.Count; i++)
        {
            songInfo.mergedIndicatorInfo[i] *= (beatsPerSecond) * 1000;
        }


        //UnityEditor.EditorUtility.SetDirty(this);
    }


}



