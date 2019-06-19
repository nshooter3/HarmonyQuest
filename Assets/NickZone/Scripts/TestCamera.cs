using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCamera : MonoBehaviour
{
    public bool followPlayer = true;
    public Vector3 distanceFromPlayer;
    public float distanceFromCameraCenter;

    [SerializeField]
    private TestPlayer player;

    private CharacterController characterController;
    private Camera cam;
    private Vector3 target;
    private Vector3 previousPosition;
    
    void Awake()
    {
        cam = GetComponent<Camera>();
        characterController = player.GetComponent<CharacterController>();
        transform.position = player.transform.position + distanceFromPlayer;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!followPlayer)
            return;

        float speed = 2f;
        // Vector3 screenPos = ScaleVectorComponents(cam.WorldToScreenPoint(player.transform.position), 1f/cam.pixelWidth, 1f/cam.pixelHeight, 1f);
        // if (screenPos.x < 0.3f || screenPos.x > 0.7f)
        target = PlayerLocation() + PlayerVelocity() / 1.2f;
        if (IsLockedOn())
        {
            target = Vector3.Lerp(TargetLocation(), PlayerLocation(), 0.5f);
            speed = 9f;
        }
        transform.position = Vector3.Lerp(transform.position, target + distanceFromPlayer, speed * Time.deltaTime);
    }

    private Vector3 PlayerLocation()
    {
        return player.transform.position;
    }

    private Vector3 PlayerVelocity()
    {
        return characterController.velocity;
    }

    private Vector3 TargetLocation()
    {
        return player.lockOnTarget.transform.position;
    }

    private bool IsLockedOn()
    {
        return player.IsLockedOn();
    }

    Vector3 ScaleVectorComponents(Vector3 v, float xScale, float yScale, float zScale)
    {
        v.x *= xScale;
        v.y *= yScale;
        v.z *= zScale;
        return v;
    }
}
