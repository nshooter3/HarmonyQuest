namespace Objects
{
    using GameManager;
    using GamePhysics;
    using Manager;
    using UI;
    using UnityEngine;

    public class SceneTransitionTrigger : ManageableObject
    {
        public string nextScene;
        public string nextDoor;

        private bool hasBeenEntered = false;

        public UITransitionManager.UITransitionType uiTransitionType = UITransitionManager.UITransitionType.FadeOut;

        public CollisionWrapper collisionWrapper;

        public override void OnAwake()
        {
            collisionWrapper.AssignFunctionToTriggerEnterDelegate(OnEnter);
        }

        public void OnEnter(Collider other)
        {
            if (!hasBeenEntered)
            {
                hasBeenEntered = true;
                SceneTransitionManager.PrepareNewScene(nextScene, nextDoor);
                UITransitionManager.instance.StartTransition(uiTransitionType);
            }
        }
    }
}
