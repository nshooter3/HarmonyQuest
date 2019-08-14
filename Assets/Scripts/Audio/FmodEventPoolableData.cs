namespace HarmonyQuest.Audio
{
    using UnityEngine;

    //This ScriptableObject class simply holds an fmod event, and how many instances of it should be created as FmodEventPoolableObjects to be stored in FmodEventPool.cs
    [CreateAssetMenu(fileName = "FmodEventPoolableData", menuName = "ScriptableObjects/FmodEventPoolableData", order = 1)]
    public class FmodEventPoolableData : ScriptableObject
    {
        public string eventName;

        //Default pool size of 5. Can be overridden in the editor field.
        public int poolableObjectCount = 5;
    }
}
