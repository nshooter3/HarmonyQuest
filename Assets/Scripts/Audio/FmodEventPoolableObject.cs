﻿namespace HarmonyQuest.Audio
{
    using System;
    using UnityEngine;

    [CreateAssetMenu(fileName = "FmodEventData", menuName = "ScriptableObjects/FmodEventPoolableObject", order = 1)]
    public class FmodEventPoolableObject : ScriptableObject
    {
        public string eventName;
        [HideInInspector]
        public int index = -1;
        [HideInInspector]
        public bool isReadyToPlay = false;

        private FMOD.Studio.EventInstance fmodEvent;

        public void Init(int index)
        {
            this.index = index;
            this.fmodEvent = FmodFacade.instance.CreateFmodEventInstance(eventName);
            isReadyToPlay = true;
        }

        public void Play(float volume = 1.0f, GameObject parent = null, Rigidbody rb = null, FmodParamData[] paramData = null)
        {
            //print("PLAY FMOD EVENT " + eventName + " INDEX " + index);
            fmodEvent.setVolume(volume);
            if (parent != null && rb != null)
            {
                FMODUnity.RuntimeManager.AttachInstanceToGameObject(fmodEvent, parent.transform, rb);
            }
            if (paramData != null)
            {
                for (int i = 0; i < paramData.Length; i++)
                {
                    FmodFacade.instance.SetFmodParameterValue(fmodEvent, paramData[i].paramName, paramData[i].paramValue);
                }
            }

            fmodEvent.start();
            isReadyToPlay = false;
        }

        public void Abort()
        {
            Restart();
        }

        public void Restart()
        {
            //print("RESTART FMOD EVENT " + eventName + " INDEX " + index);

            fmodEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            isReadyToPlay = true;
        }
    }
}
