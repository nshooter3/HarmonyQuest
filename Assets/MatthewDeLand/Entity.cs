using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public EntityType type;

    
    protected float MaxHP = 100;

    [SerializeField]
    protected float CurrentHP;

    
    protected CharacterController CharController;
    protected Animator AnimController;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
