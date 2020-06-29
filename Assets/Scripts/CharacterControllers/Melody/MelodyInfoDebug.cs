using GameAI;
using UnityEngine;

namespace Melody
{
    public class MelodyInfoDebug : MonoBehaviour, IMelodyInfo
    {

        public MelodyConfig config;
        public AIAgent lockonTarget;

        public MelodyConfig GetConfig()
        {
            return config;
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public Vector3 GetTransformForward()
        {
            return transform.forward;
        }

        public AIAgent GetLockonTarget()
        {
            return lockonTarget;
        }

        public bool HasLockonTarget()
        {
            return false;
        }

		public float GetCurrentHealth()
        {
            return 0f;
        }

        public float GetMaxHealth()
        {
            return 0f;
        }

        public MelodySound GetMelodySound()
        {
            return null;
        }

		public Vector3 GetVelocity()
        {
            return Vector3.zero;
        }    }

}