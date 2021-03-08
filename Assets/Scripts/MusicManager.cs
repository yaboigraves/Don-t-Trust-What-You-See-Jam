using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using FMODUnity;
using System;

//TESTING GOALS 
//1. get a track to loop 2 times before randomly moving to another track within the timeline



public class MusicManager : MonoBehaviour
{
    public static MusicManager current;

    [SerializeField]
    [EventRef]
    private string music;

    [StructLayout(LayoutKind.Sequential)]
    public class TimelineInfo
    {
        public int currentBeat = 0, currentBar = 0, currentPosition = 0;
        public float currentTempo = 0, songLength = 0;
        public FMOD.StringWrapper lastMarker = new FMOD.StringWrapper();
    }

    //every beat we store the time of the last beat hit in the audiosettings dsp time
    public int loopStartTime;

    public TimelineInfo timelineInfo = null;

    private GCHandle timelineHandle;

    private FMOD.Studio.EVENT_CALLBACK beatCallback;

    private FMOD.Studio.EventDescription descriptionCallback;

    public FMOD.Studio.EventInstance musicPlayEvent;

    // public FMOD.Studio.PARAMETER_DESCRIPTION testParam;


    //my variables after here

    public int maxBeatsPerLoop = 8, beatsPerLoop = 0;

    public int nextOneTime;


    public int msPerBeat;

    private void Awake()
    {
        current = this;

        musicPlayEvent = RuntimeManager.CreateInstance(music);

        //ok so using the current event we can just set parameters based on trhe transitions

        //musicPlayEvent.setParameterByName("testParam", 1.0f);


        musicPlayEvent.start();
    }

    private void Start()
    {
        timelineInfo = new TimelineInfo();
        beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);

        timelineHandle = GCHandle.Alloc(timelineInfo, GCHandleType.Pinned);
        musicPlayEvent.setUserData(GCHandle.ToIntPtr(timelineHandle));
        musicPlayEvent.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);

        musicPlayEvent.getDescription(out descriptionCallback);
        descriptionCallback.getLength(out int length);

        timelineInfo.songLength = length;




        //testing the static loader
        SongLoader.loadSongInfoFromMidi("ass");

    }

    private void Update()
    {
        musicPlayEvent.getTimelinePosition(out timelineInfo.currentPosition);
        msPerBeat = (int)(timelineInfo.currentTempo / 60 * 1000);
    }


    public void beatTrigger()
    {
        //do stuff every beat
        if (timelineInfo.currentBeat == 2)
        {
            //recalculate the next 1

            //so the next 1 is the current time + (4 * seconds per beat)
            nextOneTime = timelineInfo.currentPosition + (int)(3 * (60 / timelineInfo.currentTempo) * 1000);
        }


        beatsPerLoop++;
        if (beatsPerLoop >= maxBeatsPerLoop)
        {

            musicPlayEvent.setParameterByName("nextTrack", UnityEngine.Random.Range(1, 3));
            beatsPerLoop = 0;
        }
    }



    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

        IntPtr timelineInfoPtr;
        FMOD.RESULT result = instance.getUserData(out timelineInfoPtr);

        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("Timeline Callback error: " + result);
        }
        else if (timelineInfoPtr != IntPtr.Zero) //System(IntPtr)
        {
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;



            switch (type)
            {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                        timelineInfo.currentBeat = parameter.beat;
                        timelineInfo.currentBar = parameter.bar;
                        timelineInfo.currentTempo = parameter.tempo;

                        //check to see if its transition time
                        current.beatTrigger();

                    }
                    break;
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
                        timelineInfo.lastMarker = parameter.name;
                    }
                    break;
            }
        }
        return FMOD.RESULT.OK;
    }

    void OnGUI()
    {
        GUILayout.Box(String.Format("Current Bar = {0}, CurrentBeat = {1}, Current Position = {2}, next one time = {3}",
        timelineInfo.currentBar, timelineInfo.currentBeat, timelineInfo.currentPosition, nextOneTime));
    }

    void OnDestroy()
    {
        musicPlayEvent.setUserData(IntPtr.Zero);
        musicPlayEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicPlayEvent.release();
        timelineHandle.Free();
    }

}
