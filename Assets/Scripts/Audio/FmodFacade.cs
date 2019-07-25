using System;
using UnityEngine;

/// <summary>
/// This class will be the primary way in which other scripts interface with our fmod logic.
/// </summary>
public class FmodFacade : MonoBehaviour
{
    public static FmodFacade instance;

    [SerializeField]
    private bool debugOneShotEvents = false;

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
    /// Sets a param for the passed in fmod event
    /// </summary>
    /// <param name="fmodEvent"> The name of the fmod event we want to access </param>
    /// <param name="parameter"> The name of the param in our fmod event </param>
    /// <param name="value"> The value we want to set our param to </param>
    public void SetFmodParameterValue(FMOD.Studio.EventInstance fmodEvent, string parameter, float value)
    {
        fmodEvent.setParameterValue(parameter, value);
    }

    /// <summary>
    /// Sets a param for our fmod music event. Same logic as SetFmodParameterValue, except it will grab the currently active fmod music event on its own.
    /// </summary>
    /// <param name="parameter"> The name of the param in our fmod event </param>
    /// <param name="value"> The value we want to set our param to </param>
    public void SetMusicEventParameterValue(string parameter, float value)
    {
        FMOD.Studio.EventInstance fmodEvent = FmodMusicHandler.instance.GetMusicEvent();
        fmodEvent.setParameterValue(parameter, value);
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
    /// <param name="eventName"> A path to the fmod event we want to create an instance of </param>
    /// <param name="parent"> The gameobject our sound should be parented to. Used for fmod to track 3D sound properties </param>
    /// <param name="rb"> The rigidbody that our sound should follow the velocity of. Used for fmod to track 3D sound properties </param>
    /// <returns> A reference to the newly created fmod event </returns>
    public FMOD.Studio.EventInstance CreateFmodEventInstance(string eventName, GameObject parent = null, Rigidbody rb = null)
    {
        FMOD.Studio.EventInstance fmodEvent = FMODUnity.RuntimeManager.CreateInstance(eventName);
        if (parent != null && rb != null)
        {
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(fmodEvent, parent.transform, rb);
        }
        return fmodEvent;
    }

    /// <summary>
    /// Creates a new instance of an fmod event, and runs it as a one shot event that releases when it is complete.
    /// </summary>
    /// <param name="eventName"> The name of the event we want to create and play as a one shot event </param>
    /// <param name="volume"> The volume of the event we want to create and play as a one shot event </param>
    /// <param name="parent"> The gameobject our sound should be parented to. Used for fmod to track 3D sound properties </param>
    /// <param name="rb"> The rigidbody that our sound should follow the velocity of. Used for fmod to track 3D sound properties </param>
    /// <param name="paramData"> An array of param data that should be passed to our fmod event before playing it </param>
    public void CreateAndRunOneShotFmodEvent(string eventName, float volume = 1.0f, GameObject parent = null, Rigidbody rb = null, FmodParamData[] paramData = null)
    {
        FMOD.Studio.EventInstance fmodEvent = CreateFmodEventInstance(eventName, parent, rb);
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
        FmodEventPool.instance.PlayEvent(eventName, volume, parent, rb, paramData);
    }
}
