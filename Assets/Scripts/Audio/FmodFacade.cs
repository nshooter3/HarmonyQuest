using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class FmodFacade : MonoBehaviour
{
    public static FmodFacade instance;

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
        fmodEvent.start();
        fmodEvent.release();
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
}
