namespace Melody
{
    using HarmonyQuest.Audio;
    using UnityEngine;

    public class MelodySound : MonoBehaviour
    {
        [SerializeField]
        private FmodEventHandler attackHitEvent, attackTonalEvent, parryTonalEvent, healEvent;

        private FmodParamData[] attackHitParamData;

        private FmodParamData[] attackMissParamData;

        private FmodParamData[] healParamData;

        private void Start()
        {
            attackHitParamData = new FmodParamData[] { new FmodParamData(FmodFacade.instance.GetFmodParamFromDictionary("melody_attack_hit_param"),
                                                       (float)FmodDictionary.melody_attack_hit_param_val.great_hit) };

            attackMissParamData = new FmodParamData[] { new FmodParamData(FmodFacade.instance.GetFmodParamFromDictionary("melody_attack_hit_param"),
                                                           (float)FmodDictionary.melody_attack_hit_param_val.missed_attack) };

            healParamData = new FmodParamData[] { new FmodParamData(FmodFacade.instance.GetFmodParamFromDictionary("melody_heal_param"),
                                                           (float)FmodDictionary.melody_heal_param_val.small) };
        }

        public void AttackSwing()
        {
            AttackConnect();
        }

        public void AttackConnect()
        {
            attackHitEvent.Play(attackHitParamData);
            attackTonalEvent.Play();
        }

        public void AttackMiss()
        {
            attackHitEvent.Play(attackMissParamData);
        }

        public void InitCounter()
        {
            
        }

        public void CounterSuccess()
        {
            parryTonalEvent.Play();
        }

        public void Heal()
        {
            healEvent.Play(healParamData);
        }
    }
}
