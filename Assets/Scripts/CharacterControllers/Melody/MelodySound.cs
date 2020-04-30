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
        private FmodEventHandler attackTonalEvent;

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

        public void AttackMiss()
        {
            attackFailEvent.Play();
            attackFailTonalEvent.Play();
        }

        //ADD THIS
        public void AttackConnect()
        {
            attackHitEvent.Play();
            attackTonalEvent.Play();
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
