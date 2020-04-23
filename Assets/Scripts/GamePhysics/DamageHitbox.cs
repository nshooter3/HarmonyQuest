namespace GamePhysics
{
    using System;
    using UnityEngine;

    public class DamageHitbox : MonoBehaviour
    {
        [SerializeField]
        private string hitboxName;

        [SerializeField]
        private Collider col;

        [SerializeField]
        private CollisionWrapper collisionWrapper;

        [SerializeField]
        private CounterDamageReceiver counterDamageReceiver;

        [SerializeField]
        private GameObject agent;

        /// <summary>
        /// The debug mesh renderer associated with this hitbox. Used to visualize hitboxes if showDebugRenderer is set to true.
        /// </summary>
        [SerializeField]
        private MeshRenderer debugRenderer;

        [SerializeField]
        private bool showDebugRenderer = false;

        private int damage;

        //Variables pertaining to the delay between when EnableHitbox is called and when the hitbox becomes active.
        private bool hitboxDelayed = false;
        private float hitboxDelay;
        private float hitboxDelayTimer;

        //Variables pertaining to the time the hitbox stays active after the delay period.
        private bool hitboxActive = false;
        private float hitboxLifetime;
        private float hitboxLifetimeTimer;

        //Whether or not this hitbox can be countered.
        public bool counterable = true;
        //Bools that get set to true when the player is hit by a counterable attack, but still has a chance to counter late.
        //Hitboxes with this param set to true will apply damage once the hitbox deactivates if it hasn't been countered before then.
        public bool checkForLateCounter = false;
        public bool applyDamageWhenHitboxEnds = false;

        private Guid id;

        public void Start()
        {
            if (debugRenderer != null)
            {
                debugRenderer.enabled = false;
            }
            col.enabled = false;
            collisionWrapper.AssignFunctionToTriggerEnterDelegate(OnHitboxEnter);
        }

        public void ActivateHitbox(float delay, float lifetime, int damage, Guid id, bool counterable = true)
        {
            col.enabled = false;
            hitboxDelayed = true;
            hitboxDelay = delay;
            hitboxDelayTimer = 0.0f;
            hitboxActive = false;
            hitboxLifetime = lifetime;
            hitboxLifetimeTimer = 0.0f;
            checkForLateCounter = false;
            applyDamageWhenHitboxEnds = false;
            this.damage = damage;
            this.id = id;
            this.counterable = counterable;
        }

        public void UpdateHitbox()
        {
            if (hitboxDelayed)
            {
                hitboxDelayTimer += Time.deltaTime;
                if (hitboxDelayTimer >= hitboxDelay)
                {
                    hitboxDelayed = false;
                    hitboxActive = true;
                    col.enabled = true;
                    ToggleDebugRenderer(true);
                }
            }
            else if (hitboxActive)
            {
                hitboxLifetimeTimer += Time.deltaTime;
                if (hitboxLifetimeTimer >= hitboxLifetime)
                {
                    CancelHitbox();
                }
            }
        }

        public void CancelHitbox()
        {
            col.enabled = false;
            hitboxDelayed = false;
            hitboxDelayTimer = 0.0f;
            hitboxActive = false;
            hitboxLifetimeTimer = 0.0f;
            ToggleDebugRenderer(false);
        }

        private void OnHitboxEnter(Collider other)
        {
            DamageReceiver damageReceiver = other.GetComponent<DamageReceiver>();
            if (damageReceiver != null)
            {
                damageReceiver.ReceiveDamage(this);
            }
        }

        public void ReturnCounterDamageToSource(int counterDamage)
        {
            counterDamageReceiver.ReceiveCounterDamage(counterDamage);
        }

        public Guid GetId()
        {
            return id;
        }

        public void SetId(Guid newId)
        {
            id = newId;
        }

        public int GetDamage()
        {
            return damage;
        }

        public string GetHitboxName()
        {
            return hitboxName;
        }

        public GameObject GetAgent()
        {
            return agent;
        }

        public bool IsActive()
        {
            return hitboxDelayed || hitboxActive;
        }

        private void ToggleDebugRenderer(bool enabled)
        {
            if (debugRenderer != null && showDebugRenderer)
            {
                debugRenderer.enabled = enabled;
            }
        }
    }
}
