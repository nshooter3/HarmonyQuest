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
        private AgentHealthBar healthBarUI;

        public void Init(AIGameObjectData data)
        {
            this.data = data;
            //Create instance of our scriptable object so we don't edit the original file when changing health values.
            aiStats = Object.Instantiate(this.data.aiStats);
            curHealthBarMaxHealth = aiStats.healthBars[0];
            //TODO: Set up a meter pooling system to grab this from.
            healthBarUI = Object.FindObjectOfType<AgentHealthBar>();
            //TODO: Make this use Mitch's camera.
            healthBarUI.InitTrackingVars(data.gameObject.transform, Object.FindObjectOfType<Camera>());
            healthBarUI.SetNumHealthBarNotches(aiStats.healthBars.Length);
        }

        private void TakeDamage(int damage)
        {
            aiStats.healthBars[curHealthBar] = Mathf.Max(0, aiStats.healthBars[curHealthBar] - damage);
            if (aiStats.healthBars[curHealthBar] <= 0)
            {
                if (curHealthBar < aiStats.healthBars.Length - 1)
                {
                    curHealthBar++;
                    curHealthBarMaxHealth = aiStats.healthBars[curHealthBar];
                }
                else
                {
                    dead = true;
                }
            }
            healthBarUI.SetMeterValue(aiStats.healthBars[curHealthBar], curHealthBarMaxHealth);
            healthBarUI.SetNumHealthBarNotches(aiStats.healthBars.Length - curHealthBar);
        }

        public void ReceiveDamageHitbox(DamageHitbox damageHitbox)
        {
            if (dead == false)
            {
                if (damageHitbox != null)
                {
                    if (IsDamageHitboxCurrentlyReceived(damageHitbox) == false)
                    {
                        receivedDamageHitboxes.Add(damageHitbox);
                        TakeDamage(damageHitbox.GetDamage());
                    }
                }
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
    }
}
