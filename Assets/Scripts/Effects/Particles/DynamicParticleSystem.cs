namespace Effects.Particles
{
    using UnityEngine;
    using HarmonyQuest;
    using GameManager;

    public class DynamicParticleSystem : ManageableObject
    {
        [SerializeField]
        private ParticleSystem particles;

        [SerializeField]
        private bool followPlayer = false;
        private Transform melodyTransform;

        public override void OnStart()
        {
            melodyTransform = ServiceLocator.instance.GetMelodyController().GetTransform();
            if (particles == null)
            {
                particles = GetComponent<ParticleSystem>();
            }
        }

        // Update is called once per frame
        public override void OnUpdate()
        {
            if (followPlayer == true)
            {
                transform.position = melodyTransform.position;
            }
        }
    }
}
