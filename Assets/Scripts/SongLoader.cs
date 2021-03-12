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
    public static SongInfo loadSongInfoFromMidi(string midiFileName, float bpm)
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
        // Debug.Log(parser.test());

        Dictionary<string, List<System.Double>> midiParse = parser.parse(Application.dataPath + @"\scripts\midis\" + midiFileName + ".mid", bpm);
        return new SongInfo(midiParse["kick"], midiParse["snare"]);
    }
}


//so this is going to also need to maintain a dictionary mapping indicators to their times
[System.Serializable]
public class SongInfo
{
    public List<double> indicatorOneInfo, indicatorTwoInfo;

    public Dictionary<double, Indicator> indicatorDict = new Dictionary<double, Indicator>();
    public SongInfo(List<double> oneInfo, List<double> twoInfo)
    {
        //floats? I guess?
        indicatorOneInfo = oneInfo;
        indicatorTwoInfo = twoInfo;
        indicatorDict = new Dictionary<double, Indicator>();
    }
}
