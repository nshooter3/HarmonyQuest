using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPongObject : MonoBehaviour
{
    private Vector3 pos1 = new Vector3(0,-30,0);
    private Vector3 pos2 = new Vector3(0,30,0);
    public float speed = 1.0f;

    void Update() {
        transform.rotation = Quaternion.Lerp ( Quaternion.Euler(pos1), Quaternion.Euler(pos2), Mathf.PingPong(Time.time*speed, 1.0f));
    }
}
