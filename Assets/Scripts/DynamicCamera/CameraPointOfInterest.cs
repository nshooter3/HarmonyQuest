namespace HarmonyQuest.DynamicCamera
{
    using UnityEngine;

    public class CameraPointOfInterest : MonoBehaviour
    {
        private int playerLayer;
        private CameraFollowPointOfInterest POICamera;

        void Awake()
        {
            playerLayer = LayerMask.NameToLayer("Player");
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.layer == playerLayer)
            {
                CameraController.instance.ToggleCamera<CameraFollowPointOfInterest>();
                CameraController.instance.GetCameraBehavior<CameraFollowPointOfInterest>().targetPoint = this.transform;
            }
        }

        void OnTriggerExit(Collider col)
        {
            if (col.gameObject.layer == playerLayer)
            {
                CameraController.instance.ToggleCamera<CameraFollowPointOfInterest>();
                CameraController.instance.GetCameraBehavior<CameraFollowPointOfInterest>().targetPoint = null;
            }
        }
    }
}
