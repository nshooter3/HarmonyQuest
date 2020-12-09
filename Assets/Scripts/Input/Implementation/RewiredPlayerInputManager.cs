namespace HarmonyQuest.Input.Implementation
{
    using UnityEngine;
    using Rewired;

    public class RewiredPlayerInputManager : MonoBehaviour, IPlayerInputManager
    {
        // The Rewired player id. Currently, this will always be 0.
        public int playerId = 0;

        //A RewiredManager is required by Rewired to be in the Scene.
        //Unless a custom one is needed, use the 'DefaultRewiredManager' prefab
        public GameObject RewiredManager;

        // The Rewired Player
        private Player player;

        public void OnAwake()
        {
            Instantiate(RewiredManager);

            // Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
            player = ReInput.players.GetPlayer(playerId);
        }

        public float GetHorizontalMovement()
        {
            return player.GetAxis("MoveHorizontal");
        }

        public float GetVerticalMovement()
        {
            return player.GetAxis("MoveVertical");
        }

        public float GetHorizontalMovement2()
        {
            return player.GetAxis("MoveHorizontal2");
        }

        public float GetVerticalMovement2()
        {
            return player.GetAxis("MoveVertical2");
        }

        public bool AttackButtonDown()
        {
            return player.GetButtonDown("Attack");
        }

        public bool ParryButtonDown()
        {
            return player.GetButtonDown("Parry");
        }

        public bool DodgeButtonDown()
        {
            return player.GetButtonDown("Dodge");
        }

        public bool HarmonyModeButtonDown()
        {
            return player.GetButtonDown("HarmonyMode");
        }

        public bool HealButtonDown()
        {
            return player.GetButtonDown("Heal");
        }

        public bool LockonButtonDown()
        {
            return player.GetButtonDown("Lockon");
        }
        public bool GrappleButtonDown()
        {
            return player.GetButtonDown("Grapple");
        }

        public bool PauseButtonDown()
        {
            return player.GetButtonDown("Pause");
        }

        public bool InteractButtonDown()
        {
            return player.GetButtonDown("Interact");
        }
    }
}