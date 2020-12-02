namespace HarmonyQuest.DynamicCamera
{
    using GameManager;
    using GamePhysics;
    using UnityEngine;

    public class CameraPointOfInterest : ManageableObject
    {
        public Transform mountPoint;

        [SerializeField]
        private CollisionWrapper collisionWrapper;

        public override void OnStart()
        {
            collisionWrapper.AssignFunctionToTriggerEnterDelegate(PlayerEnter);
            collisionWrapper.AssignFunctionToTriggerExitDelegate(PlayerExit);
        }

        void PlayerEnter(Collider other)
        {
            Debug.Log("PLAYER ENTER!");
            CameraController.instance.ToggleCamera<CameraBehaviorLookAt>(true);
            CameraController.instance.GetCameraBehavior<CameraBehaviorLookAt>().targetPoint = this;
        }

        void PlayerExit(Collider other)
        {
            Debug.Log("PLAYER EXIT!");
            CameraController.instance.ToggleCamera<CameraBehaviorLookAt>(false);
            CameraController.instance.GetCameraBehavior<CameraBehaviorLookAt>().targetPoint = null;
        }
    }
}
