namespace GameManager
{
    using UnityEngine;
    using UnityEngine.Rendering.PostProcessing;

    //This ScriptableObject class holds scene specific data for loading in stuff like music and VFX.
    [CreateAssetMenu(fileName = "SceneLoadObject", menuName = "ScriptableObjects/SceneLoadObject", order = 1)]
    public class SceneLoadObject : ScriptableObject
    {
        public string fmodMusicEvent;
        public PostProcessProfile postProcessProfile;
        //TODO: Actually do something with this.
        public LightSettingsObject lightSettingsObject;
    }
}
