namespace Melody
{
    using GameManager;
    using HarmonyQuest;
    using HarmonyQuest.Input;

    //TODO: Write controller manager
    public class PauseMenuController : ManageableObject
    {
        public IPlayerInputManager input { get; private set; }

        public override void OnStart()
        {
            input = ServiceLocator.instance.GetInputManager();
        }

        public override void OnUpdate()
        {
            CheckInputs();
        }

        void CheckInputs()
        {
            if (input.PauseButtonDown())
            {
                PauseManager.TogglePaused(false);
                PlayerControllerStateManager.instance.SetState(PlayerControllerStateManager.ControllerState.Melody);
            }
        }
    }
}
