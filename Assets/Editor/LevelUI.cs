using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Level))]
public class LevelUI : Editor
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