namespace Melody
{
    using GamePhysics;
    using System.Collections.Generic;
    using UnityEngine;

    public class MelodyHealth
    {
        private MelodyController controller;

        public bool isCountering = false;

        private int currentHealth;
        private bool dead = false;
        private List<DamageHitbox> receivedDamageHitboxes = new List<DamageHitbox>();

        public MelodyHealth(MelodyController controller)
        {
            this.controller = controller;
            this.controller.counterDamageReceiver.AssignFunctionToReceiveCounterDamageDelegate(ReceiveDirectDamage);

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
            Debug.Log("MELODY TAKE DAMAGE. CURRENT HEALTH: " + currentHealth);
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
            damageHitbox.ReturnCounterDamageToSource(MelodyStats.counterDamage);
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

        private void Die()
        {
            dead = true;
        }

        public bool IsDead()
        {
            return dead;
        }

        bool WasDamageCountered(GameObject attacker)
        {
            //Calculate the angle of the absorbed attack by getting the angle between where the damage came from relative to the player, and the direction the player is facing.
            Vector3 sourceDirection = attacker.transform.position - controller.transform.position;
            float damageAngle = Vector3.Angle(controller.transform.forward, sourceDirection);
            //If the damage comes a direction within CounterDegreeRange degrees of where the player is facing, we consider it a successful counter. (CounterDegreeRange * 2 degrees total range)
            if (damageAngle <= controller.config.CounterDegreeRange)
            {
                return true;
            }
            return false;
        }
    }
}
