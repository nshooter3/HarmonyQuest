namespace GameAI.AIGameObjects
{
    using System.Collections.Generic;
    using GamePhysics;
    using UnityEngine;

    public class AIHitboxes
    {
        private AIGameObjectData data;

        //Dictionary that holds lists of hitboxes and their names. Multiple hitboxes can be paired under a single entry in the dictionary if they share a name.
        private Dictionary<string, List<DamageHitbox>> hitboxDictionary;
        private List<DamageHitbox> tempValue;

        public void Init(AIGameObjectData data)
        {
            this.data = data;
            InitHitboxDictionary();
        }

        public virtual Collider[] GetHurtboxes()
        {
            return data.hurtboxes;
        }

        public virtual Collider GetCollisionAvoidanceHitbox()
        {
            return data.collisionAvoidanceHitbox;
        }

        private void InitHitboxDictionary()
        {
            hitboxDictionary = new Dictionary<string, List<DamageHitbox>>();

            //Automatically grab all DamageHitbox attached to our agent and its children.
            List<DamageHitbox> hitboxes = new List<DamageHitbox>(data.gameObject.GetComponentsInChildren<DamageHitbox>());

            foreach (DamageHitbox hitbox in hitboxes)
            {
                if (hitboxDictionary.TryGetValue(hitbox.GetHitboxName(), out tempValue))
                {
                    tempValue.Add(hitbox);
                }
                else
                {
                    hitboxDictionary.Add(hitbox.GetHitboxName(), new List<DamageHitbox>() { hitbox });
                }
            }
        }

        public void ActivateHitbox(string name, float delay, float lifetime, int damage)
        {
            if (hitboxDictionary.TryGetValue(name, out tempValue))
            {
                foreach (DamageHitbox hitbox in tempValue)
                {
                    hitbox.ActivateHitbox(delay, lifetime, damage);
                }
            }
        }

        public void UpdateHitboxes()
        {
            foreach (KeyValuePair<string, List<DamageHitbox>> entry in hitboxDictionary)
            {
                foreach (DamageHitbox hitbox in entry.Value)
                {
                    hitbox.UpdateHitbox();
                }
            }
        }

        public void CancelHitbox(string name)
        {
            if (hitboxDictionary.TryGetValue(name, out tempValue))
            {
                foreach (DamageHitbox hitbox in tempValue)
                {
                    hitbox.CancelHitbox();
                }
            }
        }

        public void CancelAllHitboxes()
        {
            foreach (KeyValuePair<string, List<DamageHitbox>> entry in hitboxDictionary)
            {
                foreach (DamageHitbox hitbox in entry.Value)
                {
                    hitbox.CancelHitbox();
                }
            }
        }
    }
}
