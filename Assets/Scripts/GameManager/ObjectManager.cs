namespace GameManager
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// Class used to find and update multiple ManageableObjects in the scene.
    /// </summary>
    public class ObjectManager : ManageableObject
    {
        //A list containing lists of ManageableGameObjects. This gets populated in OnAwake by objects of the types we specify.
        private List<List<ManageableObject>> gameplayObjects = new List<List<ManageableObject>>();

        public void FindManageableObjectsInScene<T>() where T : ManageableObject
        {
            gameplayObjects.Add(FindObjectsOfType(typeof(T)).Cast<ManageableObject>().ToList());

            //Remove null entries as a failsafe.
            gameplayObjects = gameplayObjects.Where(item => item != null).ToList();
        }

        public override void OnAwake()
        {
            gameplayObjects.ForEach(p => p.ForEach(q => q.OnAwake()));
        }

        public override void OnStart()
        {
            gameplayObjects.ForEach(p => p.ForEach(q => q.OnStart()));
        }

        public override void OnUpdate()
        {
            gameplayObjects.ForEach(p => p.ForEach(q => q.OnUpdate()));
        }

        public override void OnLateUpdate()
        {
            gameplayObjects.ForEach(p => p.ForEach(q => q.OnLateUpdate()));
        }

        public override void OnFixedUpdate()
        {
            gameplayObjects.ForEach(p => p.ForEach(q => q.OnFixedUpdate()));
        }

        public override void OnAbort()
        {
            gameplayObjects.ForEach(p => p.ForEach(q => q.OnAbort()));
        }
    }
}

