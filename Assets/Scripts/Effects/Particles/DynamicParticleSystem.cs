namespace Effects.Particles
{
    using UnityEngine;
    using HarmonyQuest;

    public class DynamicParticleSystem : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem particleSystem;

        [SerializeField]
        private bool followPlayer = false;
        private Transform melodyTransform;

        private void Start()
        {
            melodyTransform = ServiceLocator.instance.GetMelodyController().GetTransform();
            if (particleSystem == null)
            {
                particleSystem = GetComponent<ParticleSystem>();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (followPlayer == true)
            {
                transform.position = melodyTransform.position;
            }
        }
    }
}
