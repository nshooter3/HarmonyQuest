using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class FmodListener : MonoBehaviour
{
    public static FmodListener instance;

    // Variables that are modified in the callback need to be part of a seperate class.
    // This class needs to be 'blittable' otherwise it can't be pinned in memory.
    [StructLayout(LayoutKind.Sequential)]
    class TimelineInfo
    {
        public int currentMusicBar = 0;
        public int currentMusicBeat = 0;
        public float currentMusicTempo = 0;
        public float currentMusicPosition = 0;
        public float currentMusicTimeSignatureUpper = 0;
        public float currentMusicTimeSignatureLower = 0;
        public FMOD.StringWrapper lastMarker = new FMOD.StringWrapper();
    }

    TimelineInfo timelineInfo;
    GCHandle timelineHandle;

    FMODUnity.StudioEventEmitter emitter;
    FMOD.Studio.EVENT_CALLBACK beatCallback;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        timelineInfo = new TimelineInfo();

        emitter = GetComponent<FMODUnity.StudioEventEmitter>();

        beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);


        // Pin the class that will store the data modified during the callback
        timelineHandle = GCHandle.Alloc(timelineInfo, GCHandleType.Pinned);
        // Pass the object through the userdata of the instance
        emitter.EventInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));

        //emitter.EventInstance.setVolume(0.1f);

        //emitter.EventInstance.setVolume(0);

        emitter.EventInstance.setCallback(beatCallback,
              FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT
            | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER
            | FMOD.Studio.EVENT_CALLBACK_TYPE.STARTED
            );
    }

    /*void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartEvent();
        }
    }

    void StartEvent()
    {
        emitter.enabled = true;
    }*/

    public void StopEvent()
    {
        emitter.EventInstance.setUserData(IntPtr.Zero);
        emitter.EventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        emitter.EventInstance.release();
        timelineHandle.Free();
    }

    private void OnDestroy()
    {
        StopEvent();
    }

    public void SetFmodParameterValue(string parameter, float value)
    {
        print(emitter.EventInstance.setParameterValue(parameter, value));
    }

    void OnGUI()
    {
        GUILayout.Box(String.Format("Current Beat = {0}, Last Marker = {1}", timelineInfo.currentMusicBeat, (string)timelineInfo.lastMarker));
    }

    public int GetCurrentBeat()
    {
        return timelineInfo.currentMusicBeat;
    }


    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, FMOD.Studio.EventInstance instance, IntPtr parameterPtr)
    {
        //Debug.Log("BeatEventCallback");
        // Retrieve the user data
        IntPtr timelineInfoPtr;
        FMOD.RESULT result = instance.getUserData(out timelineInfoPtr);

        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("Timeline Callback error: " + result);
        }
        else if (timelineInfoPtr != IntPtr.Zero)
        {
            // Get the object to store beat and marker details
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

            switch (type)
            {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                        //print("BEAT " + parameter.beat + " CALLBACK");
                        //print("TEMPO = " + parameter.tempo);
                        timelineInfo.currentMusicBar = parameter.bar;
                        timelineInfo.currentMusicBeat = parameter.beat;
                        timelineInfo.currentMusicTempo = parameter.tempo;
                        timelineInfo.currentMusicPosition = parameter.position;
                        timelineInfo.currentMusicTimeSignatureUpper = parameter.timesignatureupper;
                        timelineInfo.currentMusicTimeSignatureLower = parameter.timesignaturelower;
                        TestBeatTracker.instance.Beat();
                    }
                    break;
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
                        timelineInfo.lastMarker = parameter.name;
                        //print(parameter.name + " MARKER CALLBACK");
                        if (FmodChordInterpreter.instance != null && FmodChordInterpreter.instance.IsFmodMarkerChordInformation(parameter.name))
                        {
                            FmodChordInterpreter.instance.ParseChordFromMarker(parameter.name);
                            //FmodChordInterpreter.instance.PrintCurrentChord();
                            //TestMusicalSfxPlayer.instance.PlayChord(FmodChordInterpreter.instance.GetFmodChord());
                        }
                    }
                    break;
                case FMOD.Studio.EVENT_CALLBACK_TYPE.STARTED:
                    {
                        //print("FMOD EVENT STARTED!");
                    }
                    break;
            }

        }
        return FMOD.RESULT.OK;
    }
}