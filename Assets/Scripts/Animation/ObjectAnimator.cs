namespace Animation
{
    using UnityEngine;

    public class ObjectAnimator
    {
        public Animator animator;

        public void ToggleAnimationActive(bool isActive)
        {
            animator.enabled = isActive;
        }
    }
}
