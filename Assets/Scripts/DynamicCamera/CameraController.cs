namespace HarmonyQuest.DynamicCamera
{
    using UnityEngine;
    using System.Collections.Generic;

    public enum CameraBehaviors {FollowPlayer, FollowPointOfInterest}

    public class CameraController : CameraBehavior
    {
        [SerializeField]
        private List<CameraBehavior> behaviors = new List<CameraBehavior>();

        private Vector3 target;
        private float speed = 2f;

        private static CameraController inst;
        public static CameraController instance
        {
            get
            {
                if (!inst)
                {
                    inst = GameObject.FindObjectOfType<CameraController>();
                }
                if (!inst)
                {
                    GameObject camera = Camera.main.gameObject;
                    inst = camera.AddComponent<CameraController>();
                }
                return inst;
            }
        }

        void Awake()
        {
            ToggleCamera(CameraBehaviors.FollowPlayer);
        }
        void LateUpdate()
        {
            MoveCamera();
        }

        private void MoveCamera()
        {
            behaviors.ForEach(b => b.Move());
            // target = followPlayer.Movement() + followPOI.Movement();
            // transform.position = Vector3.Lerp(transform.position, target, speed * Time.deltaTime);
        }

        public void ToggleCamera(CameraBehaviors c)
        {
            behaviors.Find(x => x.type == c).ToggleActive();
        }
    }
}
