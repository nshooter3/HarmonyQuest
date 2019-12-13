using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPongObject1 : MonoBehaviour
{
    private Vector3 pos1 = new Vector3(0,0,-10);
    private Vector3 pos2 = new Vector3(0,0,10);
    public float speed = 1.0f;

    Vector3 start;

    void Start() {
        start = transform.position;
    }

    void Update() {
        transform.position = Vector3.Lerp ( start + pos1, start + pos2, Mathf.PingPong(Time.time*speed, 1.0f));
    }
}
