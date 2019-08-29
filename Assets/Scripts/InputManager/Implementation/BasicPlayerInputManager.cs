namespace HarmonyQuest.Input
{
    using UnityEngine;

    public class BasicPlayerInputManager : MonoBehaviour, IPlayerInputManager
    {
        public bool AttackButtonDown()
        {
            return Input.GetKey(KeyCode.X);
        }

        public bool DodgeButtonDown()
        {
            return Input.GetKey(KeyCode.Z);
        }

        public float GetHorizontalMovement()
        {
            float movement = 0f;
            if (Input.GetKey(KeyCode.RightArrow))
            {
                movement = 1.0f;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                movement = -1.0f;
            }
            return movement;
        }

        public float GetVerticalMovement()
        {
            float movement = 0f;
            if (Input.GetKey(KeyCode.UpArrow))
            {
                movement = 1.0f;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                movement = -1.0f;
            }
            return movement;
        }

        public bool HarmonyModeButtonDown()
        {
            return Input.GetKey(KeyCode.J);
        }

        public bool HealButtonDown()
        {
            return Input.GetKey(KeyCode.H);
        }

        public bool LockonButtonDown()
        {
            return Input.GetKey(KeyCode.LeftShift);
        }

        public bool ParryButtonDown()
        {
            return Input.GetKey(KeyCode.C);
        }
    }
}
