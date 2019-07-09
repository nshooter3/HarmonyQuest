using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class FmodFacade : MonoBehaviour
{
    public static FmodFacade instance;

    [SerializeField]
    private bool debugOneShot = false;

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

    public void SetFmodParameterValue(FMOD.Studio.EventInstance fmodEvent, string parameter, float value)
    {
        fmodEvent.setParameterValue(parameter, value);
    }

    public void PlayFmodEvent(FMOD.Studio.EventInstance fmodEvent, float volume = 1.0f)
    {
        fmodEvent.setVolume(volume);
        fmodEvent.start();
    }

    public void StopFmodEvent(FMOD.Studio.EventInstance fmodEvent)
    {
        fmodEvent.setUserData(IntPtr.Zero);
        fmodEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        fmodEvent.release();
    }

    public void PlayOneShotFmodEvent(FMOD.Studio.EventInstance fmodEvent, float volume = 1.0f)
    {
        fmodEvent.setVolume(volume);
        if (debugOneShot)
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

    public FMOD.Studio.EventInstance CreateFmodEventInstance(string eventName)
    {
        return FMODUnity.RuntimeManager.CreateInstance(eventName);
    }

    public void CreateAndRunOneShotFmodEvent(string eventName, float volume = 1.0f, string parameter = "", float value = 0)
    {
        FMOD.Studio.EventInstance fmodEvent = CreateFmodEventInstance(eventName);
        if (parameter != "")
        {
            SetFmodParameterValue(fmodEvent, parameter, value);
        }
        PlayOneShotFmodEvent(fmodEvent, volume);
    }

    public void CreateAndRunOneShotFmodEvent(string eventName, float volume = 1.0f, string parameter = "", string parameter2 = "", float value = 0, float value2 = 0)
    {
        FMOD.Studio.EventInstance fmodEvent = CreateFmodEventInstance(eventName);
        if (parameter != "")
        {
            SetFmodParameterValue(fmodEvent, parameter, value);
        }
        if (parameter2 != "")
        {
            SetFmodParameterValue(fmodEvent, parameter2, value2);
        }
        PlayOneShotFmodEvent(fmodEvent, volume);
    }
}
