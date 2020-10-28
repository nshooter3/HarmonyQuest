namespace HarmonyQuest.DynamicCamera
{
    using GameManager;
    using UnityEngine;

    public class CameraPointOfInterest : ManageableObject
    {
        private int playerLayer;

        public override void OnAwake()
        {
            playerLayer = LayerMask.NameToLayer("Player");
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.layer == playerLayer)
            {
                CameraController.instance.ToggleCamera<CameraBehaviorLookAt>(true);
                CameraController.instance.GetCameraBehavior<CameraBehaviorLookAt>().targetPoint = this.transform;
            }
        }

        void OnTriggerExit(Collider col)
        {
            if (col.gameObject.layer == playerLayer)
            {
                CameraController.instance.ToggleCamera<CameraBehaviorLookAt>(false);
                CameraController.instance.GetCameraBehavior<CameraBehaviorLookAt>().targetPoint = null;
            }
        }
    }
}
