using GameManager;
using HarmonyQuest;
using Melody;
using UnityEngine;

public class TestCamera : ManageableObject
{
    public bool followPlayer = true;
    public Vector3 distanceFromPlayer;

    private IMelodyInfo player;

    // Start is called before the first frame update
    public override void OnStart()
    {
        player = ServiceLocator.instance.GetMelodyInfo();
    }

    // Update is called once per frame
    public override void OnUpdate()
    {
        if (followPlayer)
        {
            transform.position = player.GetTransform().position + distanceFromPlayer;
        }
    }
}