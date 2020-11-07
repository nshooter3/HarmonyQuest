﻿namespace HarmonyQuest.Audio
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using GameManager;
    using Manager;
    using Saving;

    public class FmodMusicHandler : ManageableObject
    {
        public static FmodMusicHandler instance;

        private string musicEventName;

        [SerializeField]
        private bool memoryDebug = false;

        [HideInInspector]
        public bool isMusicPlaying = false;

        private bool isFadingScreenTransition = false;

        /// <summary>
        /// Delegate that gets called when we get a beat callback from fmod. Load any functions that need to get called on beat here.
        /// </summary>
        public delegate void OnBeatDelegate();
        OnBeatDelegate onBeatDelegate;

        /// <summary>
        /// Delegate that gets called when we get a chord marker callback from fmod. Load any functions that need to get called on a new chord marker here.
        /// </summary>
        public delegate void OnChordMarkerDelegate(List<FmodNote> chord);
        OnChordMarkerDelegate onChordMarkerDelegate;

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

        FMOD.Studio.EventInstance musicEvent;
        FMOD.Studio.EVENT_CALLBACK beatCallback;

        FMOD.Studio.PLAYBACK_STATE playbackState;

        public override void OnAwake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if(instance != this)
            {
                Destroy(gameObject);
            }

            timelineInfo = new TimelineInfo();
            beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);
        }

        public override void OnUpdate()
        {
            if (isFadingScreenTransition)
            {
                musicEvent.getPlaybackState(out playbackState);
                if (playbackState == FMOD.Studio.PLAYBACK_STATE.STOPPED)
                {
                    isFadingScreenTransition = false;
                    SceneTransitionManager.isMusicTransitionDone = true;
                    StopMusic();
                }
            }
        }

        public void StartMusic(string name, float volume)
        {
            musicEventName = name;

            musicEvent = FmodFacade.instance.CreateFmodEventInstance(FmodFacade.instance.GetFmodMusicEventFromDictionary(name));

            musicEvent.setCallback(beatCallback,
                  FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT
                | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER
                | FMOD.Studio.EVENT_CALLBACK_TYPE.STARTED
                );

            // Pin the class that will store the data modified during the callback
            timelineHandle = GCHandle.Alloc(timelineInfo, GCHandleType.Pinned);

            // Pass the object through the userdata of the instance
            musicEvent.setUserData(GCHandle.ToIntPtr(timelineHandle));

            FmodFacade.instance.PlayFmodEvent(musicEvent, volume);
            isMusicPlaying = true;
        }

        public void StopMusic()
        {
            if (isMusicPlaying == true)
            {
                musicEventName = "";
                timelineHandle.Free();
                FmodFacade.instance.StopFmodEvent(musicEvent);
                isMusicPlaying = false;
            }
        }

        public void SetMusicParam(string param, float value)
        {
            if (isMusicPlaying == true)
            {
                FmodFacade.instance.SetFmodParameterValue(musicEvent, param, value);
            }
        }

        public void FadeOutAll()
        {
            //Only fade out music if the next scene has different music.
            if (SceneLoadObjectDictionary.instance.GetSceneLoadObject(SaveDataManager.saveData.currentScene).fmodMusicEvent != musicEventName)
            {
                FmodFacade.instance.SetMusicParam("global_event_end_param", 1f);
                isFadingScreenTransition = true;
            }
            else
            {
                SceneTransitionManager.isMusicTransitionDone = true;
            }
        }

        public bool IsMusicFadingSceneTransition()
        {
            return isFadingScreenTransition;
        }

        public void PauseMusic()
        {
            FmodFacade.instance.PauseFmodEvent(musicEvent);
        }

        public void ResumeMusic()
        {
            FmodFacade.instance.ResumeFmodEvent(musicEvent);
        }

        private void OnDestroy()
        {
            StopMusic();
        }

        public string GetMusicEventName()
        {
            return musicEventName;
        }

        public int GetCurrentBeat()
        {
            return timelineInfo.currentMusicBeat;
        }

        public float GetBeatDuration()
        {
            return 60.0f / timelineInfo.currentMusicTempo;
        }

        public float GetCurrentMusicTempo()
        {
            return timelineInfo.currentMusicTempo;
        }

        public FMOD.Studio.EventInstance GetMusicEvent()
        {
            return musicEvent;
        }

        [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
        static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, FMOD.Studio.EventInstance instance, IntPtr parameterPtr)
        {
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
                            timelineInfo.currentMusicBar = parameter.bar;
                            timelineInfo.currentMusicBeat = parameter.beat;
                            timelineInfo.currentMusicTempo = parameter.tempo;
                            timelineInfo.currentMusicPosition = parameter.position;
                            timelineInfo.currentMusicTimeSignatureUpper = parameter.timesignatureupper;
                            timelineInfo.currentMusicTimeSignatureLower = parameter.timesignaturelower;
                            FmodMusicHandler.instance.onBeatDelegate();
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
                                if (FmodMusicHandler.instance.onChordMarkerDelegate != null)
                                {
                                    FmodMusicHandler.instance.onChordMarkerDelegate(FmodChordInterpreter.instance.GetFmodChord());
                                }
                                //FmodChordInterpreter.instance.PrintCurrentChord();
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

        /// <summary>
        /// If a function needs to be called on beat with the music, pass it in here!
        /// </summary>
        /// <param name="func"> Function to be called on beat </param>
        public void AssignFunctionToOnBeatDelegate(OnBeatDelegate func)
        {
            onBeatDelegate += func;
        }

        public void RemoveFunctionFromOnBeatDelegate(OnBeatDelegate func)
        {
            onBeatDelegate -= func;
        }

        public void ClearOnBeatDelegate()
        {
            onBeatDelegate = null;
        }

        /// <summary>
        /// If a function needs to be called on a chord change, pass it in here!
        /// </summary>
        /// <param name="func"> Function to be called on a chord change </param>
        public void AssignFunctionToOnChordMarkerDelegate(OnChordMarkerDelegate func)
        {
            onChordMarkerDelegate += func;
        }

        public void RemoveFunctionFromOnChordMarkerDelegate(OnChordMarkerDelegate func)
        {
            onChordMarkerDelegate -= func;
        }

        public void ClearOnChordMarkerDelegate()
        {
            onChordMarkerDelegate = null;
        }

        void OnGUI()
        {
            if (memoryDebug)
            {
                int curAlloc, maxAlloc;
                FMOD.Memory.GetStats(out curAlloc, out maxAlloc);
                GUILayout.Box("Fmod cur memory alloc: " + curAlloc + ", Fmod max memory alloc: " + maxAlloc);
            }
        }
    }
}