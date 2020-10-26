namespace Manager
{
    using Saving;
    using UnityEngine.SceneManagement;

    public static class SceneTransitionManager
    {
        public static bool isTransitioning = false;
        public static bool isMusicTransitionDone = false;

        //Set the information we'll need for when it's time to change scenes.
        //Sets the isTransitioning flag, which prevents various things from updating while the scene is transitioning.
        public static void PrepareNewScene(string sceneName, string doorName)
        {
            isTransitioning = true;
            SaveDataManager.saveData.currentScene = sceneName;
            SaveDataManager.saveData.currentDoor = doorName;
        }

        public static void TransitionToNewScene()
        {
            isTransitioning = false;
            isMusicTransitionDone = false;
            SceneManager.LoadScene(SaveDataManager.saveData.currentScene);
        }
    }
}
