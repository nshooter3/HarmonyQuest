namespace HarmonyQuest.Audio
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Static class used to determine which music should be playing in which scenes.
    /// </summary>
    public static class FmodSceneMusicDictionary
    {
        private static string dictResult;

        //Scene-name-to-FmodEventDictionary-entry dictionary for Music.
        private static Dictionary<string, string> sceneToMusicDictionary = new Dictionary<string, string>()
        {
            //{ "", "" },

            #region TestScenes
            { "BoxPushTest", "twilit_hollow_dissonance" },
            { "CameraTest", "twilit_hollow_dissonance" },
            { "CombatTest", "twilit_hollow_dissonance" },
            { "GrapplingHookTest", "twilit_hollow_dissonance" },
            { "PhysicsTest", "twilit_hollow_dissonance" },
            { "SceneTransitionTest", "twilit_hollow" },
            { "SceneTransitionTest2", "twilit_hollow_dissonance" },
            #endregion
        };

        public static string GetSceneMusic(string scene)
        {
            if (sceneToMusicDictionary.TryGetValue(scene, out dictResult))
            {
                return dictResult;
            }
            else
            {
                Debug.LogWarning("Warning: No music assigned to scene " + scene + " in the FmodSceneMusicDictionary. Loading the placeholder music.");
                return FmodFacade.instance.GetFmodMusicEventFromDictionary("placeholder");
            }
        }
    }
}
