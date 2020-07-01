namespace Melody
{
    using UnityEngine;

    public class MelodyAnimationEvents : MonoBehaviour
    {
        public MelodySound melodySound;

        public void PlayFootstepSound()
        {
            melodySound.Footstep();
        }
    }
}