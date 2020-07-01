using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class TestPlayerInputManager : MonoBehaviour
{
    public static TestPlayerInputManager instance;

    // The Rewired player id of this character
    public int playerId = 0;

    private Player player; // The Rewired Player
    private CharacterController cc;

    private Vector3 moveVector;
    private bool fire;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
        player = ReInput.players.GetPlayer(playerId);

        // Get the character controller
        cc = GetComponent<CharacterController>();
    }

    public float GetHorizontalMovement()
    {
        return player.GetAxis("MoveHorizontal"); // get input by name or action id
    }

    public float GetVerticalMovement()
    {
        return player.GetAxis("MoveVertical");
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
}
