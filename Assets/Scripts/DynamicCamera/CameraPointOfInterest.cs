namespace HarmonyQuest.DynamicCamera
{
    using UnityEngine;

    public class CameraPointOfInterest : MonoBehaviour
    {
        private int playerLayer;
        private CameraBehaviors followPOI = CameraBehaviors.FollowPointOfInterest;

        void Awake()
        {
            playerLayer = LayerMask.NameToLayer("Player");
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.layer == playerLayer)
            {
                CameraController.instance.ToggleCamera(followPOI);
            }
        }

        void OnTriggerExit(Collider col)
        {
            print("Prosafd");
            if (col.gameObject.layer == playerLayer)
            {
                CameraController.instance.ToggleCamera(followPOI);
            }
        }
    }
}
