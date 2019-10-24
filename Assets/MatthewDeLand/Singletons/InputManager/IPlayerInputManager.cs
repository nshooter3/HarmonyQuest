public interface  IPlayerInputManager
{
    float GetHorizontalMovement();
    
    float GetVerticalMovement();
    
    bool AttackButtonDown();
    
    bool ParryButtonDown();
    
    bool DodgeButtonDown();
    
    bool HarmonyModeButtonDown();
    
    bool HealButtonDown();
    
    bool LockonButtonDown();
}
