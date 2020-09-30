namespace HarmonyQuest.Input.Implementation
{
    using UnityEngine;

    public class BasicPlayerInputManager : MonoBehaviour, IPlayerInputManager
    {
        public void OnAwake() { }

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

        public float GetHorizontalMovement2()
        {
            return 0f;
        }

        public float GetVerticalMovement2()
        {
            return 0f;
        }

        public bool HarmonyModeButtonDown()
        {
            return Input.GetKey(KeyCode.F);
        }

        public bool HealButtonDown()
        {
            return Input.GetKey(KeyCode.V);
        }

        public bool LockonButtonDown()
        {
            return Input.GetKey(KeyCode.LeftShift);
        }

        public bool ParryButtonDown()
        {
            return Input.GetKey(KeyCode.C);
        }

        public bool GrappleButtonDown()
        {
            return Input.GetKey(KeyCode.LeftShift);
        }
    }
}
