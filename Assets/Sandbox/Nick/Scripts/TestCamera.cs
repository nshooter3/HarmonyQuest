using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCamera : MonoBehaviour
{
    public bool followPlayer = true;
    public Vector3 distanceFromPlayer;

    [SerializeField]
    private TestPlayer player;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (followPlayer)
        {
            transform.position = player.transform.position + distanceFromPlayer;
        }
    }
}
