namespace HarmonyQuest.DynamicCamera
{
    using UnityEngine;
    using System.Collections.Generic;
    using Melody;
    using GameManager;

    public class CameraController : ManageableObject
    {
        private List<CameraBehavior> behaviors = new List<CameraBehavior>();

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

        public override void OnAwake()
        {
            if (!inst)
            {
                inst = this;
            }
            else if (inst != this)
            {
                Destroy(this);
            }
            // Fill behaviors list with camera behaviors
            behaviors.Add(new CameraBehaviorFollowPlayer());
            behaviors.Add(new CameraBehaviorLookAt());
            InitCamera();
            // Activate the first behavior
            if (behaviors[0] != null)
            {
                behaviors[0].ToggleActive(true);
            }
        }

        public override void OnLateUpdate()
        {
            MoveCamera();
            UpdateCamera();
        }

        private void InitCamera()
        {
            IMelodyInfo player = ServiceLocator.instance.GetMelodyController();
            behaviors.ForEach(b => b.Init(transform, player));
        }

        private void MoveCamera()
        {
            behaviors.ForEach(b => b.Move());
        }

        private void UpdateCamera()
        {
            behaviors.ForEach(b => b.Update());
        }

        public void ToggleCamera<T>(bool value)
        {
            behaviors.Find(x => x is T).ToggleActive(value);
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
