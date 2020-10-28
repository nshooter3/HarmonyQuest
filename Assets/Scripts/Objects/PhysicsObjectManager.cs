namespace Objects
{
    using GameManager;

    public class PhysicsObjectManager : ManageableObject
    {
        PhysicsObject[] physicsObjects;

        // Start is called before the first frame update
        public override void OnStart()
        {
            physicsObjects = FindObjectsOfType<PhysicsObject>();
        }

        // Update is called once per frame
        public override void OnFixedUpdate()
        {
            if (!PauseManager.GetPaused())
            {
                foreach (PhysicsObject physicsObject in physicsObjects)
                {
                    physicsObject.ObjectFixedUpdate();
                }

                foreach (PhysicsObject physicsObject in physicsObjects)
                {
                    physicsObject.ObjectLateFixedUpdate();
                }
            }
        }
    }
}
