namespace Melody
{
    using GamePhysics;
    using HarmonyQuest;
    using UI;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Collections;

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

            //Set up our counter hurtbox a bit differently, since it should ignore unblockable attacks.
            damageReceiver = controller.counterHurtbox.gameObject.AddComponent<DamageReceiver>();
            damageReceiver.AssignFunctionToReceiveDamageDelegate(CounterHurtboxReceiveDamageHitbox);

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

        private void TakeDamageDelayed(int damage, float delay = 0f)
        {
            controller.StartCoroutine(TakeDamageDelayedCoroutine(damage, delay));
        }

        private IEnumerator TakeDamageDelayedCoroutine(int damage, float delay = 0f)
        {
            yield return new WaitForSeconds(delay);
            TakeDamage(damage);
        }

        private void TakeDamage(int damage)
        {
            if (UITransitionManager.instance.IsTransitionActive() == false)
            {
                currentHealth = Mathf.Max(0, currentHealth - damage);
                playerHealth.SetMeterValue(currentHealth, MelodyStats.maxHealth);
                controller.melodySound.TakeDamage();
                if (currentHealth <= 0)
                {
                    Die();
                }
                //Debug.Log("MELODY TAKE DAMAGE. CURRENT HEALTH: " + currentHealth);
            }
        }

        public void ReceiveDamageHitbox(DamageHitbox damageHitbox)
        {
            if (dead == false && isDashing == false && damageHitbox != null)
            {
                //Only pay attention to this hitbox if we haven't been hit by it yet.
                if (IsDamageHitboxCurrentlyReceived(damageHitbox) == false)
                {
                    if (damageHitbox.counterable == true && WasDamageCountered(damageHitbox.GetAgent()))
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
                        if (damageHitbox.counterable == true)
                        {
                            damageHitbox.checkForLateCounter = true;
                        }
                        else
                        {
                            damageHitbox.checkForLateDodge = true;
                        }
                    }
                    receivedDamageHitboxes.Add(damageHitbox);
                }
            }
        }

        public void CounterHurtboxReceiveDamageHitbox(DamageHitbox damageHitbox)
        {
            //Our counter hurtbox should only pay attention to counterable hitboxes.
            if (damageHitbox.counterable == true)
            {
                ReceiveDamageHitbox(damageHitbox);
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

        public void CheckForLateDodges()
        {
            for (int i = 0; i < receivedDamageHitboxes.Count; i++)
            {
                if (receivedDamageHitboxes[i].checkForLateDodge == true || receivedDamageHitboxes[i].checkForLateCounter == true)
                {
                    //Debug.Log("LATE DODGE SUCCESS");
                    receivedDamageHitboxes[i].checkForLateDodge = false;
                    receivedDamageHitboxes[i].checkForLateCounter = false;
                    receivedDamageHitboxes[i].applyDamageWhenHitboxEnds = false;
                    receivedDamageHitboxes.RemoveAt(i);
                    i--;
                }
            }
        }

        private void DealCounterDamage(DamageHitbox damageHitbox)
        {
            damageHitbox.ReturnCounterDamageToSource(MelodyStats.counterDamage, controller.gameObject);
        }

        //Used to receive counter damage and other things not tied to damage hitboxes.
        public void ReceiveDirectDamage(int damage, GameObject dealer)
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
                        TakeDamageDelayed(receivedDamageHitboxes[i].GetDamage(), controller.config.PostHitDamageDelay - controller.config.PostCounterGracePeriod);
                    }
                    receivedDamageHitboxes.RemoveAt(i);
                    i--;
                }
            }
        }

        private void Die()
        {
            controller.melodySound.Death();
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

        public float GetCurrentHealth()
        {
            return currentHealth;
        }
    }
}
