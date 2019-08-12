﻿namespace HarmonyQuest.Audio
{
    using System.Collections.Generic;
    using UnityEngine;

    public class FmodEventPool : MonoBehaviour
    {
        public static FmodEventPool instance;

        public FmodEventPoolableObject[] events;
        public int[] eventPoolSizes;

        private Dictionary<string, List<FmodEventPoolableObject>> eventPools;

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
            InitPools();
        }

        void InitPools()
        {
            eventPools = new Dictionary<string, List<FmodEventPoolableObject>>();
            for (int i = 0; i < events.Length; i++)
            {
                List<FmodEventPoolableObject> eventPool = new List<FmodEventPoolableObject>();
                for (int j = 0; j < eventPoolSizes[i]; j++)
                {
                    FmodEventPoolableObject eventGameobject = Instantiate(events[i], transform);
                    eventGameobject.Init(j);
                    eventPool.Add(eventGameobject);
                }
                eventPools.Add(events[i].eventName, eventPool);
            }
        }

        public FmodEventPoolableObject PlayEvent(string eventName, float volume = 1.0f, GameObject parent = null, Rigidbody rb = null, FmodParamData[] paramData = null)
        {
            List<FmodEventPoolableObject> eventPool;
            eventPools.TryGetValue(eventName, out eventPool);

            if (eventPool != null)
            {
                for (int i = 0; i < eventPool.Count; i++)
                {
                    if (eventPool[i].isReadyToPlay == true)
                    {
                        eventPool[i].Play(volume, parent, rb, paramData);
                        //Prepare our next event object before it is told to play
                        eventPool[(i + 1) % eventPool.Count].Restart();
                        return eventPool[i];
                    }
                }
            }

            Debug.LogWarning("No fmod event object available for event " + eventName + ". Consider increasing the pool size.");
            return null;
        }

        public FmodEventPoolableObject RestartEvent(string eventName, int index)
        {
            List<FmodEventPoolableObject> eventPool;
            eventPools.TryGetValue(eventName, out eventPool);

            if (eventPool != null && eventPool.Count > index && eventPool[index] != null)
            {
                eventPool[index].Restart();
                return eventPool[index];
            }

            Debug.LogWarning("No fmod event exists for attempt to restart " + eventName + " at index " + index + ". Doing nothing.");
            return null;
        }

        public void ClearPools()
        {
            eventPools.Clear();
        }
    }
}
