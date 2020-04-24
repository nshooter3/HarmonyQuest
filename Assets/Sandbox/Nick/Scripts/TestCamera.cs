using HarmonyQuest;
using Melody;
using UnityEngine;

public class TestCamera : MonoBehaviour
{
    public bool followPlayer = true;
    public Vector3 distanceFromPlayer;

    private IMelodyInfo player;
    
    // Start is called before the first frame update
    void Start()
    {
        player = ServiceLocator.instance.GetMelodyInfo();
    }

    // Update is called once per frame
    void Update()
    {
        if (followPlayer)
        {
            transform.position = player.GetTransform().position + distanceFromPlayer;
        }
    }
}
