namespace GameManager
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Class used to find and update multiple ManageableObjects in the scene.
    /// </summary>
    public class ObjectManager : ManageableObject
    {
        //A list containing lists of ManageableGameObjects. This gets populated in OnAwake by objects of the types we specify.
        private List<List<ManageableObject>> objects = new List<List<ManageableObject>>();

        public void FindManageableObjectsInScene<T>() where T : ManageableObject
        {
            objects.Add(FindObjectsOfType(typeof(T)).Cast<ManageableObject>().ToList());

            //Remove null entries as a failsafe.
            objects = objects.Where(item => item != null).ToList();
        }

        public void AddManageableObject(ManageableObject obj)
        {
            if (obj != null)
            {
                objects.Add(new List<ManageableObject> { obj });
            }
        }

        public override void OnAwake()
        {
            objects.ForEach(p => p.ForEach(q => q.OnAwake()));
        }

        public override void OnStart()
        {
            objects.ForEach(p => p.ForEach(q => q.OnStart()));
        }

        public override void OnUpdate()
        {
            objects.ForEach(p => p.ForEach(q => q.OnUpdate()));
        }

        public override void OnLateUpdate()
        {
            objects.ForEach(p => p.ForEach(q => q.OnLateUpdate()));
        }

        public override void OnFixedUpdate()
        {
            objects.ForEach(p => p.ForEach(q => q.OnFixedUpdate()));
        }

        public override void OnAbort()
        {
            objects.ForEach(p => p.ForEach(q => q.OnAbort()));
        }
    }
}

