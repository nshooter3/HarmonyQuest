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

    public void StartMusic(string name, float volume)
    {
        FmodMusicHandler.instance.StartMusic(name, volume);
    }

    public void StopMusic()
    {
        FmodMusicHandler.instance.StopMusic();
    }

    public void SetFmodParameterValue(FMOD.Studio.EventInstance fmodEvent, string parameter, float value)
    {
        fmodEvent.setParameterValue(parameter, value);
    }

    public void SetMusicEventParameterValue(string parameter, float value)
    {
        FMOD.Studio.EventInstance fmodEvent = FmodMusicHandler.instance.GetMusicEvent();
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

    public void CreateAndRunOneShotFmodEvent(string eventName, float volume = 1.0f, GameObject parent = null, Rigidbody rb = null, FmodParamData[] paramData = null)
    {
        FMOD.Studio.EventInstance fmodEvent = CreateFmodEventInstance(eventName);
        if (parent != null && rb != null)
        {
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(fmodEvent, parent.transform, rb);
        }
        if (paramData != null)
        {
            for (int i = 0; i < paramData.Length; i++)
            {
                SetFmodParameterValue(fmodEvent, paramData[i].paramName, paramData[i].paramValue);
            }
        }
        PlayOneShotFmodEvent(fmodEvent, volume);
    }
}
