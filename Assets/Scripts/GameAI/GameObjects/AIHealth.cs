namespace GameAI.AIGameObjects
{
    using GamePhysics;
    using System.Collections.Generic;
    using UnityEngine;
    using UI;

    public class AIHealth
    {
        private AIGameObjectData data;

        private AIStats aiStats;
        private int curHealthBar = 0;
        private int curHealthBarMaxHealth;
        private bool dead = false;
        private List<DamageHitbox> receivedDamageHitboxes = new List<DamageHitbox>();
        private AgentHealthBars agentHealthBarsUI;

        public bool isCountering = false;

        public void Init(AIGameObjectData data)
        {
            this.data = data;
            //Create instance of our scriptable object so we don't edit the original file when changing health values.
            aiStats = Object.Instantiate(this.data.aiStats);
            curHealthBar = aiStats.healthBars.Length - 1;
            curHealthBarMaxHealth = aiStats.healthBars[curHealthBar];

            this.data.CounterDamageReceiver.AssignFunctionToReceiveCounterDamageDelegate(ReceiveDirectDamage);

            //TODO: Make this use Mitch's camera.
            agentHealthBarsUI = AgentHealthBarsPool.instance.GetAgentHealthBar(aiStats.healthBars.Length, data.gameObject.transform, Object.FindObjectOfType<Camera>());
        }

        private void TakeDamage(int damage)
        {
            aiStats.healthBars[curHealthBar] = Mathf.Max(0, aiStats.healthBars[curHealthBar] - damage);
            if (aiStats.healthBars[curHealthBar] <= 0)
            {
                if (curHealthBar > 0)
                {
                    curHealthBar--;
                    curHealthBarMaxHealth = aiStats.healthBars[curHealthBar];
                }
                else
                {
                    Die();
                }
            }
            agentHealthBarsUI.SetMeterValue(curHealthBar, aiStats.healthBars[curHealthBar], curHealthBarMaxHealth);
            if (dead)
            {
                AgentHealthBarsPool.instance.ReturnAgentHealthBarToPool(agentHealthBarsUI);
            }
        }

        private void Die()
        {
            dead = true;
        }

        public void ReceiveDamageHitbox(DamageHitbox damageHitbox)
        {
            if (dead == false)
            {
                if (damageHitbox != null)
                {
                    if (IsDamageHitboxCurrentlyReceived(damageHitbox) == false)
                    {
                        if (isCountering && damageHitbox.counterable == true && WasDamageCountered(damageHitbox.GetAgent()))
                        {
                            DealCounterDamage(damageHitbox);
                        }
                        else
                        {
                            TakeDamage(damageHitbox.GetDamage());
                        }
                        receivedDamageHitboxes.Add(damageHitbox);
                    }
                }
            }
        }

        private void DealCounterDamage(DamageHitbox damageHitbox)
        {
            damageHitbox.ReturnCounterDamageToSource(data.aiStats.counterDamage);
        }

        //Used to receive counter damage and other things not tied to damage hitboxes.
        public void ReceiveDirectDamage(int damage)
        {
            if (dead == false)
            {
                TakeDamage(damage);
            }
        }

        private bool IsDamageHitboxCurrentlyReceived(DamageHitbox newDamageHitbox)
        {
            foreach (DamageHitbox damageHitbox in receivedDamageHitboxes)
            {
                if (newDamageHitbox.GetId() == damageHitbox.GetId())
                {
                    return true;
                }
            }
            return false;
        }

        public void RemoveInactiveReceivedDamageHitboxes()
        {
            for (int i = 0; i < receivedDamageHitboxes.Count; i++)
            {
                if (receivedDamageHitboxes[i].IsActive() == false)
                {
                    receivedDamageHitboxes.RemoveAt(i);
                    i--;
                }
            }
        }

        bool WasDamageCountered(GameObject attacker)
        {
            //Calculate the angle of the absorbed attack by getting the angle between where the damage came from relative to the enemy, and the direction the enemy is facing.
            Vector3 sourceDirection = attacker.transform.position - data.gameObject.transform.position;
            float damageAngle = Vector3.Angle(data.gameObject.transform.forward, sourceDirection);
            //If the damage comes a direction within CounterDegreeRange degrees of where the enemy is facing, we consider it a successful counter. (CounterDegreeRange * 2 degrees total range)
            if (damageAngle <= data.CounterDegreeRange)
            {
                return true;
            }
            return false;
        }

        public bool IsDead()
        {
            return dead;
        }
    }
}
