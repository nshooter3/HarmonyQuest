﻿namespace GameAI.AIGameObjects
{
    using GamePhysics;
    using System.Collections.Generic;
    using UnityEngine;

    public class AIHealth
    {
        private AIGameObjectData data;

        private AIStats aiStats;
        private int curHealthBar = 0;
        private bool dead = false;
        private List<DamageHitbox> receivedDamageHitboxes = new List<DamageHitbox>();

        public void Init(AIGameObjectData data)
        {
            this.data = data;
            //Create instance of our scriptable object so we don't edit the original file when changing health values.
            aiStats = Object.Instantiate(this.data.aiStats);
        }

        private void TakeDamage(int damage)
        {
            aiStats.healthBars[curHealthBar] = Mathf.Max(0, aiStats.healthBars[curHealthBar] - damage);
            if (aiStats.healthBars[curHealthBar] <= 0)
            {
                if (curHealthBar < aiStats.healthBars.Length - 1)
                {
                    curHealthBar++;
                }
                else
                {
                    dead = true;
                }
            }
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
