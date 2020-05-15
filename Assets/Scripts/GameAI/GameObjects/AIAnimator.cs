using UnityEngine;
using HarmonyQuest.Audio;

public class AIAnimator
{
    private Animator animator;

    public AIAnimator(Animator animator)
    {
        this.animator = animator;
    }

    public void OnUpdate()
    {
        animator.SetFloat("NormalizedTime", FmodFacade.instance.GetNormalizedBeatProgress());
    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
    }

    public void SetVelocity(Vector3 moveDirection, Vector3 rotation, Vector3 velocity, float maxSpeed)
    {
        Debug.Log("velocity.magnitude: " + velocity.magnitude/Time.deltaTime + "   max: " + maxSpeed);
        float speed = velocity.magnitude / Time.deltaTime / maxSpeed;
        if(speed > 0.01)
        {
            animator.SetFloat("UpDown", 1f);
        }
        else
        {
            animator.SetFloat("UpDown", 0);
        }
        animator.SetFloat("Speed", speed);
        //animator.SetFloat("Speed", 1f);
    }

    public void Die()
    {

    }

    internal void AttackComplete()
    {
        animator.ResetTrigger("Attack");
    }
}
