namespace HarmonyQuest.Audio
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using GameManager;

    /// <summary>
    /// This class will be the primary way in which other scripts interface with our fmod logic.
    /// </summary>
    public class FmodFacade : ManageableObject
    {
        //Enum used to quantify how close an action is to the beat.
        public enum OnBeatAccuracy
        {
            Great = 1,
            Good = 2,
            Miss = 3,
        }

        public static FmodFacade instance;

        //Our pooling system for preloading fmod events. Helps reduce latency when playing sounds when used.
        private FmodEventPool fmodEventPool;

        //Dictionary used to convert our fmod event and parameter names into the values fmod will actually use.
        private FmodDictionary fmodDictionary;

        [SerializeField]
        private bool debugOneShotEvents = false;

        public override void OnAwake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
            fmodDictionary = new FmodDictionary();
            fmodEventPool = new FmodEventPool();
        }

        /// <summary>
        /// Starts an fmod music event, which will fire beat and marker callbacks and loop until it is told to stop.
        /// </summary>
        /// <param name="name"> The name of the fmod music event </param>
        /// <param name="volume"> The volume of the fmod music event </param>
        public void StartMusic(string name, float volume)
        {
            FmodMusicHandler.instance.StartMusic(name, volume);
        }

        /// <summary>
        /// Stops the active fmod music event if there is one. We will no longer receive beat and marker callbacks if no music is playing.
        /// </summary>
        public void StopMusic()
        {
            FmodMusicHandler.instance.StopMusic();
        }

        /// <summary>
        /// Sets a param for the current fmod music event.
        /// </summary>
        /// <param name="param"> The name of the param </param>
        /// <param name="value">The new param value </param>
        public void SetMusicParam(string param, float value)
        {
            FmodMusicHandler.instance.SetMusicParam(param, value);
        }

        /// <summary>
        /// Starts an fmod ambience event
        /// </summary>
        /// <param name="name"> The name of the fmod ambience event </param>
        /// <param name="volume"> The volume of the fmod ambience event </param>
        public void StartAmbience(string name, float volume)
        {
            FmodMusicHandler.instance.StartAmbience(name, volume);
        }

        /// <summary>
        /// Stops the active fmod ambience event if there is one.
        /// </summary>
        public void StopAmbience()
        {
            FmodMusicHandler.instance.StopAmbience();
        }

        /// <summary>
        /// Sets a param for the current fmod ambience event.
        /// </summary>
        /// <param name="param"> The name of the param </param>
        /// <param name="value">The new param value </param>
        public void SetAmbienceParam(string param, float value)
        {
            FmodMusicHandler.instance.SetAmbienceParam(param, value);
        }

        /// <summary>
        /// Returns which beat of the measure we're on. i.e. Beat 1, 2, 3 or 4 for a 4/4 song.
        /// </summary>
        /// <returns> Which beat of the measure we're on. </returns>
        public int GetCurrentBeat()
        {
            return FmodMusicHandler.instance.GetCurrentBeat();
        }

        /// <summary>
        /// Get the length of a beat at our current bpm.
        /// </summary>
        /// <returns> The length of a beat at our current bpm. </returns>
        public float GetBeatDuration()
        {
            return FmodMusicHandler.instance.GetBeatDuration();
        }

        /// <summary>
        /// Get our current tempo.
        /// </summary>
        /// <returns> Our current tempo. </returns>
        public float GetCurrentMusicTempo()
        {
            return FmodMusicHandler.instance.GetCurrentMusicTempo();
        }

        /// <summary>
        /// Get the current fmod music event.
        /// </summary>
        /// <returns> The current fmod music event. </returns>
        public FMOD.Studio.EventInstance GetMusicEvent()
        {
            return FmodMusicHandler.instance.GetMusicEvent();
        }

        /// <summary>
        /// Sets a param for the passed in fmod event
        /// </summary>
        /// <param name="fmodEvent"> The name of the fmod event we want to access </param>
        /// <param name="parameter"> The name of the param in our fmod event </param>
        /// <param name="value"> The value we want to set our param to </param>
        public void SetFmodParameterValue(FMOD.Studio.EventInstance fmodEvent, string parameter, float value)
        {
            //print("Set param " + parameter + ", dict value " + GetFmodParamFromDictionary(parameter) + " TO " + value);
            fmodEvent.setParameterValue(GetFmodParamFromDictionary(parameter), value);
        }

        /// <summary>
        /// Plays the passed in fmod event
        /// </summary>
        /// <param name="fmodEvent"> The name of the fmod event we want to play </param>
        /// <param name="volume"> The volume of the fmod event we want to play </param>
        public void PlayFmodEvent(FMOD.Studio.EventInstance fmodEvent, float volume = 1.0f)
        {
            fmodEvent.setVolume(volume);
            fmodEvent.start();
        }

        /// <summary>
        /// Stops the passed in fmod event
        /// </summary>
        /// <param name="fmodEvent"> The name of the event we want to stop </param>
        public void StopFmodEvent(FMOD.Studio.EventInstance fmodEvent)
        {
            fmodEvent.setUserData(IntPtr.Zero);
            fmodEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            fmodEvent.release();
        }

        /// <summary>
        /// Plays the passed in fmod event as a one shot event, meaning it is set to release the event automatically as soon as it stops playing.
        /// This allows for instanced fmod events without letting them infinitely build up.
        /// </summary>
        /// <param name="fmodEvent"> The name of the event we want to play as a one shot event </param>
        /// <param name="volume"> The volume of the event we want to play as a one shot event </param>
        /// <param name="paramData"> An array of param data that should be passed to our fmod event before playing it </param>
        public void PlayOneShotFmodEvent(FMOD.Studio.EventInstance fmodEvent, float volume = 1.0f, FmodParamData[] paramData = null)
        {
            if (paramData != null)
            {
                for (int i = 0; i < paramData.Length; i++)
                {
                    SetFmodParameterValue(fmodEvent, paramData[i].paramName, paramData[i].paramValue);
                }
            }
            fmodEvent.setVolume(volume);
            if (debugOneShotEvents)
            {
                Debug.Log("1. START ONE SHOT EVENT");
                FMOD.Studio.EVENT_CALLBACK stoppedOneShotEventCallback;
                stoppedOneShotEventCallback = new FMOD.Studio.EVENT_CALLBACK(StoppedOneShotEventCallback);
                fmodEvent.setCallback(stoppedOneShotEventCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.STOPPED);
            }
            fmodEvent.start();
            fmodEvent.release();
        }

        [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
        static FMOD.RESULT StoppedOneShotEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, FMOD.Studio.EventInstance instance, IntPtr parameterPtr)
        {
            Debug.Log("2. RELEASE ONE SHOT EVENT");
            return FMOD.RESULT.OK;
        }

        /// <summary>
        /// Creates a new instance of an fmod event based on the even path passed in
        /// </summary>
        /// <param name="eventPath"> A path to the fmod event we want to create an instance of </param>
        /// <param name="parent"> The gameobject our sound should be parented to. Used for fmod to track 3D sound properties </param>
        /// <param name="rb"> The rigidbody that our sound should follow the velocity of. Used for fmod to track 3D sound properties </param>
        /// <returns> A reference to the newly created fmod event </returns>
        public FMOD.Studio.EventInstance CreateFmodEventInstance(string eventPath, GameObject parent = null, Rigidbody rb = null)
        {
            FMOD.Studio.EventInstance fmodEvent = FMODUnity.RuntimeManager.CreateInstance(eventPath);
            if (parent != null && rb != null)
            {
                FMODUnity.RuntimeManager.AttachInstanceToGameObject(fmodEvent, parent.transform, rb);
            }
            return fmodEvent;
        }

        /// <summary>
        /// Creates a new instance of an fmod event, and runs it as a one shot event that releases when it is complete.
        /// </summary>
        /// <param name="eventPath"> The name of the event we want to create and play as a one shot event </param>
        /// <param name="volume"> The volume of the event we want to create and play as a one shot event </param>
        /// <param name="parent"> The gameobject our sound should be parented to. Used for fmod to track 3D sound properties </param>
        /// <param name="rb"> The rigidbody that our sound should follow the velocity of. Used for fmod to track 3D sound properties </param>
        /// <param name="paramData"> An array of param data that should be passed to our fmod event before playing it </param>
        public void CreateAndRunOneShotFmodEvent(string eventPath, float volume = 1.0f, GameObject parent = null, Rigidbody rb = null, FmodParamData[] paramData = null)
        {
            FMOD.Studio.EventInstance fmodEvent = CreateFmodEventInstance(eventPath, parent, rb);
            PlayOneShotFmodEvent(fmodEvent, volume, paramData);
        }

        /// <summary>
        /// Plays an fmod event using our object pool. This saves cpu performance since we aren't creating/destroying events every time we play one.
        /// </summary>
        /// <param name="eventName"> The name of the event we want to play </param>
        /// <param name="volume"> The volume of the event we want to play </param>
        /// <param name="parent"> The gameobject our sound should be parented to. Used for fmod to track 3D sound properties </param>
        /// <param name="rb"> The rigidbody that our sound should follow the velocity of. Used for fmod to track 3D sound properties </param>
        /// <param name="paramData"> An array of param data that should be passed to our fmod event before playing it </param>
        public void PlayPooledFmodEvent(string eventName, float volume = 1.0f, GameObject parent = null, Rigidbody rb = null, FmodParamData[] paramData = null)
        {
            fmodEventPool.PlayEvent(eventName, volume, parent, rb, paramData);
        }

        /// <summary>
        /// Function that returns how close to on beat we currently are. Accuracy is determined based on FmodOnBeatAccuracyChecker's onBeatPadding param.
        /// </summary>
        /// <returns> An OnBeatAccuracy value based on close to the beat we are right now </returns>
        public OnBeatAccuracy WasActionOnBeat(bool useDegreesOfOnBeatAccuracyOverride = false)
        {
            return FmodOnBeatAccuracyChecker.instance.WasActionOnBeat(useDegreesOfOnBeatAccuracyOverride);
        }

        /// <summary>
        /// Used to let FmodOnBeatAccuracyChecker know that we've attempted an on beat action this beat.
        /// </summary>
        public void PerformOnBeatAction()
        {
            FmodOnBeatAccuracyChecker.instance.PerformOnBeatAction();
        }

        /// <summary>
        /// Check FmodOnBeatAccuracyChecker to see if we've already tried an on beat action this beat. If so, don't allow any more on beat actions until the next beat.
        /// </summary>
        /// <returns></returns>
        public bool HasPerformedActionThisBeat()
        {
            return FmodOnBeatAccuracyChecker.instance.HasPerformedActionThisBeat();
        }

        /// <summary>
        /// Return a value from 0 to 1 gauging how far into a beat we are. i.e. 0 is the start of a beat, 0.5 is halfway through a beat, 1 is the end of a beat.
        /// </summary>
        /// <returns></returns>
        public float GetNormalizedBeatProgress()
        {
            return FmodOnBeatAccuracyChecker.instance.GetNormalizedBeatProgress();
        }

        /// <summary>
        /// Used to convert our music key into an fmod path.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetFmodMusicEventFromDictionary(string key)
        {
            string val;
            fmodDictionary.fmodMusicEventDictionary.TryGetValue(key, out val);
            return val;
        }

        /// <summary>
        /// Used to convert our sfx key into an fmod path.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetFmodSFXEventFromDictionary(string key)
        {
            string val;
            fmodDictionary.fmodSFXEventDictionary.TryGetValue(key, out val);
            return val;
        }

        /// <summary>
        /// Used to convert our stand in param name into an fmod param name.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetFmodParamFromDictionary(string key)
        {
            string val;
            fmodDictionary.fmodParamDictionary.TryGetValue(key, out val);
            return val;
        }

        /// <summary>
        /// Returns a list of all of our fmod sfx names
        /// </summary>
        /// <returns></returns>
        public List<string> GetFmodSFXEventNames()
        {
            return fmodDictionary.fmodSFXEventDictionary.Keys.ToList();
        }

        /*public void GetDSPData()
        {
            FMOD.RESULT result;
            uint blocksize;
            int numblocks;
            int frequency;
            float ms;

            FMOD.SPEAKERMODE speakerMode;
            int numSpeakers;

            result = FMOD.System.getDSPBufferSize(out blocksize, out numblocks);
            result = FMOD.System.getSoftwareFormat(out frequency, out speakerMode, out numSpeakers);

            ms = (float)blocksize * 1000.0f / (float)frequency;

            print("Mixer blocksize        = " + ms + " ms\n");
            print("Mixer Total buffersize = " + ms * numblocks + " ms\n");
            print("Mixer Average Latency  = " + ms * ((float)numblocks - 1.5f) + " ms\n");
        }*/
    }
}
