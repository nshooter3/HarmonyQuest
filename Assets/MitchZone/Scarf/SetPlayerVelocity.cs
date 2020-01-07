using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPlayerVelocity : MonoBehaviour
{
    public CharacterController player;

    // Update is called once per frame
    void Update()
    {
        if (player != null)
            Shader.SetGlobalVector("_PlayerVelocity", player.velocity);
    }
}
