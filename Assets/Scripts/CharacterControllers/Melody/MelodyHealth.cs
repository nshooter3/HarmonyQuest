namespace Melody
{
    using GamePhysics;
    using System.Collections.Generic;
    using UnityEngine;

    public class MelodyHealth
    {
        private MelodyController controller;

        private int currentHealth;
        private bool dead = false;
        private List<DamageHitbox> receivedDamageHitboxes = new List<DamageHitbox>();

        public MelodyHealth(MelodyController controller)
        {
            this.controller = controller;

            currentHealth = MelodyStats.maxHealth;

            DamageReceiver damageReceiver;

            foreach (Collider hurtbox in controller.hurtboxes)
            {
                //Create and attach a DamageReceiver to all our hurtboxes at runtime
                damageReceiver = hurtbox.gameObject.AddComponent<DamageReceiver>();
                damageReceiver.AssignFunctionToReceiveDamageDelegate(ReceiveDamageHitbox);
            }
        }

        public void OnUpdate(float deltaTime)
        {
            RemoveInactiveReceivedDamageHitboxes();
        }

        private void TakeDamage(int damage)
        {
            currentHealth = Mathf.Max(0, currentHealth - damage);
            if (currentHealth <= 0)
            {
                Die();
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

        private void Die()
        {
            dead = true;
        }

        public bool IsDead()
        {
            return dead;
        }
    }
}
