namespace Melody
{
    using GameAI;
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
        private FmodEventHandler dashTonalEvent;

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

        private string global_combat_proximity_param = "global_combat_proximity_param";
        private string global_puzzle_proximity_param = "global_puzzle_proximity_param";
        private string global_health_param = "global_health_param";

        //Param value enums
        public enum FootstepSurface { Standard = 0 }

        private float maxHealthValueFmod = 100f;

        private float maxPuzzleProximityValueFmod = 100f;

        private IMelodyInfo melodyInfo;
        private AIAgentManager aiAgentManager;

        private bool isInPuzzleZone = false;

        public void Init(IMelodyInfo melodyInfo, AIAgentManager aiAgentManager)
        {
            this.melodyInfo = melodyInfo;
            this.aiAgentManager = aiAgentManager;
        }

        public void OnFixedUpdate()
        {
            isInPuzzleZone = false;
        }

        public void OnUpdate()
        {
            if (landedAttackThisFrame == true)
            {
                AttackConnectSound();
                landedAttackThisFrame = false;
            }
            SetHealthParam();
            SetCombatProximityParam();
            if (isInPuzzleZone == true)
            {
                FmodFacade.instance.SetMusicParam(global_puzzle_proximity_param, maxPuzzleProximityValueFmod);
            }
            else
            {
                FmodFacade.instance.SetMusicParam(global_puzzle_proximity_param, 0f);
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
            dashTonalEvent.Play();
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

        //Param setting functions
        public void SetHealthParam()
        {
            FmodFacade.instance.SetMusicParam(global_health_param, (melodyInfo.GetCurrentHealth()/ melodyInfo.GetMaxHealth()) * maxHealthValueFmod);
        }

        //Scale our fmod combat proximity param from 0 to combatProximityFmodParamMaxValue based on the distance of the closest living ai agent.
        //Clamp this distance value between combatProximatoryMaxVolumeDistance and combatProximatoryMinVolumeDistance to ensure that our fmod param value only changes when enemies are a certain distance away from the player.
        public void SetCombatProximityParam()
        {
            float clampedProximity = Mathf.Clamp(aiAgentManager.aiAgentsUtil.GetClosestAgentDistance(), AIStateConfig.combatProximityMinRange, AIStateConfig.combatProximityMaxRange);
            float proximityRange = AIStateConfig.combatProximityMaxRange - AIStateConfig.combatProximityMinRange;
            float proximityParam = (1.0f - ((clampedProximity - AIStateConfig.combatProximityMinRange) / proximityRange)) * AIStateConfig.combatProximityFmodParamMaxValue;

            FmodFacade.instance.SetMusicParam(global_combat_proximity_param, proximityParam);
        }

        public void PuzzleZoneTriggerEntered()
        {
            isInPuzzleZone = true;
        }
    }
}
