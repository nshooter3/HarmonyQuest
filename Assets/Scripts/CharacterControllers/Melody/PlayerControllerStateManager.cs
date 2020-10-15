namespace Melody
{
    using GameManager;
    using UnityEngine;

    public class PlayerControllerStateManager : ManageableObject
    {
        public static PlayerControllerStateManager instance;

        MelodyController melodyController;
        PauseMenuController pauseMenuController;

        public enum ControllerState { Melody, Pause };
        [HideInInspector]
        public ControllerState controllerState = ControllerState.Melody;

        public override void OnAwake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            melodyController = FindObjectOfType<MelodyController>();
            melodyController.OnAwake();

            pauseMenuController = FindObjectOfType<PauseMenuController>();
            pauseMenuController.OnAwake();
        }

        public override void OnStart()
        {
            melodyController.OnStart();
            pauseMenuController.OnStart();
        }

        public override void OnAbort()
        {
            melodyController.OnAbort();
            pauseMenuController.OnAbort();
        }

        public override void OnUpdate()
        {
            switch (controllerState)
            {
                case ControllerState.Melody:
                    melodyController.OnUpdate();
                    break;
                case ControllerState.Pause:
                    pauseMenuController.OnUpdate();
                    break;
            }
        }

        public override void OnFixedUpdate()
        {
            switch (controllerState)
            {
                case ControllerState.Melody:
                    melodyController.OnFixedUpdate();
                    break;
                case ControllerState.Pause:
                    pauseMenuController.OnFixedUpdate();
                    break;
            }
        }

        // Update is called once per frame
        public override void OnLateUpdate()
        {
            switch (controllerState)
            {
                case ControllerState.Melody:
                    melodyController.OnLateUpdate();
                    break;
                case ControllerState.Pause:
                    pauseMenuController.OnLateUpdate();
                    break;
            }
        }

        public void SetState(ControllerState controllerState)
        {
            this.controllerState = controllerState;
        }
    }
}
