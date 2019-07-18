namespace HarmonyQuest.DynamicCamera
{
    using UnityEngine;
    using System.Collections.Generic;

    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private List<CameraBehavior> behaviors = new List<CameraBehavior>();

        private Vector3 target;

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
            ToggleCamera<CameraFollowPlayer>();
        }

        void LateUpdate()
        {
            MoveCamera();
        }

        private void MoveCamera()
        {
            behaviors.ForEach(b => b.Move());
        }

        public void ToggleCamera<T>()
        {
            behaviors.Find(x => x is T).ToggleActive();
        }

        /**
         * Pass a camera behavior type and get an object back
         * A good lesson in C# generics
         */
        public T GetCameraBehavior<T>() where T : CameraBehavior
        {
            return (T) behaviors.Find(x => x is T);
        }
    }
}
