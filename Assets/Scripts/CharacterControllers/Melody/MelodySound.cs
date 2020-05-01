namespace Melody
{
    using HarmonyQuest.Audio;
    using UnityEngine;

    public class MelodySound : MonoBehaviour
    {
        [SerializeField]
        private FmodEventHandler attackFailEvent;

        [SerializeField]
        private FmodEventHandler attackFailTonalEvent;

        [SerializeField]
        private FmodEventHandler attackHitEvent;

        [SerializeField]
        private FmodEventHandler attackHitTonalEvent;

        [SerializeField]
        private FmodEventHandler attackSwingEvent;

        [SerializeField]
        private FmodEventHandler takeDamageEvent;

        [SerializeField]
        private FmodEventHandler dashEvent;

        [SerializeField]
        private FmodEventHandler deathEvent;

        [SerializeField]
        private FmodEventHandler footstepEvent;

        [SerializeField]
        private FmodEventHandler footstepTonalEvent;

        [SerializeField]
        private FmodEventHandler harmonyModeActivateEvent;

        [SerializeField]
        private FmodEventHandler harmonyModeDeactivateEvent;

        [SerializeField]
        private FmodEventHandler healEvent;

        [SerializeField]
        private FmodEventHandler healTonalEvent;

        [SerializeField]
        private FmodEventHandler lockOffEvent;

        [SerializeField]
        private FmodEventHandler lockOnEvent;

        [SerializeField]
        private FmodEventHandler parryFailEvent;

        [SerializeField]
        private FmodEventHandler parryFailTonalEvent;

        [SerializeField]
        private FmodEventHandler parryEvent;

        [SerializeField]
        private FmodEventHandler parryTonalEvent;

        private bool landedAttackThisFrame = false;

        private void Update()
        {
            if (landedAttackThisFrame == true)
            {
                AttackConnectSound();
                landedAttackThisFrame = false;
            }
        }

        public void AttackMiss()
        {
            attackFailEvent.Play();
            attackFailTonalEvent.Play();
        }

        public void AttackConnect()
        {
            //Set a flag instead of playing the sound directly so that if multiple enemies are hit by a player attack, we only play one sound.
            landedAttackThisFrame = true;
        }

        public void AttackConnectSound()
        {
            attackHitEvent.Play();
            attackHitTonalEvent.Play();
        }

        public void AttackSwing()
        {
            attackSwingEvent.Play();
        }

        public void TakeDamage()
        {
            takeDamageEvent.Play();
        }

        public void Dash()
        {
            dashEvent.Play();
        }

        public void Death()
        {
            deathEvent.Play();
        }

        //ADD THIS
        public void Footstep()
        {
            footstepEvent.Play();
            footstepTonalEvent.Play();
        }

        //Feature not implemented yet
        public void HarmonyModeActivate()
        {
            harmonyModeActivateEvent.Play();
        }

        //Feature not implemented yet
        public void HarmonyModeDeactivate()
        {
            harmonyModeDeactivateEvent.Play();
        }

        //Feature not implemented yet
        public void Heal()
        {
            healEvent.Play();
            healTonalEvent.Play();
        }

        public void LockOff()
        {
            lockOffEvent.Play();
        }

        public void LockOn()
        {
            lockOnEvent.Play();
        }

        //Feature not implemented yet
        public void CounterFail()
        {
            parryFailEvent.Play();
            parryFailTonalEvent.Play();
        }

        public void CounterSuccess()
        {
            parryEvent.Play();
            parryTonalEvent.Play();
        }

        //Param value enums

        public enum FootstepSurface {Standard = 0}
    }
}
