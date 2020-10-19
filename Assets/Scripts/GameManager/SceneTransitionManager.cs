namespace Manager
{
    using Saving;
    using UnityEngine.SceneManagement;
    using HarmonyQuest.Audio;

    public static class SceneTransitionManager
    {
        public static void LoadNewScene(string sceneName, string doorName)
        {
            SaveDataManager.saveData.currentScene = sceneName;
            SaveDataManager.saveData.currentDoor = doorName;
            SceneManager.LoadScene(sceneName);
        }
    }
}
