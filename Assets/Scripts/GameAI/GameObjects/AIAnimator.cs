using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAnimator : MonoBehaviour
{
    public Animator animator { get; private set; }

    public AIAnimator()
    {
        animator = gameObject.GetComponent<Animator>();
    }



    public void Attack()
    {
        animator.SetTrigger("Attack");
    }

    internal void AttackComplete()
    {
        animator.ResetTrigger("Attack");
    }
}
