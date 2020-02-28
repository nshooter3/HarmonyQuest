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

        /// <summary>
        /// If this field is not null, this class's id will be overwritten by the overrideID object's id.
        /// This allows for multiple hitboxes to be treated as one hitbox, so that damage receivers do not perceive themselves as getting hit more than once.
        /// </summary>
        [SerializeField]
        private DamageHitbox overrideID;

        /// <summary>
        /// The debug mesh renderer associated with this hitbox. Used to visualize hitboxes if showDebugRenderer is set to true.
        /// </summary>
        [SerializeField]
        private MeshRenderer debugRenderer;

        [SerializeField]
        private bool showDebugRenderer = false;

        private int damage;

        //Variables pertaining to the delay between when EnableHitbox is called and when the hitbox becomes active
        private bool hitboxDelayed = false;
        private float hitboxDelay;
        private float hitboxDelayTimer;

        //Variables pertaining to the time the hitbox stays active after the delay period.
        private bool hitboxActive = false;
        private float hitboxLifetime;
        private float hitboxLifetimeTimer;

        private Guid id = Guid.NewGuid();

        public void Start()
        {
            if (overrideID != null)
            {
                id = overrideID.id;
            }
            if (debugRenderer != null)
            {
                debugRenderer.enabled = false;
            }
            col.enabled = false;
        }

        public void ActivateHitbox(float delay, float lifetime, int damage)
        {
            col.enabled = false;
            hitboxDelayed = true;
            hitboxDelay = delay;
            hitboxDelayTimer = 0.0f;
            hitboxActive = false;
            hitboxLifetime = lifetime;
            hitboxLifetimeTimer = 0.0f;
            this.damage = damage;
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

        public Guid GetId()
        {
            return id;
        }

        public int GetDamage()
        {
            return damage;
        }

        public string GetHitboxName()
        {
            return hitboxName;
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
