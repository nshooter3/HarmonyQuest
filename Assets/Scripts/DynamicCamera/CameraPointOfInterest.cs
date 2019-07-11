namespace HarmonyQuest.DynamicCamera
{
    using UnityEngine;

    public class CameraPointOfInterest : MonoBehaviour
    {
        private int playerLayer;
        private CameraBehaviors followPOI = CameraBehaviors.FollowPointOfInterest;
        private CameraFollowPointOfInterest POICamera;

        void Awake()
        {
            playerLayer = LayerMask.NameToLayer("Player");
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.layer == playerLayer)
            {
                CameraController.instance.ToggleCamera(followPOI);
                POICamera = CameraController.instance.GetCameraBehavior(followPOI) as CameraFollowPointOfInterest;
                POICamera.targetPoint = this.transform;
            }
        }

        void OnTriggerExit(Collider col)
        {
            if (col.gameObject.layer == playerLayer)
            {
                CameraController.instance.ToggleCamera(followPOI);
                POICamera = CameraController.instance.GetCameraBehavior(followPOI) as CameraFollowPointOfInterest;
                POICamera.targetPoint = null;
            }
        }
    }
}
