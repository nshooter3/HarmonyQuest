namespace HarmonyQuest.DynamicCamera
{
    using UnityEngine;

    public class CameraPointOfInterest : MonoBehaviour
    {
        private int playerLayer;

        void Awake()
        {
            playerLayer = LayerMask.NameToLayer("Player");
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.layer == playerLayer)
            {
                CameraController.instance.ToggleCamera<CameraFollowPointOfInterest>(true);
                CameraController.instance.GetCameraBehavior<CameraFollowPointOfInterest>().targetPoint = this.transform;
            }
        }

        void OnTriggerExit(Collider col)
        {
            if (col.gameObject.layer == playerLayer)
            {
                CameraController.instance.ToggleCamera<CameraFollowPointOfInterest>(false);
                CameraController.instance.GetCameraBehavior<CameraFollowPointOfInterest>().targetPoint = null;
            }
        }
    }
}
