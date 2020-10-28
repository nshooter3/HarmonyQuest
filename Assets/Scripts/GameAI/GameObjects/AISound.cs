namespace GameAI.AIGameObjects
{
    using HarmonyQuest.Audio;
    using System.Collections.Generic;
    using UnityEngine;

    public class AISound : MonoBehaviour
    {
        private AIGameObjectData data;

        [SerializeField]
        private FmodEventHandler[] fmodEvents;

        private Dictionary<string, FmodEventHandler> eventDictionary;

        public void Init(AIGameObjectData data)
        {
            this.data = data;

            eventDictionary = new Dictionary<string, FmodEventHandler>();

            foreach (FmodEventHandler fmodEvent in fmodEvents)
            {
                eventDictionary.Add(fmodEvent.sfxEventName, fmodEvent);
            }
        }

        //Separate function to ensure that we can subscribe to hitbox callback delegates.
        public void PlayFmodEvent(string eventName)
        {
            PlayFmodEvent(eventName, null);
        }

        public void PlayFmodEvent(string eventName, FmodParamData[] extraParams)
        {
            FmodEventHandler val;
            if (eventDictionary.TryGetValue(eventName, out val))
            {
                val.Play(extraParams);
            }
            else
            {
                Debug.LogWarning("AISound warning: fmod event with name of " + eventName + " does not exist on this agent.");
            }
        }
    }
}