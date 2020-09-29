using UnityEngine;
using GameManager;

public class BillboardSprite : ManageableObject
{
    public Camera mainCamera;

    public override void OnUpdate()
    {
        transform.LookAt(mainCamera.transform.position, -Vector3.up);
    }
}
