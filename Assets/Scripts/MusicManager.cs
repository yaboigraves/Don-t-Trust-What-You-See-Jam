using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using FMODUnity;
using System;


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
        this.enabled = false;
        //musicPlayEvent = RuntimeManager.CreateInstance(music);
    }

    private void Start()
    {
        // timelineInfo = new TimelineInfo();
        // beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);

        // timelineHandle = GCHandle.Alloc(timelineInfo, GCHandleType.Pinned);
        // musicPlayEvent.setUserData(GCHandle.ToIntPtr(timelineHandle));
        // musicPlayEvent.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);

        // musicPlayEvent.getDescription(out descriptionCallback);
        // descriptionCallback.getLength(out int length);

        // timelineInfo.songLength = length;

    }

    public void LoadSong(string fmodSongName)
    {
        music = fmodSongName;
        musicPlayEvent = RuntimeManager.CreateInstance(music);

        timelineInfo = new TimelineInfo();
        beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);

        timelineHandle = GCHandle.Alloc(timelineInfo, GCHandleType.Pinned);
        musicPlayEvent.setUserData(GCHandle.ToIntPtr(timelineHandle));
        musicPlayEvent.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);

        musicPlayEvent.getDescription(out descriptionCallback);
        descriptionCallback.getLength(out int length);

        timelineInfo.songLength = length;

        this.enabled = true;
    }

    public void StartBattle()
    {
        Debug.Log("starting battle");
        musicPlayEvent.start();
    }

    public void EndBattle()
    {
        musicPlayEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private void Update()
    {
        musicPlayEvent.getTimelinePosition(out timelineInfo.currentPosition);
        msPerBeat = (int)(timelineInfo.currentTempo / 60 * 1000);
    }

    public SongInfo getMidiInfo(string songName)
    {

        //so we need to just pass in the bpm info of the current loaded track, need to make sure everything 
        //is loaded in fmod
        return SongLoader.loadSongInfoFromMidi(songName, 80);
    }



    public void beatTrigger()
    {
        BattleManager.current.CheckPhase();

        if (timelineInfo.currentBeat == 1)
        {
            CameraManager.current.SwitchCamera();

            AnimationManager.current.BeatSwitchAnimation();
        }
        if (BattleManager.current.battleMode == "defense")
        {
            //check whether we're waiting for an input or not
            if (timelineInfo.currentBeat == 1 || timelineInfo.currentBeat == 3)
            {
                //tell the battlemanager to dequeue a new defense prompt
                BattleManager.current.DequeuDefensePrompt();

                //reset if we got an input from the last defense
                InputManager.current.gotInputLastDefense = false;

            }
            else
            {
                //we're just waiting for a defense prompt
            }

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
