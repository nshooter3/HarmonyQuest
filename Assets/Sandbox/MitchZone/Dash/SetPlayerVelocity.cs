using UnityEngine;
using Melody;

public class SetPlayerVelocity : MonoBehaviour
{
    public MelodyController player;

    // Update is called once per frame
    void Update()
    {
        if (player != null)
            Shader.SetGlobalVector("_PlayerVelocity", player.rigidBody.velocity);
    }
}
