﻿using Melody;
using UnityEngine;
using GameManager;

// [ExecuteInEditMode ]
public class TestScarf : ManageableObject
{
    public float dashProgress = 0f;
    [SerializeField]
    private MelodyController player;
    [SerializeField]
    private Transform ball;

    private Animator scarfAnimator;



    public override void OnStart()
    {
        scarfAnimator = GetComponent<Animator>();
    }

    public override void OnUpdate()
    {
        Shader.SetGlobalVector("_BallLocation", ball.transform.position);
        Shader.SetGlobalVector("_PlayerLocation", player.transform.forward);
        Shader.SetGlobalFloat("_Progress", dashProgress);
        Quaternion rot = player.transform.rotation;
        Quaternion rot2 = ball.transform.rotation;
        Shader.SetGlobalVector("_PlayerRotation", new Vector4(rot.x, rot.y, rot.z, rot.w));
        Shader.SetGlobalVector("_BallRotation", new Vector4(rot2.x, rot2.y, rot2.z, rot2.w));
    }

    public void Dash()
    {
        print("Scarf Dash");
        scarfAnimator.SetTrigger("dash");
    }
}
