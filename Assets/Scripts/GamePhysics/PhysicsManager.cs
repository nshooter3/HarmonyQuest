namespace GamePhysics
{
    using UnityEngine;

    public class PhysicsManager
    {
        public PhysicsManager()
        {
            PauseManager.AssignFunctionToOnPauseDelegate(OnPause);
            PauseManager.AssignFunctionToOnUnpauseDelegate(OnUnpause);
        }

        public static void OnPause()
        {
            Physics.autoSimulation = false;
        }

        public static void OnUnpause()
        {
            Physics.autoSimulation = true;
        }
    }
}