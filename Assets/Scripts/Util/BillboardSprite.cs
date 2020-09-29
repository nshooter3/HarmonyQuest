using UnityEngine;

public class BillboardSprite : MonoBehaviour
{
    public Camera mainCamera;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(mainCamera.transform.position, -Vector3.up);
    }
}
