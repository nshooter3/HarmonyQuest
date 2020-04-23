namespace Melody
{
    using GamePhysics;
    using HarmonyQuest;
    using UI;
    using System.Collections.Generic;
    using UnityEngine;

    public class MelodyHealth
    {
        private MelodyController controller;

        public bool isCountering = false;
        public bool dealtCounterDamage = false;
        public bool isDashing = false;

        private float postSuccessfulCounterTimer;

        private int currentHealth;
        private bool dead = false;
        private List<DamageHitbox> receivedDamageHitboxes = new List<DamageHitbox>();
        public UIMeter playerHealth;

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

            playerHealth = ServiceLocator.instance.GetUIManager().playerHealth;
        }

        public void OnUpdate(float deltaTime)
        {
            RemoveInactiveReceivedDamageHitboxes();
            if (postSuccessfulCounterTimer > 0)
            {
                postSuccessfulCounterTimer -= Time.deltaTime;
            }
        }

        private void TakeDamage(int damage)
        {
            currentHealth = Mathf.Max(0, currentHealth - damage);
            playerHealth.SetMeterValue(currentHealth, MelodyStats.maxHealth);
            if (currentHealth <= 0)
            {
                Die();
            }
            //Debug.Log("MELODY TAKE DAMAGE. CURRENT HEALTH: " + currentHealth);
        }

        public void ReceiveDamageHitbox(DamageHitbox damageHitbox)
        {
            if (dead == false && isDashing == false && damageHitbox != null)
            {
                //Only pay attention to this hitbox if we haven't been hit by it yet.
                if (IsDamageHitboxCurrentlyReceived(damageHitbox) == false)
                {
                    if (damageHitbox.counterable == true)
                    {
                        if (WasDamageCountered(damageHitbox.GetAgent()))
                        {
                            //Debug.Log("EARLY COUNTER SUCCESS");
                            DealCounterDamage(damageHitbox);
                            dealtCounterDamage = true;
                        }
                        else
                        {
                            //Only apply incoming hitbox damage if we're not just coming off a successful counter.
                            if (postSuccessfulCounterTimer <= 0)
                            {
                                damageHitbox.applyDamageWhenHitboxEnds = true;
                            }
                            //If the player gets hit by a counterable attack, give them until the end of the hitbox to react.
                            damageHitbox.checkForLateCounter = true;
                        }
                    }
                    else
                    {
                        //Ignore incoming hitbox damage for a little bit if we're just coming off a successful counter
                        if (postSuccessfulCounterTimer <= 0)
                        {
                            TakeDamage(damageHitbox.GetDamage());
                        }
                    }
                    receivedDamageHitboxes.Add(damageHitbox);
                }
            }
        }

        public void SetPostSuccessfulCounterTimer()
        {
            postSuccessfulCounterTimer = controller.config.SuccessfulCounterInvincibilityTime;
        }

        //If melody has already been hit by a counterable attack, but counters before the hitbox goes away, we still consider it a successful counter.
        public void CheckForLateCounters()
        {
            for (int i = 0; i < receivedDamageHitboxes.Count; i++)
            {
                if (receivedDamageHitboxes[i].checkForLateCounter == true)
                {
                    //Debug.Log("LATE COUNTER SUCCESS");
                    receivedDamageHitboxes[i].checkForLateCounter = false;
                    receivedDamageHitboxes[i].applyDamageWhenHitboxEnds = false;
                    DealCounterDamage(receivedDamageHitboxes[i]);
                    dealtCounterDamage = true;
                    receivedDamageHitboxes.RemoveAt(i);
                    i--;
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
                    if (receivedDamageHitboxes[i].applyDamageWhenHitboxEnds == true)
                    {
                        receivedDamageHitboxes[i].applyDamageWhenHitboxEnds = false;
                        TakeDamage(receivedDamageHitboxes[i].GetDamage());
                    }
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
            //First, check to see if the player is countering.
            if (isCountering == false)
            {
                return false;
            }

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
