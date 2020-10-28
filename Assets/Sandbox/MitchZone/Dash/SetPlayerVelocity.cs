using UnityEngine;
using Melody;
using GameManager;

public class SetPlayerVelocity : ManageableObject
{
    public MelodyController player;

    public override void OnUpdate()
    {
        if (player != null)
            Shader.SetGlobalVector("_PlayerVelocity", player.rigidBody.velocity);
    }
}
