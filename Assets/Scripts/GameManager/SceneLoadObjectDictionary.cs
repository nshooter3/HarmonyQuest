namespace GameManager
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public class SceneLoadObjectDictionary
    {
        public static SceneLoadObjectDictionary instance;

        private Dictionary<string, SceneLoadObject> sceneLoadObjectDictionary = new Dictionary<string, SceneLoadObject>();

        private string[] sceneLoadObjects;

        private SceneLoadObject tempSceneLoadObject;
        private string path;

        public SceneLoadObjectDictionary()
        {
            if (instance != null)
            {
                throw new ApplicationException("SceneLoadObjectDictionary already exists, so nothing was done.");
            }
            instance = this;
            sceneLoadObjects = AssetDatabase.FindAssets("t:SceneLoadObject", new[] { "Assets/Scripts/GameManager/SceneLoadScriptableObjects" });
            foreach (string sceneLoadObject in sceneLoadObjects)
            {
                path = AssetDatabase.GUIDToAssetPath(sceneLoadObject);
                tempSceneLoadObject = AssetDatabase.LoadAssetAtPath<SceneLoadObject>(path);
                sceneLoadObjectDictionary.Add(tempSceneLoadObject.name, tempSceneLoadObject);
            }
        }

        public SceneLoadObject GetSceneLoadObject(string sceneName)
        {
            if (!sceneLoadObjectDictionary.TryGetValue(sceneName, out tempSceneLoadObject))
            {
                Debug.LogError("No SceneLoadObject with name " + sceneName + " exists. Please add one under Assets/Scripts/GameManager/SceneLoadScriptableObjects");
            }
            return tempSceneLoadObject;
        }
    }
}
