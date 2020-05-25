using UnityEngine;
using HarmonyQuest.Audio;

public class AIAnimator
{
    private Animator animator;

    private Vector2 forward2D;
    private Vector2 velocity2D;

    public AIAnimator(Animator animator)
    {
        this.animator = animator;
        forward2D  = new Vector2();
        velocity2D = new Vector2();
    }

    public void OnUpdate()
    {
        animator.SetFloat("NormalizedTime", FmodFacade.instance.GetNormalizedBeatProgress());
    }

    public void SetVelocity(Vector3 rotation, Vector3 velocity, float maxSpeed)
    {
        forward2D.Set(rotation.x, rotation.z);
        velocity2D.Set(velocity.x, velocity.z);

        float speed = velocity.magnitude / Time.deltaTime / maxSpeed;
        if(speed > 0.0f)
        {
            animator.SetFloat("UpDown", Mathf.Cos(Mathf.Deg2Rad * Vector2.Angle(forward2D, velocity2D)));
            animator.SetFloat("LeftRight", Mathf.Sin(Mathf.Deg2Rad * Vector2.Angle(forward2D, velocity2D)));
        }
        else
        {
            animator.SetFloat("UpDown", 0);
            animator.SetFloat("LeftRight", 0);
        }
        animator.SetFloat("Speed", speed);
    }

    public void SetTrigger(string name)
    {
        animator.SetTrigger(name);
    }

    internal void ResetTrigger(string name)
    {
        animator.ResetTrigger(name);
    }

    public void SetBool(string name, bool val)
    {
        animator.SetBool(name, val);
    }
}
