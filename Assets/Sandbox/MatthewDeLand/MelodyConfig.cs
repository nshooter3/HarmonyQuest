using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelodyConfig : MonoBehaviour
{
    [Header("Basic Movement")]
    public float MaxSpeed = 5;         //Unity Units Per Second
    public float MaxAcceleration = 1;  //Unity Units Per Second
    public float TurningSpeed = 3;

    [Header("Dash Settings")]
    public float DashLength  = 3; 
    public float DashIntroTime  = 0.3f; 
    public float DashOutroTime  = 0.3f;
    public float DashTime  = 0.3f;




    void Start()
    {
        //TODO load values in depending on save data.
    }

}
