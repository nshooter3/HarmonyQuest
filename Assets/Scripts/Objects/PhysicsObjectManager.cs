namespace Objects
{
    using UnityEngine;

    public class PhysicsObjectManager : MonoBehaviour
    {
        PhysicsObject[] physicsObjects;

        // Start is called before the first frame update
        void Start()
        {
            physicsObjects = FindObjectsOfType<PhysicsObject>();
        }

        // Update is called once per frame
        void FixedUpdate()
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
