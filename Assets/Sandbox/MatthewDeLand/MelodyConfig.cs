using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelodyConfig : MonoBehaviour
{
    public float MaxSpeed = 5;
    [Header("Dash Settings")]
    public float DodgeSpeed  = 5;  //In world units
    public float DashLength  = 3; 
    public float DashIntroTime  = 0.3f; 
    public float DashOutroTime  = 0.3f;
    public float DashTime  = 0.3f;




    void Start()
    {
        //TODO load values in depending on save data.
    }

}
