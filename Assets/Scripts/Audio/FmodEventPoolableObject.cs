namespace HarmonyQuest.Audio
{
    using UnityEngine;

    public class FmodEventPoolableObject
    {
        public string eventName;
        public int index = -1;
        public bool isPlaying = false;

        private FMOD.Studio.EventInstance fmodEvent;

        public FmodEventPoolableObject(string eventName, int index)
        {
            this.eventName = eventName;
            this.index = index;
            fmodEvent = FmodFacade.instance.CreateFmodEventInstance(FmodFacade.instance.GetFmodSFXEventFromDictionary(eventName));
            isPlaying = false;
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
            isPlaying = true;
        }

        public void Pause()
        {
            FmodFacade.instance.PauseFmodEvent(fmodEvent);
        }

        public void Resume()
        {
            FmodFacade.instance.ResumeFmodEvent(fmodEvent);
        }

        public void Abort()
        {
            RestartAndPause();
        }

        public void RestartAndPause()
        {
            //print("RESTART FMOD EVENT " + eventName + " INDEX " + index);

            fmodEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            isPlaying = false;
        }
    }
}
