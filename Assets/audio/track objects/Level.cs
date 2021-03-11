using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "BattleLevel", menuName = "Create New Level")]
public class Level : ScriptableObject
{
    public SongInfo songInfo;
    public string midiFileName;
    public float bpm;

    public void buildSongInfo()
    {
        Debug.Log("buildind dat young song info");
        songInfo = SongLoader.loadSongInfoFromMidi(midiFileName, bpm);
    }


}




[CustomEditor(typeof(Level))]
public class BuildLevelMidiButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (Level)target;

        if (GUILayout.Button("Build Midi", GUILayout.Height(40)))
        {
            script.buildSongInfo();
        }

    }
}