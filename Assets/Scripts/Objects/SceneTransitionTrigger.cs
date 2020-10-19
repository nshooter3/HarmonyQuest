namespace Objects
{
    using GameManager;
    using GamePhysics;
    using Manager;
    using UnityEngine;

    public class SceneTransitionTrigger : ManageableObject
    {
        public string nextScene;
        public string nextDoor;

        public CollisionWrapper collisionWrapper;

        public override void OnAwake()
        {
            collisionWrapper.AssignFunctionToTriggerEnterDelegate(OnEnter);
        }

        public void OnEnter(Collider other)
        {
            //Todo: Some kind of visual transition
            SceneTransitionManager.LoadNewScene(nextScene, nextDoor);
        }
    }
}
