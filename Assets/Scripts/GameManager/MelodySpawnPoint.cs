namespace Manager
{
    using GameManager;
    using UnityEngine;

    public class MelodySpawnPoint : ManageableObject
    {
        public string id;
        public GameObject visualizer;

        public override void OnAwake()
        {
            visualizer.SetActive(false);
        }
    }
}
