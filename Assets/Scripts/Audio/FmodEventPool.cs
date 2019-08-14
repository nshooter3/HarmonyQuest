namespace HarmonyQuest.Audio
{
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;

    public class FmodEventPool
    {
        //eventData gets loaded automatically by grabbing the FmodEventPoolableData components attached to the scriptable objects
        //in the Resources folder defined by fmodEventPoolDataFolderPath.
        private string fmodEventPoolDataFolderPath = "ScriptableObjects/Fmod";
        private FmodEventPoolableData[] eventData;

        private Dictionary<string, List<FmodEventPoolableObject>> eventPools;

        //Variables used to retrieve and use data from eventData for populating eventPools.  
        private string initEventName = "";
        private int initEventPoolCount = 0;

        public FmodEventPool()
        {
            InitPools();
        }

        void InitPools()
        {
            eventData = Resources.LoadAll(fmodEventPoolDataFolderPath, typeof(FmodEventPoolableData)).Cast<FmodEventPoolableData>().ToArray();
            eventPools = new Dictionary<string, List<FmodEventPoolableObject>>();

            for (int i = 0; i < eventData.Length; i++)
            {
                initEventName = eventData[i].eventName;
                initEventPoolCount = eventData[i].poolableObjectCount;
                List<FmodEventPoolableObject> eventPool = new List<FmodEventPoolableObject>();
                for (int j = 0; j < initEventPoolCount; j++)
                {
                    FmodEventPoolableObject eventGameobject = new FmodEventPoolableObject(initEventName, j);
                    eventPool.Add(eventGameobject);
                }
                eventPools.Add(initEventName, eventPool);
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
