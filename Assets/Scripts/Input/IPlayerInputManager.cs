namespace HarmonyQuest.Input
{
    public interface IPlayerInputManager
    {
        float GetHorizontalMovement();

        float GetVerticalMovement();

        float GetHorizontalMovement2();

        float GetVerticalMovement2();

        bool AttackButtonDown();

        bool ParryButtonDown();

        bool DodgeButtonDown();

        bool HarmonyModeButtonDown();

        bool HealButtonDown();

        bool LockonButtonDown();

        bool GrappleButtonDown();

        bool PauseButtonDown();

        void OnAwake();
    }
}
