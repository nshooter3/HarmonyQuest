namespace HarmonyQuest.Audio
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using GameManager;
    using Manager;

    public class FmodMusicHandler : ManageableObject
    {
        public static FmodMusicHandler instance;

        private string musicEventName;
        private string ambienceEventName;

        [SerializeField]
        private bool memoryDebug = false;

        [HideInInspector]
        public bool isMusicPlaying = false;

        [HideInInspector]
        public bool isAmbiencePlaying = false;

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
        FMOD.Studio.EventInstance ambienceEvent;
        FMOD.Studio.EVENT_CALLBACK beatCallback;

        private float fadeOutTimer, maxFadeOutTimer = 1.5f;

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
            if (fadeOutTimer > 0f)
            {
                fadeOutTimer = Mathf.Max(fadeOutTimer - Time.deltaTime, 0f);
                FmodFacade.instance.SetMusicParam("global_master_fade_.5_param", 1f - fadeOutTimer/maxFadeOutTimer);
                //Ambience doesn't have a master fade param yet, so we'll have to set that up later.
                if (fadeOutTimer <= 0f)
                {
                    SceneTransitionManager.isMusicTransitionDone = true;
                    StopAll();
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

        public void StartAmbience(string name, float volume)
        {
            ambienceEventName = name;

            ambienceEvent = FmodFacade.instance.CreateFmodEventInstance(FmodFacade.instance.GetFmodMusicEventFromDictionary(name));

            FmodFacade.instance.PlayFmodEvent(ambienceEvent, volume);
            isAmbiencePlaying = true;
        }

        public void StopAmbience()
        {
            if (isAmbiencePlaying == true)
            {
                ambienceEventName = "";
                FmodFacade.instance.StopFmodEvent(ambienceEvent);
                isAmbiencePlaying = false;
            }
        }

        public void SetAmbienceParam(string param, float value)
        {
            if (isAmbiencePlaying == true)
            {
                FmodFacade.instance.SetFmodParameterValue(ambienceEvent, param, value);
            }
        }

        public void PauseAll()
        {
            FmodFacade.instance.PauseFmodEvent(musicEvent);
            FmodFacade.instance.PauseFmodEvent(ambienceEvent);
        }

        public void ResumeAll()
        {
            FmodFacade.instance.ResumeFmodEvent(musicEvent);
            FmodFacade.instance.ResumeFmodEvent(ambienceEvent);
        }

        public void StopAll()
        {
            StopMusic();
            StopAmbience();
        }

        public void FadeOutAll()
        {
            Debug.Log("FadeOutAll");
            fadeOutTimer = maxFadeOutTimer;
        }

        public bool IsMusicFading()
        {
            return fadeOutTimer > 0;
        }

        private void OnDestroy()
        {
            StopAll();
        }

        public string GetMusicEventName()
        {
            return musicEventName;
        }

        public string GetAmbienceEventName()
        {
            return ambienceEventName;
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