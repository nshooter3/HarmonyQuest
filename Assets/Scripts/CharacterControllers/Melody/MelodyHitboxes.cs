namespace Melody
{
    using System;
    using System.Collections.Generic;
    using GamePhysics;

    public class MelodyHitboxes
    {
        private MelodyController controller;

        //Dictionary that holds lists of hitboxes and their names. Multiple hitboxes can be paired under a single entry in the dictionary if they share a name.
        private Dictionary<string, List<DamageHitbox>> hitboxDictionary;
        private List<DamageHitbox> tempValue;

        public MelodyHitboxes(MelodyController controller)
        {
            this.controller = controller;
            InitHitboxDictionary();
        }

        private void InitHitboxDictionary()
        {
            hitboxDictionary = new Dictionary<string, List<DamageHitbox>>();

            //Automatically grab all DamageHitbox attached to our agent and its children.
            List<DamageHitbox> hitboxes = new List<DamageHitbox>(controller.gameObject.GetComponentsInChildren<DamageHitbox>());

            foreach (DamageHitbox hitbox in hitboxes)
            {
                if (hitboxDictionary.TryGetValue(hitbox.GetHitboxName(), out tempValue))
                {
                    //Ensure that grouped hitboxes in our hitbox dictionary share an id.
                    hitbox.SetId(tempValue[0].GetId());
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
                    hitbox.ActivateHitbox(delay, lifetime, damage, Guid.NewGuid());
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
