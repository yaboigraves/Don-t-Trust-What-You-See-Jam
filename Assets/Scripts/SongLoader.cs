using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using IronPython.Hosting;


/*
    so this is going to read the midi for each loop we create to extract the indicators timing
    all we really need is 2 lanes for now
    basic idea for now : just program in manually that we hit on the 1


*/

public static class SongLoader
{
    public static SongInfo loadSongInfoFromMidi(string midiFileName)
    {
        var engine = Python.CreateEngine();

        ICollection<string> searchPaths = engine.GetSearchPaths();

        //Path to the folder of greeter.py
        searchPaths.Add(Application.dataPath);
        //Path to the Python standard library
        searchPaths.Add(Application.dataPath + @"\Plugins\Lib\");
        engine.SetSearchPaths(searchPaths);

        dynamic py = engine.ExecuteFile(Application.dataPath + @"\scripts\midiParser.py");
        dynamic parser = py.MidiParser();
        Debug.Log(parser.test());




        return null;
    }



}

public class SongInfo
{
    public List<float> indicatorOneInfo, indicatorTwoInfo;
    public SongInfo(List<float> oneInfo, List<float> twoInfo)
    {
        //floats? I guess?
        indicatorOneInfo = oneInfo;
        indicatorTwoInfo = twoInfo;
    }



}
